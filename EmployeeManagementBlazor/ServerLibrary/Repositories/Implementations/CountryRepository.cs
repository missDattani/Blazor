using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using ServerLibrary.Repositories.Contracts;


namespace ServerLibrary.Repositories.Implementations
{
    public class CountryRepository : IGenericRepositoryInterface<Country>
    {
        private readonly FirestoreDb _firestoreDb;
        public CountryRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> DeleteById(string id)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Country");
            DocumentReference docRef = collectionReference.Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }

            await docRef.DeleteAsync();
            return Success();
        }

        public async Task<List<Country>> GetAll()
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Country");
            QuerySnapshot snapshots = await collectionReference.GetSnapshotAsync();

            List<Country> countries = new List<Country>();

            foreach (DocumentSnapshot doc in snapshots.Documents)
            {
                if (doc.Exists)
                {
                    Country country = doc.ConvertTo<Country>();
                    countries.Add(country);
                }
            }
            return countries;
        }

        public async Task<Country> GetById(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("Country").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Country country = snapshot.ConvertTo<Country>();
                return country;
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Insert(Country entity)
        {
            if (!await CheckNameAsync(entity.Name!))
            {
                return new GeneralResponse(false, "Country already added");
            }

            CollectionReference collectionReference = _firestoreDb.Collection("Country");
            await collectionReference.AddAsync(entity);
            return new GeneralResponse(false, "Country added successfully");
        }

        public async Task<GeneralResponse> Update(Country entity)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Country");
            DocumentReference countryDoc = collectionReference.Document(entity.Id);
            DocumentSnapshot snapshot = await countryDoc.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return NotFound();
            }
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Name", entity.Name! }
            };
            await countryDoc.UpdateAsync(updates);
            return new GeneralResponse(true, "Country updated successfully");
        }

        private static GeneralResponse NotFound() => new(false, "Sorry country not found");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task<bool> CheckNameAsync(string name)
        {
            CollectionReference collectionReference = _firestoreDb.Collection("Country");
            Query query = collectionReference.WhereEqualTo("Name", name.ToLower());
            QuerySnapshot snapshots = await query.GetSnapshotAsync();
            return snapshots.Count == 0;
        }
    }
}
