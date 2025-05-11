using WOPIHost.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on a specific IP and port (optional step for cross-network access)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5050); // Bind to all IP addresses on port 5050
});

// Register FileService as a Singleton or Scoped service
builder.Services.AddSingleton<FileService>(); // FileService can stay as Singleton if state is not shared
builder.Services.AddHttpClient<WopiDiscoveryService>();  // HttpClient registration for WopiDiscoveryService
builder.Services.AddSingleton<WopiDiscoveryService>(); // WopiDiscoveryService can be Singleton since it holds no state

// Add controllers for the API
builder.Services.AddControllers();

// Enable CORS (Allow external access to your API from any origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow all origins
              .AllowAnyHeader()  // Allow any headers
              .AllowAnyMethod(); // Allow any method (GET, POST, etc.)
    });
});

var app = builder.Build();

// Use CORS (this will apply the "AllowAll" policy to all incoming requests)
app.UseCors("AllowAll");

// Middleware for authorization (if needed)
app.UseAuthorization();

// Map controllers to endpoints
app.MapControllers();

app.Run();
