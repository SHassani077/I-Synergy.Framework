﻿using ISynergy.Framework.MessageBus.Azure.Extensions;
using ISynergy.Framework.MessageBus.Sample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ISynergy.Framework.MessageBus.Sample.Publisher
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

                var serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddOptions()
                    .AddMessageBusAzurePublishIntegration<TestDataModel>(config)
                    .AddSingleton<ApplicationAzure>()
                    .BuildServiceProvider();

                var application = serviceProvider.GetRequiredService<ApplicationAzure>();
                application.RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }

            return 0;
        }

        
    }
}
