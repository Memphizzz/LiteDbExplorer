using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace LiteDbExplorer.Wpf
{
    public class LocalPaletteHelper : PaletteHelper
    {
        public virtual void InitTheme(bool isDark)
        {
            SetLightDark(isDark);
        }

        public override void SetLightDark(bool isDark)
        {
            if (!TryFindAndReplaceMergedDictionary(
                @"(\/MaterialDesignThemes.Wpf;component\/Themes\/MaterialDesignTheme\.)((Light)|(Dark))",
                $"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.{(isDark ? "Dark" : "Light")}.xaml")
            )
            {
                throw new ApplicationException("Unable to find Light/Dark base theme in Application resources.");
            }

            TryFindAndReplaceMergedDictionary(
                @"(\/MahApps.Metro;component\/Styles\/Accents\/)((BaseLight)|(BaseDark))",
                $"pack://application:,,,/MahApps.Metro;component/Styles/Accents/{(isDark ? "BaseDark" : "BaseLight")}.xaml");

            TryFindAndReplaceMergedDictionary(
                @"(\/LiteDbExplorer.Wpf;component\/Themes\/ApplicationColors\.)((Light)|(Dark))",
                $"pack://application:,,,/LiteDbExplorer.Wpf;component/Themes/ApplicationColors.{(isDark ? "Dark" : "Light")}.xaml");

            TryFindAndReplaceMergedDictionary(
                @"(\/MaterialDesignExtensions;component\/Themes\/MaterialDesign((Light)|(Dark))Theme)",
                $"pack://application:,,,/MaterialDesignExtensions;component/Themes/{(isDark ? "MaterialDesignDarkTheme" : "MaterialDesignLightTheme")}.xaml");
        }

        private bool TryFindAndReplaceMergedDictionary(string pattern, string newResourceDictionarySource)
        {
            var existingResourceDictionary = FindResourceDictionary(pattern);
            if (existingResourceDictionary == null)
            {
                return false;
            }

            SwitchMergedDictionaries(existingResourceDictionary, newResourceDictionarySource);

            return true;
        }

        private void SwitchMergedDictionaries(
            ResourceDictionary oldResourceDictionary,
            string newResourceDictionarySource)
        {
            var newResourceDictionary = new ResourceDictionary {Source = new Uri(newResourceDictionarySource)};
            SwitchMergedDictionaries(oldResourceDictionary, newResourceDictionary);
        }

        private void SwitchMergedDictionaries(ResourceDictionary oldResourceDictionary,
            ResourceDictionary newResourceDictionary)
        {
            Application.Current.Resources.MergedDictionaries.Remove(oldResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(newResourceDictionary);
        }

        private ResourceDictionary FindResourceDictionary(string pattern)
        {
            return Application.Current.Resources
                .MergedDictionaries
                .SelectMany(p => p.MergedDictionaries)
                .Where(rd => rd.Source != null)
                .FirstOrDefault(rd => Regex.Match(rd.Source.OriginalString, pattern).Success);
        }
    }
}