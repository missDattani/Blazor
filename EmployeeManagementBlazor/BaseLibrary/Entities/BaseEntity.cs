

using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class BaseEntity
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        [Required]
        public string? Name { get; set; }
     
    }
}
