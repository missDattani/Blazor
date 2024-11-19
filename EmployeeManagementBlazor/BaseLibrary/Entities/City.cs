
using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class City : BaseEntity
    {
        [FirestoreProperty]
        public Country? Country { get; set; }
        [FirestoreProperty]
        public string? CountryId { get; set; }
        [FirestoreProperty]
        public List<Town>? Towns { get; set; }
    }
}
