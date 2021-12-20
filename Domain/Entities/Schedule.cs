using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public bool IsAveryDay { get; set; }
    }
}
