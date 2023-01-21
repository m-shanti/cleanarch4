using System.Reflection;
using cleanarch4.Core.ProjectAggregate;
using cleanarch4.SharedKernel;
using cleanarch4.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace cleanarch4.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
  private readonly IDomainEventDispatcher? _dispatcher;

  public AppDbContext(DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher)
      : base(options)
  {
    _dispatcher = dispatcher; 
  }

  public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<KnowledgePill> KnowledgePills => Set<KnowledgePill>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
