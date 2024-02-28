using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace LiteDbExplorer.Core
{
    public sealed class FileCollectionReference : CollectionReference
    {
        public FileCollectionReference(string name, DatabaseReference database) : base(name, database)
        {
        }

        protected override IEnumerable<DocumentReference> GetAllItem(ILiteCollection<BsonDocument> liteCollection)
        {
            return LiteCollection.FindAll().Select(bsonDocument => new FileDocumentReference(bsonDocument, this));
        }

        public override void RemoveDocument(DocumentReference document)
        {
            Database.LiteDatabase.FileStorage.Delete(document.LiteDocument["_id"]);
            Items.Remove(document);
        }
        
        public LiteFileInfo<string> GetFileObject(DocumentReference document)
        {
            return Database.LiteDatabase.FileStorage.FindById(document.LiteDocument["_id"]);
        }
    }
}