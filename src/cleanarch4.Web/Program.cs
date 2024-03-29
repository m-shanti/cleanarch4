using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using cleanarch4.Core;
using cleanarch4.Infrastructure;
using cleanarch4.Infrastructure.Data;
using cleanarch4.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder();

string connectionString = builder.Configuration.GetConnectionString("SqlConnection");  //Configuration.GetConnectionString("DefaultConnection");

builder.WebHost.ConfigureServices((context, services) =>
  {
    bool isProduction = context.HostingEnvironment.IsProduction();
    builder.Services.AddDbContext<AppDbContext>(
      options =>
      {
        if (isProduction)
        {
          options.UseSqlServer(
            connectionString,
            x => x.MigrationsAssembly("cleanarch4.Infrastructure.Sql"));
        }
        else
        {
          options.UseSqlite(
            connectionString,
            x => x.MigrationsAssembly("cleanarch4.Infrastructure.Sqlite"));
        }
      });
  }
);


builder.Services.AddHealthChecks();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

//builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

// string connectionString = builder.Configuration.GetConnectionString("SqlConnection");  //Configuration.GetConnectionString("DefaultConnection");
//
// bool isProduction = false;
//
// builder.WebHost.ConfigureAppConfiguration((context, configurationBuilder) =>
// {
//   // isProduction = context.HostingEnvironment.IsProduction();
//   // if (isProduction)
//   // {
//   //   builder.Services.AddDbContextSqlServer(connectionString);
//   // }
//   // else
//   // {
//   //   builder.Services.AddDbContextSqlLite(connectionString);
//   // }
//   
//   var configuration = context.Configuration;
//   var provider = configuration.GetValue("environment", "Unknown"); // sql, sqllite
//
//   Console.WriteLine($"Provider={provider}.");
//   
//   builder.Services.AddDbContext<AppDbContext>(
//     options => _ = provider switch
//     {
//       "Development" => options.UseSqlite(
//         connectionString,
//         x => x.MigrationsAssembly("cleanarch4.Infrastructure.Sqlite")),
//
//       "Production" => options.UseSqlServer(
//         connectionString,
//         x => x.MigrationsAssembly("cleanarch4.Infrastructure.Sql")),
//
//       _ => throw new Exception($"Unsupported provider: {provider}")
//     });
// });

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
  {
    options.SignIn.RequireConfirmedAccount = true;
  })
  .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
  googleOptions.ClientId = "131297295554-b6lpi1aqobvscii7qr6ss96f3rmsf54e.apps.googleusercontent.com";
  googleOptions.ClientSecret = "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";
});

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
  c.EnableAnnotations();
});

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});


builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule(new DefaultCoreModule());
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

app.UseEndpoints(endpoints =>
{
  endpoints.MapDefaultControllerRoute();
  endpoints.MapRazorPages();
});

app.MapHealthChecks("/healthz");

// Seed Database
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  try
  {
      AppDbContext context = services.GetRequiredService<AppDbContext>();
      context.Database.Migrate();
      //context.Database.EnsureCreated();
      SeedData.Initialize(services);
  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
  }
}

app.Run();
