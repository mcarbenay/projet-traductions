using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Data
{
    /// <summary>
    /// The main EF Core DbContext for the translation management platform.
    /// </summary>
    public class PolyglotInitiativeDbContext : DbContext
    {
        public PolyglotInitiativeDbContext(DbContextOptions<PolyglotInitiativeDbContext> options) : base(options) { }

        public DbSet<Solution> Solutions => Set<Solution>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Component> Components => Set<Component>();
        public DbSet<ResourceFile> ResourceFiles => Set<ResourceFile>();
        public DbSet<TranslatableResource> TranslatableResources => Set<TranslatableResource>();
        public DbSet<ResourceTranslation> ResourceTranslations => Set<ResourceTranslation>();
        public DbSet<TranslationNeed> TranslationNeeds => Set<TranslationNeed>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserSolutionAccess> UserSolutionAccesses => Set<UserSolutionAccess>();
        public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
        public DbSet<ExternalIdentifier> ExternalIdentifiers => Set<ExternalIdentifier>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserSolutionAccess composite key
            modelBuilder.Entity<UserSolutionAccess>()
                .HasKey(usa => new { usa.UserId, usa.SolutionId });

            // Relationships
            modelBuilder.Entity<Solution>()
                .HasMany(s => s.Projects)
                .WithOne(p => p.Solution)
                .HasForeignKey(p => p.SolutionId);

            modelBuilder.Entity<Solution>()
                .HasMany(s => s.TranslationNeeds)
                .WithOne(tn => tn.Solution)
                .HasForeignKey(tn => tn.SolutionId);

            modelBuilder.Entity<Solution>()
                .HasOne(s => s.Owner)
                .WithMany(u => u.OwnedSolutions)
                .HasForeignKey(s => s.OwnerId);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Components)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.ExternalIdentifier)
                .WithMany(ei => ei.Projects)
                .HasForeignKey(p => p.ExternalIdentifierId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Component>()
                .HasMany(c => c.ResourceFiles)
                .WithOne(rf => rf.Component)
                .HasForeignKey(rf => rf.ComponentId);

            modelBuilder.Entity<ResourceFile>()
                .HasMany(rf => rf.TranslatableResources)
                .WithOne(tr => tr.ResourceFile)
                .HasForeignKey(tr => tr.ResourceFileId);

            modelBuilder.Entity<TranslatableResource>()
                .HasMany(tr => tr.ResourceTranslations)
                .WithOne(rt => rt.TranslatableResource)
                .HasForeignKey(rt => rt.TranslatableResourceId);

            modelBuilder.Entity<TranslationNeed>()
                .HasMany(tn => tn.ResourceTranslations)
                .WithOne(rt => rt.TranslationNeed)
                .HasForeignKey(rt => rt.TranslationNeedId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserSolutionAccesses)
                .WithOne(usa => usa.User)
                .HasForeignKey(usa => usa.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ModifiedTranslations)
                .WithOne(rt => rt.ModifiedBy)
                .HasForeignKey(rt => rt.ModifiedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ModifiedExternalIdentifiers)
                .WithOne(ei => ei.ModifiedBy)
                .HasForeignKey(ei => ei.ModifiedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserSolutionAccess>()
                .HasOne(usa => usa.Solution)
                .WithMany()
                .HasForeignKey(usa => usa.SolutionId);

            modelBuilder.Entity<ApiKey>()
                .HasOne(ak => ak.Solution)
                .WithMany()
                .HasForeignKey(ak => ak.SolutionId);
        }
    }
}