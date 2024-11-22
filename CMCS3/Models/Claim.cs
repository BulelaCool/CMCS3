using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS3.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required]
        public string? LecturerName { get; set; }

        [Required]
        public int HoursWorked { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        [BindNever]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        [NotMapped] // This tells EF Core to ignore this property in the database
        public IFormFile? SupportingDocuments { get; set; } // File uploaded during form submission

        public string? SupportingDocumentsPath { get; set; } // Path to the saved file (stored in the database)

        [Required] // New property to store selected roles
        public string? SelectedRoles { get; set; }
    }
}