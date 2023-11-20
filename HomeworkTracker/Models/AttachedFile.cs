using System.ComponentModel.DataAnnotations;

namespace HomeworkTrackerApi.Models
{
    public interface IAttachable
    {
        Guid Id { get; set; }
        List<AttachedFile>? AttachedFiles { get; set; }
    }
    public class AttachedFile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? AttachableId { get; set; }
        //[Required]
        //public string? AttachableType { get; set; }
    }
}
