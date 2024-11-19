
using System.ComponentModel.DataAnnotations;


namespace BaseLibrary.Entities
{
    public class OtherBaseEntity
    {
        public string? Id { get; set; }
        [Required]
        public string? CivilId { get; set; }
        [Required]
        public string? FileNumber { get; set; }
        public string? Other { get; set; }
    }
}
