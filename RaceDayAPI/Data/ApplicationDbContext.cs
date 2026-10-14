using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;


namespace RaceDayAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<WeatherInfo> WeatherInfos { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  USERS
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_User_Role",
                    "Role IN ('Organiser', 'Participant')"));
            });

            //EVENTS
            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasOne(e => e.Organiser)
                      .WithMany(u => u.Events)
                      .HasForeignKey(e => e.OrganiserId)
                      .OnDelete(DeleteBehavior.Restrict);   // ← Changed from Cascade

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Event_Status", "Status IN ('Open', 'Closed', 'Cancelled')");
                    t.HasCheckConstraint("CK_Event_Type", "EventType IN ('run', 'walk', 'cycle')");
                });
            });

            //  CATEGORIES 
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.Events)
                      .WithMany(e => e.Categories)
                      .HasForeignKey(c => c.EventId)
                      .OnDelete(DeleteBehavior.Cascade);      // ← OK to cascade
            });

            //  ENROLMENTS
            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.HasIndex(e => new { e.EventId, e.ParticipantId })
                      .IsUnique();

                entity.HasOne(e => e.Participant)
                      .WithMany(u => u.Enrolments)
                      .HasForeignKey(e => e.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict);     // ← Restrict

                entity.HasOne(e => e.Events)
                      .WithMany(ev => ev.Enrolments)
                      .HasForeignKey(e => e.EventId)
                      .OnDelete(DeleteBehavior.Restrict);     // ← Restrict (was Cascade → caused the bug)

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Enrolments)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);     // ← Restrict

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Enrolment_Status",
                    "Status IN ('Confirmed', 'Pending', 'Cancelled')"));
            });

            //  RESULTS
            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasIndex(r => r.EnrolmentId)
                      .IsUnique();

                entity.HasOne(r => r.Enrolment)
                      .WithOne(e => e.Result)
                      .HasForeignKey<Result>(r => r.EnrolmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Result_Status",
                    "Status IN ('Pending', 'Official', 'Disqualified')"));
            });

            //  WEATHERINFOS 
            modelBuilder.Entity<WeatherInfo>(entity =>
            {
                entity.HasOne(w => w.Events)
                      .WithMany(e => e.WeatherInfos)
                      .HasForeignKey(w => w.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AUDITLOGS 
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);     // ← Restrict to avoid cascade conflict
            });

        }
    }
}
