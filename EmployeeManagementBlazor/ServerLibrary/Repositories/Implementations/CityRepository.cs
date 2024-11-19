

using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CityRepository : IGenericRepositoryInterface<City>
    {
        private readonly FirestoreDb _firestoreDb;
        public CityRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> DeleteById(string id)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("City");
            DocumentReference docRef = collectionReference.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }

            await docRef.DeleteAsync();
            return Success();
        }

        public async Task<List<City>> GetAll()
        {
            CollectionReference collectionReference = _firestoreDb.Collection("City");
            QuerySnapshot snapshots = await collectionReference.GetSnapshotAsync();

            List<City> cities = new List<City>();

            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    City city = doc.ConvertTo<City>();
                    cities.Add(city);
                }
            }
            return cities;
        }

        public async Task<City> GetById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("City").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                City city = snapshot.ConvertTo<City>();
                return city;
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Insert(City entity)
        {
            if (!await CheckNameAsync(entity.Name!))
            {
                return new GeneralResponse(false, "City already added");
            }

            CollectionReference collectionReference = _firestoreDb.Collection("City");
            await collectionReference.AddAsync(entity);
            return new GeneralResponse(false, "City added successfully");
        }

        public async Task<GeneralResponse> Update(City entity)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("City");
            DocumentReference cityDoc = collectionReference.Document(entity.Id);
            DocumentSnapshot snapshot = await cityDoc.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Name", entity.Name! }
            };
            await cityDoc.UpdateAsync(updates);
            return new GeneralResponse(true, "City updated successfully");
        }
        private static GeneralResponse NotFound() => new(false, "Sorry city not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task<bool> CheckNameAsync(string name)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("City");
            Query query = collectionReference.WhereEqualTo("Name", name.ToLower());
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            return snapshots.Count == 0;
        }
    }
}
