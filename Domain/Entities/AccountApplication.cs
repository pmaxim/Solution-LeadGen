using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    //Vonage application
    public class AccountApplication
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(36)]
        public string AppId { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public virtual Account Account { get; set; }
    }
}
