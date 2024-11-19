

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class Branch : BaseEntity
    {
        [FirestoreProperty]
        public Department? Department { get; set; }
        [FirestoreProperty]
        public string? DepartmentId { get; set; }
        [FirestoreProperty]
        public List<Employee>? Employees { get; set; }
    }
}
