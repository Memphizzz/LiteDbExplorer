using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Forge.Forms;
using Forge.Forms.Annotations;
using JetBrains.Annotations;
using LiteDB;
using LiteDbExplorer.Core;

namespace LiteDbExplorer.Modules.Shared
{
    [Title("Add document options")]
    [Action(ACTION_CANCEL, "CANCEL", IsCancel = true, ClosesDialog = true)]
    [Action(ACTION_OK, "CREATE", IsDefault = false, ClosesDialog = true, Validates = true)]
    // [Action(ACTION_OK_AND_EDIT, "CREATE AND EDIT", IsDefault = true, ClosesDialog = true, Validates = true)]
    public class AddDocumentOptions : INotifyPropertyChanged, IActionHandler
    {
        private readonly CollectionReference _collection;
        public const string ACTION_OK = "ok";
        public const string ACTION_OK_AND_EDIT = "ok_and_edit";
        public const string ACTION_CANCEL = "cancel";

        private BsonType _idType;
        private NewIdLookup _newIdLookup;
        private string _newIdString;

        public AddDocumentOptions(CollectionReference collection)
        {
            _collection = collection;

            EditAfterCreate = true;

            Reset();
        }

        [UsedImplicitly]
        public IReadOnlyList<BsonType> HandledIdTypes => NewIdLookup.HandledIdTypes;

        [Field(Name = "New _id type", ToolTip = "Enter with new _id type.")]
        [SelectFrom("{Binding HandledIdTypes}", SelectionType = SelectionType.ComboBox)]
        public BsonType IdType
        {
            get => _idType;
            set
            {
                _idType = value;
                SetNewId();
            }
        }

        [Field(Name = "New _id")]
        [Value(Must.BeInvalid, When = "{Binding IdIsInvalid}", Message = "Value {Value} is invalid for {Binding IdType}.")]
        [Action("reset", "RESET")]
        public string NewIdString
        {
            get => _newIdString;
            set
            {
                _newIdString = value;
                ParseValue();
            }
        }

        public bool EditAfterCreate { get; set; }


        public BsonValue NewId { get; private set; }

        public bool IdIsValid { get; private set; }

        public bool IdIsInvalid => !IdIsValid;

        public void HandleAction(IActionContext actionContext)
        {
            if (actionContext.Action is "reset")
            {
                Reset();
                ModelState.Validate(this, nameof(NewIdString));
            }
        }

        public void Reset()
        {
            _newIdLookup = new NewIdLookup(_collection);
            IdType = _newIdLookup.IdType;
            SetNewId();
        }

        private void SetNewId()
        {
            _newIdLookup.IdType = IdType;
            NewId = _newIdLookup.NewId;
            IdIsValid = true;
            _newIdString = NewId.ToString();
            OnPropertyChanged(nameof(NewIdString));
        }

        private void ParseValue()
        {
            IdIsValid = NewIdLookup.TryParse(IdType, NewIdString, out var bsonValue);
            if (IdIsValid)
            {
                NewId = bsonValue;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [UsedImplicitly]
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}