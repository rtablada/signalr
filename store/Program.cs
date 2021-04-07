using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace store
{
    public class Program
    {
        private static HubConnection connection;

        public static void Main(string[] args)
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:4000/status")
                .WithAutomaticReconnect()
                .Build();

            Task.Run(() => ConnectAndSendMessage());

            CreateHostBuilder(args).Build().Run();
        }

        private static async Task ConnectAndSendMessage()
        {
            await connection.StartAsync();

            await connection.InvokeAsync("StoreConnected", "03");

            connection.Reconnected += async (connectionId) => {
                await connection.InvokeAsync("StoreConnected", "03");
            };
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
