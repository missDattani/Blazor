using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using ServerLibrary.Repositories.Contracts;


namespace ServerLibrary.Repositories.Implementations
{
    public class TownRepository : IGenericRepositoryInterface<Town>
    {
        private readonly FirestoreDb _firestoreDb;
        public TownRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> DeleteById(string id)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Town");
            DocumentReference docRef = collectionReference.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }

            await docRef.DeleteAsync();
            return Success();
        }

        public async Task<List<Town>> GetAll()
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Town");
            QuerySnapshot snapshots = await collectionReference.GetSnapshotAsync();

            List<Town> towns = new List<Town>();

            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Town town = doc.ConvertTo<Town>();
                    towns.Add(town);
                }
            }
            return towns;
        }

        public async Task<Town> GetById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("Town").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Town town = snapshot.ConvertTo<Town>();
                return town;
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Insert(Town entity)
        {
            if (!await CheckNameAsync(entity.Name!))
            {
                return new GeneralResponse(false, "Town already added");
            }

            CollectionReference collectionReference = _firestoreDb.Collection("Town");
            await collectionReference.AddAsync(entity);
            return new GeneralResponse(false, "Town added successfully");
        }

        public async Task<GeneralResponse> Update(Town entity)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Town");
            DocumentReference townDoc = collectionReference.Document(entity.Id);
            DocumentSnapshot snapshot = await townDoc.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Name", entity.Name! }
            };
            await townDoc.UpdateAsync(updates);
            return new GeneralResponse(true, "Town updated successfully");
        }

        private static GeneralResponse NotFound() => new(false, "Sorry town not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task<bool> CheckNameAsync(string name)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Town");
            Query query = collectionReference.WhereEqualTo("Name", name.ToLower());
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            return snapshots.Count == 0;
        }
    }
}
