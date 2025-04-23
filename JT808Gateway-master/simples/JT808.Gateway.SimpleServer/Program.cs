using JT808.Gateway.Abstractions.Enums;
using JT808.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using JT808.Gateway.SimpleServer.Impl;
using JT808.Gateway.SimpleServer.Services;
using JT808.Gateway.Abstractions;
using Microsoft.AspNetCore.Hosting;
using JT808.Gateway.Abstractions.Configurations;
using Microsoft.AspNetCore.Builder;
using JT808.Gateway.Extensions;

namespace JT808.Gateway.SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            builder.Services.AddSingleton<JT1078VideoTriggerService>();
            builder.Services.AddJT808UnificationSendService();
                //使用内存队列实现会话通知
                .AddSingleton<JT808SessionService>()
                .AddSingleton<IJT808SessionProducer, JT808SessionProducer>()
                .AddSingleton<IJT808SessionConsumer, JT808SessionConsumer>()
                .AddJT808Configure()
                .AddGateway(builder.Configuration)
                .AddMessageHandler<JT808MessageHandlerImpl>()
                .AddMsgLogging<JT808MsgLogging>()
                .AddSessionNotice()
                .AddTransmit(builder.Configuration)
                .AddTcp()
                .AddUdp()
                .Builder();

            builder.Services.AddControllers();
            var app = builder.Build();
            app.UseRouting();
            app.MapControllers();
            app.Run();
            var scope = app.Services.CreateScope();
            var triggerService = scope.ServiceProvider.GetRequiredService<JT1078VideoTriggerService>();

            await triggerService.SendVideoStreamCommand("9667163125");
        }
    }
}
