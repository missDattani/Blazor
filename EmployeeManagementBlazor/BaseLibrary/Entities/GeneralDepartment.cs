

using Google.Cloud.Firestore;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class GeneralDepartment : BaseEntity
    {
        [FirestoreProperty]
        public List<Department>? Departments { get; set; }
    }
}
