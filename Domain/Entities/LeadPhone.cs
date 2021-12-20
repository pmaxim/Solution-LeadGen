using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class LeadPhone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(14)]
        public string Phone { get; set; }

        [Required]
        public string UserName { get; set; }

        public bool IsCall { get; set; }
        public string Error { get; set; }
    }
}
