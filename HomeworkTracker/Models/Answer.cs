using HomeworkTracker.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeworkTrackerApi.Models
{
    public class Answer
    {
        [Key]
        
        public Guid Id { get; set; } 
//ВОЗМОЖНО СТОИТ СДЕЛАТЬ ДТО И ДЛЯ ЭТОЙ МОДЕЛИ, НО Я НЕ ЗНАЮ КАК ПРИВЯЗЫВАТЬ К ЗАДАНИЯМ
        public string? TextAnswer { get; set; }
        public List<AnswerAttachment>? Attachments { get; set; }//возможно в этом нет необходимости
        public Exercise Exercise { get; set; }
    }
}
