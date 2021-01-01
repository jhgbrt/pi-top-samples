using System;
using System.Net.Http;

using Grpc.Net.Client;

using Morse.Service;

Console.Write("Address: ");
var address = Console.ReadLine();
Console.Write("Port: ");
var port = Console.ReadLine();

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
    var reply = await client.TransmitMessageAsync(
                      new MorseRequest { Message = message }
                      );
    Console.WriteLine(
        reply.Ok ? "OK" : "ERROR"
        );
    Console.WriteLine(reply.Message);
}   
