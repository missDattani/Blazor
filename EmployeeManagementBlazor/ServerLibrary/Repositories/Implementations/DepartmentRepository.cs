using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using ServerLibrary.Repositories.Contracts;


namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository : IGenericRepositoryInterface<Department>
    {
        private readonly FirestoreDb _firestoreDb;
        public DepartmentRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> DeleteById(string id)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Department");
            DocumentReference docRef = collectionReference.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }

            await docRef.DeleteAsync();
            return Success();
        }

        public async Task<List<Department>> GetAll()
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Department");
            QuerySnapshot snapshots = await collectionReference.GetSnapshotAsync();

            List<Department> departments = new List<Department>();

            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Department department = doc.ConvertTo<Department>();
                    departments.Add(department);
                }
            }
            return departments;
        }

        public async Task<Department> GetById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("Department").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Department department = snapshot.ConvertTo<Department>();
                return department;
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Insert(Department entity)
        {
            if (!await CheckNameAsync(entity.Name!))
            {
                return new GeneralResponse(false, "Department already added");
            }

            CollectionReference collectionReference = _firestoreDb.Collection("Department");
            await collectionReference.AddAsync(entity);
            return new GeneralResponse(false, "Department added successfully");
        }

        public async Task<GeneralResponse> Update(Department entity)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Department");
            DocumentReference departmentDoc = collectionReference.Document(entity.Id);
            DocumentSnapshot snapshot = await departmentDoc.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Name", entity.Name! }
            };
            await departmentDoc.UpdateAsync(updates);
            return new GeneralResponse(true, "Department updated successfully");
        }

        private static GeneralResponse NotFound() => new(false, "Sorry department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task<bool> CheckNameAsync(string name)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
            Query query = collectionReference.WhereEqualTo("Name", name.ToLower());
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            return snapshots.Count == 0;
        }
    }
}
