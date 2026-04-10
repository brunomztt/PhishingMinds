var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<PhishingMinds.Server.Data.DbConnectionFactory>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

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
