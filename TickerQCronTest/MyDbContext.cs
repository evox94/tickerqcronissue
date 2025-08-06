using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.Configurations;

namespace TickerQCronTest;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply TickerQ entity configurations explicitly
        builder.ApplyConfiguration(new TimeTickerConfigurations());
        builder.ApplyConfiguration(new CronTickerConfigurations());
        builder.ApplyConfiguration(new CronTickerOccurrenceConfigurations());

        // Override table mappings for all entities to remove schemas
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            builder.Entity(entityType.ClrType).ToTable(entityType.GetTableName());
        }
    }
}