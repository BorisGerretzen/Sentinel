using Microsoft.Extensions.Configuration;
using Sentinel.Settings;

IConfigurationBuilder? builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

IConfigurationRoot? config = builder.Build();
ExportSettings? appConfig = config.GetSection("Export").Get<ExportSettings>();
Main main = new(appConfig);
main.Start();