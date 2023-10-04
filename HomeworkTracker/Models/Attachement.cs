using HomeworkTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public class Attachement
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Path { get; set; } /* добавите это */
        [Required]
        public Exercise Exercise { get; set; }// добавить ссылку на ответ, проверять в контроллере, если вложение уже прикреплено к задаче не добавлять его в ответ
    }
}
