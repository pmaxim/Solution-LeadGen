using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Shared.Models
{
    public class MyProfileViewModel
    {
        public int Id { get; set; }
        public string? OpenTime { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public string? LastVisit { get; set; }
        public int AmountOfVisits { get; set; }
        public string? Src { get; set; }
        [Required]
        public string? Sid { get; set; }
        [Required]
        public string? Token { get; set; }
        public string? FriendlyName { get; set; }
        public string? Balance { get; set; }
        public string? DateCreated { get; set; }
    }
}
