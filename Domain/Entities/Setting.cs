using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public partial class Setting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }
}