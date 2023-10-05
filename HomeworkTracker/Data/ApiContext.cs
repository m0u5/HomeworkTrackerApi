using HomeworkTracker.Models;
using HomeworkTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeworkTrackerApi.Data
{
    public class ApiContext:DbContext
    {
        public DbSet<AnswerAttachment> AnswerAttachments { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<Attachement> Attachement { get; set;}
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }
    }
}
