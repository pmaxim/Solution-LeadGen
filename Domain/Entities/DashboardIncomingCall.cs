using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class DashboardIncomingCall
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Now;
        public virtual Account Account { get; set; }
        public ICollection<CallIncoming> CallIncomings { get; set; } = new List<CallIncoming>();
    }
}
