using HomeworkTracker.Models;
using HomeworkTrackerApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeworkTrackerApi.Data
{
    public class ApiContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<AnswerAttachment> AnswerAttachments { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<ExerciseAttachment> Attachments { get; set;}

        
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }
    }
}
