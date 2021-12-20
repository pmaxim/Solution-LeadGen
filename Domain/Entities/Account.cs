using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    //Account for Twilio and Vonage
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [MaxLength(40)]
        [Required]
        public string Sid { get; set; }

        [Required]
        [MaxLength(40)]
        public string Token { get; set; }

        public bool IsVonage { get; set; }

        public bool IsActive { get; set; }

        public string Title { get; set; }

        public ICollection<AccountApplication> AccountApplications { get; set; } = new List<AccountApplication>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
