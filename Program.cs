using Microsoft.EntityFrameworkCore;
using Korp_Teste_JoaoComini.Data;
using Korp_Teste_JoaoComini.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register services
builder.Services.AddDbContext<BancoContexto>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<FaturamentoService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<BancoContexto>();
    contexto.Database.EnsureCreated();
}

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");

app.MapGet("/", () => Results.Ok(new { message = "Korp API is running", swagger = "/swagger" }));
app.MapControllers();

app.Run();
