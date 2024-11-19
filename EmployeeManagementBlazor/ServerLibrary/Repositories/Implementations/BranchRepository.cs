using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using ServerLibrary.Repositories.Contracts;


namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository : IGenericRepositoryInterface<Branch>
    {
        private readonly FirestoreDb _firestoreDb;
        public BranchRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> DeleteById(string id)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Branch");
            DocumentReference docRef = collectionReference.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }

            await docRef.DeleteAsync();
            return Success();
        }

        public async Task<List<Branch>> GetAll()
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Branch");
            QuerySnapshot snapshots = await collectionReference.GetSnapshotAsync();

            List<Branch> branches = new List<Branch>();

            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Branch branch = doc.ConvertTo<Branch>();
                    branches.Add(branch);
                }
            }
            return branches;
        }

        public async Task<Branch> GetById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("Branch").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Branch branch = snapshot.ConvertTo<Branch>();
                return branch;
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Insert(Branch entity)
        {
            if (!await CheckNameAsync(entity.Name!))
            {
                return new GeneralResponse(false, "Branch already added");
            }

            CollectionReference collectionReference = _firestoreDb.Collection("Branch");
            await collectionReference.AddAsync(entity);
            return new GeneralResponse(false, "Branch added successfully");
        }

        public async Task<GeneralResponse> Update(Branch entity)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Branch");
            DocumentReference branchDoc = collectionReference.Document(entity.Id);
            DocumentSnapshot snapshot = await branchDoc.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Name", entity.Name! }
            };
            await branchDoc.UpdateAsync(updates);
            return new GeneralResponse(true, "Branch updated successfully");
        }
        private static GeneralResponse NotFound() => new(false, "Sorry branch not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task<bool> CheckNameAsync(string name)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Branch");
            Query query = collectionReference.WhereEqualTo("Name", name.ToLower());
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            return snapshots.Count == 0;
        }
    }
}
