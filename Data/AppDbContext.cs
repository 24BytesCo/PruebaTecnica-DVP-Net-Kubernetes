using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.Data;
public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkTask>()
            .HasOne(w => w.CreatedByUserNavigation)
            .WithMany(u => u.CreatedWorkTasks)
            .HasForeignKey(w => w.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkTask>()
            .HasOne(w => w.AssignedToUserNavigation)
            .WithMany(u => u.AssignedWorkTasks)
            .HasForeignKey(w => w.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkTask>()
            .HasOne(w => w.WorkTaskStatusNavigation)
            .WithMany(u => u.WorkTask)
            .HasForeignKey(w => w.WorkTaskStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }


    public DbSet<WorkTask>? WorkTasks { get; set; }
    public DbSet<WorkTaskStatus>? WorkTaskStatuses { get; set; }

}