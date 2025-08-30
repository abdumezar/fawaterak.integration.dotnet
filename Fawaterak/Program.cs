using Fawaterak.Options;
using Fawaterak.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// OpenAPI + Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Fawaterak Payment Integration API",
        Version = "v1",
        Description = "A minimal, production-ready ASP.NET Core Web API that integrates with the Fawaterak payment gateway",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Fawaterak Integration",
            Url = new Uri("https://github.com/abdumezar/fawaterak.integration.dotnet")
        }
    });

    // Configure JSON property naming for Swagger to match Newtonsoft.Json attributes
    c.UseAllOfToExtendReferenceSchemas();
    c.SupportNonNullableReferenceTypes();
});

// Options binding
builder.Services.Configure<FawaterakOptions>(builder.Configuration.GetSection("Fawaterak"));

// HttpClient factory
builder.Services.AddHttpClient();

// Services
builder.Services.AddScoped<IFawaterakPaymentService, FawaterakPaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fawaterak Payment API v1");
        c.RoutePrefix = "swagger"; // Swagger UI at /swagger
        c.DocumentTitle = "Fawaterak Payment Integration API";
        c.DisplayRequestDuration();
    });
    
    // Keep the original OpenAPI endpoint as well
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();