using System;
using System.Net.Http;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Net.Client;

using Morse.Service;

Console.Write("Address: ");
var address = Console.ReadLine();
Console.Write("Port: ");
var port = Console.ReadLine();
Console.Write("Speed in WPM: ");
var speed = int.Parse(Console.ReadLine());
Console.Write("FWPM: ");
var fspeed = int.Parse(Console.ReadLine());

// DANGER, WILL ROBINSON
// The code below disables certificate validation. Ok for local prototyping, don't use this in production.
var httpHandler = new HttpClientHandler();
httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
using var channel = GrpcChannel.ForAddress($"https://{address}:{port}", new GrpcChannelOptions { HttpHandler = httpHandler });
var client = new MorseTransmitter.MorseTransmitterClient(channel);

while (true)
{
    Console.Write("message: ");
    var message = Console.ReadLine();
    if (string.IsNullOrEmpty(message)) break;
    MorseReply reply = default;
    while (reply is null)
    {
        try
        {
            reply = await client.TransmitMessageAsync(
                              new MorseRequest { Message = message, Wpm = speed, Fwpm = fspeed }
                              );
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            Console.WriteLine("pi is busy, waiting...");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
    Console.WriteLine(
        reply.Ok ? "OK" : "ERROR"
        );
    Console.WriteLine(TimeSpan.FromTicks(reply.DurationInTicks));
    Console.WriteLine(reply.Message);
}   
