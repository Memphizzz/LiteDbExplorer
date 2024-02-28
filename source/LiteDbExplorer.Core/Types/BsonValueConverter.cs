using System;
using System.Collections.Generic;
using LiteDB;

namespace LiteDbExplorer.Core
{
    public class BsonValueConverter
    {
        public static IReadOnlyDictionary<BsonType, Func<BsonValue>> ConvertibleTypes(BsonValue bsonValue)
        {
            var options = new Dictionary<BsonType, Func<BsonValue>>();

            if (bsonValue.Type == BsonType.Null || 
                bsonValue.Type == BsonType.Document || 
                bsonValue.Type == BsonType.Array ||
                bsonValue.Type == BsonType.Binary ||
                bsonValue.Type == BsonType.String && bsonValue.AsString.Length > 64)
            {
                return options;
            }

            if (bsonValue.Type != BsonType.String)
            {
                options.Add(BsonType.String, () => new BsonValue(bsonValue.ToString()));
            }

            if (bool.TryParse(bsonValue.ToString(), out var boolResult))
            {
                options.Add(BsonType.Boolean, () => new BsonValue(boolResult));
            }

            if (int.TryParse(bsonValue.ToString(), out var int32Result))
            {
                options.Add(BsonType.Int32, () => new BsonValue(int32Result));
            }

            if (long.TryParse(bsonValue.ToString(), out var int64Result))
            {
                options.Add(BsonType.Int64, () => new BsonValue(int64Result));
            }

            if (double.TryParse(bsonValue.ToString(), out var doubleResult))
            {
                options.Add(BsonType.Double, () => new BsonValue(doubleResult));
            }

            if (decimal.TryParse(bsonValue.ToString(), out var decimalResult))
            {
                options.Add(BsonType.Decimal, () => new BsonValue(decimalResult));
            }

            if (DateTime.TryParse(bsonValue.ToString(), out var dateTimeResult))
            {
                options.Add(BsonType.DateTime, () => new BsonValue(dateTimeResult));
            }

            if (Guid.TryParse(bsonValue.ToString(), out var guidResult))
            {
                options.Add(BsonType.Guid, () => new BsonValue(guidResult));
            }

            return options;
        }
    }
}