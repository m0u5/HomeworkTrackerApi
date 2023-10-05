using HomeworkTrackerApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeworkTracker.Models
{
    public class Exercise
    {
        [Key]
        
        public Guid Id { get; set; } 
        [Required]
        public string Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public DateTime? DeadLine { get; set; }
        [Required]
        public string StudentLogin { get; set; }
        
        public bool IsCompleted { get; set; }

        //ссылка на ответы
        public List<Answer>? Answers { get; set; }

        //Ссылка на прикрепленные файлы
        public List<Attachement>? Attachements { get; set;}
    }
}
