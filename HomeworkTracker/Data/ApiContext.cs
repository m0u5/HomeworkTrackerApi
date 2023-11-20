using HomeworkTracker.Models;
using HomeworkTrackerApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeworkTrackerApi.Data
{
    public class ApiContext:IdentityDbContext<ApplicationUser>
    {
        //public DbSet<AnswerAttachment> AnswerAttachments { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Answer> Answer { get; set; }
        //public DbSet<ExerciseAttachment> Attachments { get; set;}
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.AttachedFiles)
                .WithOne()
                //.HasForeignKey(a => a.AttachableId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasMany(a => a.AttachedFiles)
                .WithOne()
                //.HasForeignKey(a => a.AttachableId)
                .HasPrincipalKey(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
