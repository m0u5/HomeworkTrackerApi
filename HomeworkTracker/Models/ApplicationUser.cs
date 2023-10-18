using HomeworkTracker.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public class ApplicationUser:IdentityUser
    {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<Exercise>? Exercises { get; set; }
        public List<Answer>? Answers { get; set; }
    }
}
