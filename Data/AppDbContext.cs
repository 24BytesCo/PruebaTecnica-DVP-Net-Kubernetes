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

        // Configurar la relación entre User y WorkTask
        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedWorkTasks)
            .WithOne(wt => wt.CreatedByUserNavigation)
            .HasForeignKey(wt => wt.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade); // Ajustar según sea necesario
    }


    public DbSet<WorkTask>? WorkTasks { get; set; }
    public DbSet<WorkTaskStatus>? WorkTaskStatuses { get; set; }

}