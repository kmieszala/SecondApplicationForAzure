using Microsoft.EntityFrameworkCore;
using SecondApplicationForAzure.Common.Configuration;
using SecondApplicationForAzure.Model;
using SecondApplicationForAzure.Server.Configuration;
using SecondApplicationForAzure.Server.Data;
using SecondApplicationForAzure.Server.Services;
using SecondApplicationForAzure.Services.Configuration;
using SecondApplicationForAzure.Services.Services.Logs;
using SecondApplicationForAzure.Services.Services.Students;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddDbContext<SecondAppDbContext>(options =>
    options.EnableSensitiveDataLogging(true)
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty.ToString()), ServiceLifetime.Singleton);

// adds a background task - consumer downloading events from the Azure Service Bus queue
builder.Services.AddHostedService<LogConsumerService>();

AddConfigurationSections(builder);
AddServices(builder);
AddAutoMappers(builder);

var app = builder.Build();

app.MapGet("/", () => "Go to \"/swagger/index.html\"");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    var dbFactory = new SecondAppDbContextFactory();

    Console.WriteLine("Migration - START");
    using (var db = dbFactory.CreateDbContext(null))
    {
        db.Database.Migrate();
    }
    Console.WriteLine("Migration - STOP");
}

void AddAutoMappers(WebApplicationBuilder builder)
{
    IServiceCollection serviceCollection = builder.Services.AddAutoMapper(
        typeof(AutoMapperWebConfig),
        typeof(AutoMapperServiceConfig));
}

void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ILogService>(x =>
        ActivatorUtilities.CreateInstance<LogService>(x, "SecondApp.Api"));
    builder.Services.AddScoped<IStudentService, StudentService>();
}
void AddConfigurationSections(WebApplicationBuilder builder)
{
    builder.Services.AddOptions<AzureServiceBusSection>()
        .BindConfiguration(AzureServiceBusSection.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();
}