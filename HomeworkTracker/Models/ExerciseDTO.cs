using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public class ExerciseDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime DeadLine { get; set; }
        [Required]
        public string StudentLogin { get; set; }

        public bool IsCompleted { get; set; }

    }
}
