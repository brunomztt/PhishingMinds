var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PhishingMinds.Server.Data.DbConnectionFactory>();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS (ANTES DO BUILD)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("https://localhost:51634")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Pipeline
app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); // ✅ aqui tá certo

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

// Teste conexão
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<PhishingMinds.Server.Data.DbConnectionFactory>();

    using (var connection = dbFactory.CreateConnection())
    {
        connection.Open();
        Console.WriteLine("Conectou com o MySQL!");
    }
}

app.Run();