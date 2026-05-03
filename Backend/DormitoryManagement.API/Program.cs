using Serilog;
using DormitoryManagement.API.Configuration;
using DormitoryManagement.API.Data;
using DormitoryManagement.API.Middleware;
using DormitoryManagement.API.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/dormitory-api-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services
        .AddOptions<DatabaseOptions>()
        .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
        .ValidateDataAnnotations()
        .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
            "Database:ConnectionString is required.")
        .ValidateOnStart();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Dormitory Management API", Version = "v1" });
    });

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:8080",
                    "http://127.0.0.1:5500",    // VS Code Live Server
                    "http://localhost:5500")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Data access layer (Scoped — one instance per HTTP request)
    builder.Services.AddScoped<IEducationPlaceRepository, EducationPlaceRepository>();
    builder.Services.AddScoped<IStudentRepository, StudentRepository>();

    // Service layer
    builder.Services.AddScoped<IEducationPlaceService, EducationPlaceService>();
    builder.Services.AddScoped<IStudentService, StudentService>();

    var app = builder.Build();

    // Global error handling — registered first so it wraps the entire pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Dormitory Management API is starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
