

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class Department : BaseEntity
    {
        [FirestoreProperty]
        public GeneralDepartment? GeneralDepartment { get; set; }
        [FirestoreProperty]
        public string? GeneralDepartmentId { get; set; }
        [FirestoreProperty]
        public List<Branch>? Branches { get; set; }
    }
}
