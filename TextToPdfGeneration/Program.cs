// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TextToWordGeneration.Interfaces;
using TextToWordGeneration.Models;
using TextToWordGeneration.Processors;

var services = new ServiceCollection();

//IConfiguration configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
//    .AddJsonFile("AppSettings.json").Build();

var configStr = await File.ReadAllTextAsync("AppSettings.json");

Config config = JsonConvert.DeserializeObject<Config>(configStr);

services.AddSingleton<IFileOperations, FileOperations>()
        .AddSingleton<IWordDocGenerator, WordDocGenerator>()
        .AddSingleton<Config>(config)
        .AddSingleton<TextToWordProcessor>();

var provider = services.BuildServiceProvider();

var service = provider.GetService<TextToWordProcessor>();

Task.Run(() => service.Processor(args)).GetAwaiter().GetResult();