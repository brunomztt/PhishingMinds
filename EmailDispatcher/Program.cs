using EmailDispatcher;
using EmailDispatcher.Application.Services;
using EmailDispatcher.Infrastructure.Data;
using EmailDispatcher.Infrastructure.Email;
using EmailDispatcher.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<MySqlConnectionFactory>();

builder.Services.AddScoped<CampaignRepository>();

builder.Services.AddScoped<EmailSender>();

builder.Services.AddScoped<CampaignService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
