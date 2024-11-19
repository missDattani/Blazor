

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class RefreshTokenInfo
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? RToken { get; set; }
        [FirestoreProperty]
        public string? UserId { get; set; }
        [FirestoreProperty]
        public DateTime? CreatedAt { get; set; }
        [FirestoreProperty]
        public DateTime? UpdatedAt { get; set; }
    }
}
