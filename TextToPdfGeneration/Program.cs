// See https://aka.ms/new-console-template for more information
using iText.Commons.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TextToPdfGeneration.Interfaces;
using TextToPdfGeneration.Models;
using TextToPdfGeneration.Processors;


Console.WriteLine("Hello, World!");

var services = new ServiceCollection();

//IConfiguration configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
//    .AddJsonFile("AppSettings.json").Build();

var configStr = await File.ReadAllTextAsync("AppSettings.json");

Config config = JsonConvert.DeserializeObject<Config>(configStr);

services.AddSingleton<IFileOperations, FileOperations>()
        .AddSingleton<IPdfGenerator, PdfGenerator>()
        .AddSingleton<Config>(config)
        .AddSingleton<TextToPdfProcessor>();

var provider = services.BuildServiceProvider();

var service = provider.GetService<TextToPdfProcessor>();

Task.Run(() => service.Processor(args)).GetAwaiter().GetResult();