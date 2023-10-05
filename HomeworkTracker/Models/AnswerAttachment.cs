using HomeworkTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public class AnswerAttachment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Path { get; set; }

        [Required]    
        public Answer Answer { get; set; }
    }
}
