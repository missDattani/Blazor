

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class Country : BaseEntity
    {
        [FirestoreProperty]
        public List<City>? Cities { get; set; }
    }
}
