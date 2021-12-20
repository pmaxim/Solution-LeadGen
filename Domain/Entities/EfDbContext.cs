using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    public partial class EfDbContext : DbContext
    {
        public EfDbContext()
        {
        }

        public EfDbContext(DbContextOptions<EfDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountApplication> AccountApplications { get; set; }
        public virtual DbSet<VonageWebhook> VonageWebhooks { get; set; }
        public virtual DbSet<TwilioWebhook> TwilioWebhooks { get; set; }
        public virtual DbSet<LeadPhone> LeadPhones { get; set; }
        public virtual DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<DashboardIncomingCall> DashboardIncomingCalls { get; set; }
        public virtual DbSet<CallIncoming> CallIncomings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasIndex(z => z.Name);

            modelBuilder.Entity<VonageWebhook>().HasIndex(z => z.From);
            modelBuilder.Entity<VonageWebhook>().HasIndex(z => z.To);

            modelBuilder.Entity<TwilioWebhook>().HasIndex(z => z.To);
            modelBuilder.Entity<TwilioWebhook>().HasIndex(z => z.From);
            modelBuilder.Entity<TwilioWebhook>().HasIndex(z => z.Direction);

            modelBuilder.Entity<LeadPhone>().HasIndex(z => z.Phone);
            modelBuilder.Entity<LeadPhone>().HasIndex(z => z.UserName);

            modelBuilder.Entity<Schedule>().HasIndex(z => z.UserName);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}