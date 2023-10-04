using HomeworkTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public class Answer
    {
        [Key]
        public Guid Id { get; set; }

        public string? TextAnswer { get; set; }
        public List<Attachement> Attachements { get; set; }//возможно в этом нет необходимости
        public Exercise Exercise { get; set; }
    }
}
