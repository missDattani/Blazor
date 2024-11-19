

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class Town : BaseEntity
    {
        [FirestoreProperty]
        public List<Employee>? Employees { get; set; }
        [FirestoreProperty]
        public City? City { get; set; }
        [FirestoreProperty]
        public string? CityId { get; set; }
    }
}
