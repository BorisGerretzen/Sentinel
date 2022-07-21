using Microsoft.Extensions.Configuration;

IConfigurationBuilder? builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

IConfigurationRoot? config = builder.Build();
AppSettings? appConfig = config.GetSection("Settings").Get<AppSettings>();
Main main = new(appConfig);
main.Start();