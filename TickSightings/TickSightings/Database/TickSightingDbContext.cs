using Microsoft.EntityFrameworkCore;

namespace TickSightings.Database;

public class TickSightingsDbContext(DbContextOptions<TickSightingsDbContext> options) : DbContext(options)
{
    public DbSet<SightingEntity> Sightings => Set<SightingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var sighting = modelBuilder.Entity<SightingEntity>();

        sighting.HasKey(s => s.Id);

        sighting.HasIndex(s => s.Date);
        sighting.HasIndex(s => s.City);
        sighting.HasIndex(s => s.Region);
        sighting.HasIndex(s => s.Species);
        sighting.HasIndex(s => s.Source);
    }
}