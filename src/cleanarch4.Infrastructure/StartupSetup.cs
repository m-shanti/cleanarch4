using cleanarch4.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cleanarch4.Infrastructure;

public static class StartupSetup
{
  public static void AddDbContextSqlLite(this IServiceCollection services, string connectionString) =>
      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlite(connectionString)); // will be created in web project root
  
  public static void AddDbContextSqlServer(this IServiceCollection services, string connectionString) =>
    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString)); // will be created in web project root
}
