using JT808.Protocol.Extensions.JT1078.MessageBody;
using JT808.Protocol.MessageBody;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Enums;
using Microsoft.Extensions.DependencyInjection;

public class JT1078VideoTriggerService
{
    private readonly IJT808SessionService sessionService;
    private readonly IJT808UnificationSendService sendService;

    public JT1078VideoTriggerService(
        IJT808SessionService sessionService,
        IJT808UnificationSendService sendService)
    {
        this.sessionService = sessionService;
        this.sendService = sendService;
    }

    public async Task SendVideoStreamCommand(string terminalPhoneNo)
    {
        if (!sessionService.TryGetSession(terminalPhoneNo, out var session))
        {
            Console.WriteLine($"Device {terminalPhoneNo} is not online.");
            return;
        }

        var videoCmd = new JT808_0x9101
        {
            ChannelNo = 1, // Logical video channel
            AVItemType = 0, // 0: audio + video
            StreamType = 1, // 1: main stream
            ServerIPAddress = "148.66.155.196", // JT1078 video server
            ServerIPAddressLength = (byte)"148.66.155.196".Length,
            ServerTcpPort = 1078,  // JT1078 TCP
            ServerUdpPort = 1078,  // JT1078 UDP
            LogicalChannelNo = 1
        };

        await sendService.SendAsync(terminalPhoneNo, new JT808Package
        {
            MsgId = JT808MsgId._0x9101,
            Bodies = videoCmd
        });
        
        Console.WriteLine("Sent 0x9101 to " + terminalPhoneNo);
    }
}
