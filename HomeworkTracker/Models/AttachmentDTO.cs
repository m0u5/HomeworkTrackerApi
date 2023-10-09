using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public class AttachmentDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Path { get; set; }
        
    }
}
