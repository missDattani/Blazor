

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class ApplicationUser
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? FullName { get; set; }
        [FirestoreProperty]
        public string? Email { get; set; }
        [FirestoreProperty]
        public string? Password { get; set; }
    }
}
