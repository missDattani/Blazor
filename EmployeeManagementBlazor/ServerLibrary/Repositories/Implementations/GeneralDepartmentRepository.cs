using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class GeneralDepartmentRepository : IGenericRepositoryInterface<GeneralDepartment>
    {
        private readonly FirestoreDb _firestoreDb;
        public GeneralDepartmentRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> DeleteById(string id)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
            DocumentReference docRef = collectionReference.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if(!snapshot.Exists)
            {
                return NotFound();
            }
           
            await docRef.DeleteAsync();
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll()
        {
            CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
            QuerySnapshot snapshots = await collectionReference.GetSnapshotAsync();

            List<GeneralDepartment> departments = new List<GeneralDepartment>();

            foreach (DocumentSnapshot doc in snapshots.Documents) 
            { 
                if(doc.Exists)
                {
                    GeneralDepartment department = doc.ConvertTo<GeneralDepartment>();
                    departments.Add(department);
                }
            }
            return departments;
        }

        public async Task<GeneralDepartment> GetById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("GeneralDepartment").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if(snapshot.Exists)
            {
                GeneralDepartment department = snapshot.ConvertTo<GeneralDepartment>();
                return department;
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Insert(GeneralDepartment entity)
        {
            var checkIfNull = await CheckNameAsync(entity.Name!);
           if (!checkIfNull)
            {
                return new GeneralResponse(false, "Department already added");
            }

            CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
            await collectionReference.AddAsync(entity);
            return new GeneralResponse(false,"Department added successfully");
        }

        //public async Task<GeneralResponse> Update(GeneralDepartment entity)
        //{
        //    CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
        //    DocumentReference departmentDoc = collectionReference.Document(entity.Id);
        //    DocumentSnapshot snapshot = await departmentDoc.GetSnapshotAsync();
        //    if (!snapshot.Exists)
        //    {
        //        return NotFound();
        //    }
        //    Dictionary<string, object> updates = new Dictionary<string, object>
        //    {
        //        { "Name", entity.Name! }
        //    };
        //    await departmentDoc.UpdateAsync(updates);
        //    return new GeneralResponse(true, "Department updated successfully");
        //}

        public async Task<GeneralResponse> Update(GeneralDepartment entity)
        {
            try
            {
                CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
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
                var response = new GeneralResponse(true, "Department updated successfully");

                return response; // Ensure this works with JSON serialization
            }
            catch (Exception ex)
            {
                return  new GeneralResponse(false, $"Error: {ex.Message}");
            }
        }


        private static GeneralResponse NotFound() => new(false, "Sorry department not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task<bool> CheckNameAsync(string name)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("GeneralDepartment");
            Query query = collectionReference.WhereEqualTo("Name", name);
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            return snapshots.Count == 0 ? true : false;
        }
    }
}
