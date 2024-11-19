

using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    [FirestoreData]
    public class Employee : BaseEntity
    {
        [FirestoreProperty]
        [Required]
        public string? CivilId { get; set; }
        [FirestoreProperty]
        [Required]
        public string? FileNumber { get; set; }
        [FirestoreProperty]
        [Required]
        public string? FullName { get; set; }
        [FirestoreProperty]
        [Required]
        public string? JobName { get; set; }
        [FirestoreProperty]
        [Required]
        public string? Address { get; set; }
        [FirestoreProperty]
        [Required, DataType(DataType.PhoneNumber)]
        public string? Phonenumber { get; set; }
        [FirestoreProperty]
        [Required]
        public string? Photo { get; set; }
        [FirestoreProperty]
        public string? Other { get; set; }
        [FirestoreProperty]
        public Branch? Branch { get; set; }
        [FirestoreProperty]
        public string? BranchId { get; set;}
        [FirestoreProperty]
        public Town? Town { get; set; }
        [FirestoreProperty]
        public string? TownId { get;set; }

    }
}
