using LiteDB;

namespace LiteDbExplorer.Core
{
    public static class JsonSerializerExtension
    {

        public static string SerializeDecoded(this BsonValue bsonValue, bool pretty = false)
        {
            var json = JsonSerializer.Serialize(bsonValue);

            return EncodingExtensions.DecodeEncodedNonAsciiCharacters(json);
        }
    }
}