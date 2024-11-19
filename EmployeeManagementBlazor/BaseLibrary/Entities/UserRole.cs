using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class UserRole
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? RoleId { get; set;}
        [FirestoreProperty]
        public string? UserId { get; set; }
    }
}
