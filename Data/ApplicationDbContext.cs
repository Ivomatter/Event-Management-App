using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using Microsoft.AspNetCore.Identity;

namespace NemeTschek.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        //public System.Data.Entity.DbSet<ApplicationUser> User { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Event> Event { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Team> Team { get; set; }

        public Microsoft.EntityFrameworkCore.DbSet<UsersEvents> UsersEvents { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<UsersInterests> UsersInterests  { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<UsersTeams> UsersTeams { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Interest> Interests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UsersEvents>().HasKey(bc => new { bc.UserId, bc.EventId });
            modelBuilder.Entity<UsersTeams>().HasKey(bc => new { bc.UserId, bc.TeamId });
            modelBuilder.Entity<UsersInterests>().HasKey(bc => new { bc.UserId, bc.InterestId });
            //  modelBuilder.Entity<>
            /*
            modelBuilder.Entity<Event>()
                .HasOne<UsersEvents>(e => e.User)
                .WithMany(c => c.Events)
                .Map(cs =>
            {
                cs.MapLeftKey("ApplicationUsersId");
                cs.MapRightKey("EventsEventId");
                cs.ToTable("ApplicationUserEvent");
            });
            */
            //modelBuilder.Entity<ApplicationUser>()
            //   .HasMany(a => a.Events)
            //   .WithMany(e => e.ApplicationUsers)
            //   .Map(cs =>
            //   {
            //       cs.MapLeftKey("EventId");
            //       cs.MapRightKey("ApplicationUserId");
            //       cs.ToTable("ApplicationUserEvent");
            //   });
            /*
            modelBuilder.Entity<Team>()
                .HasMany<ApplicationUser>(e => e.ApplicationUsers)
                .WithMany(c => c.Teams)
                .Map(cs =>
                {
                    cs.MapLeftKey("ApplicationUserId");
                    cs.MapRightKey("TeamId");
                    cs.ToTable("UserTeam");
                });           
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            */
        }
    }
}