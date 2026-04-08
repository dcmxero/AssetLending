using Application.Services;
using Infrastructure;
using Infrastructure.Repositories.AssetManagement;
using Infrastructure.Repositories.Identity;
using Infrastructure.Seeds;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IAssetCategoryRepository, AssetCategoryRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAssetCategoryService, AssetCategoryService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<INotificationService, ConsoleNotificationService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile);
    }
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors();

// Auto-migrate and seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (context.Database.IsRelational())
    {
        context.Database.Migrate();
    }
    else
    {
        context.Database.EnsureCreated();
    }
}

if (app.Environment.IsDevelopment())
{
    DataSeed.Initialize(app.Services);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Entry point class. Partial declaration to make it accessible for integration tests.
/// </summary>
public partial class Program;

