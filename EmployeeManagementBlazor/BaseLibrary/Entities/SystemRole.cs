

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class SystemRole
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? Name { get; set; }
    }
}
