using HomeworkTracker.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeworkTrackerApi.Models
{
    public class Answer:IAttachable
    {
        [Key]
        
        public Guid Id { get; set; } 

        public string? TextAnswer { get; set; }
        //public List<AnswerAttachment>? Attachments { get; set; }
        public Exercise? Exercise { get; set; }
       public List<AttachedFile>? AttachedFiles { get; set; }
    }
}
