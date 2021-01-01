using Grpc.Core;

using Morse.Service;
using System.Threading.Tasks;
using PiTop.MakerArchitecture.Foundation;

public class MorseService : MorseTransmitter.MorseTransmitterBase
{

    public override async Task<MorseReply> TransmitMessage(MorseRequest request, ServerCallContext context)
    {
        var morse = request.Message.ToMorse();

        var board = PiTop.PiTop4Board.Instance;
        var plate = board.GetOrCreateFoundationPlate();
        var led = plate.GetOrCreateLed(DigitalPort.D0);
        var buzzer = plate.GetOrCreateBuzzer(DigitalPort.D1);

        var unit = 150;
        foreach (var character in morse)
        {
            if (character == "/") // word boundary
            {
                await Task.Delay(7 * unit);
            }
            else foreach (var symbol in character)
                {
                    var delay = symbol switch
                    {
                        '.' => 1,
                        '-' => 3,
                    };
                    led.On();
                    buzzer.On();
                    await Task.Delay(delay * unit);
                    led.Off();
                    buzzer.Off();
                    await Task.Delay(1 * unit);
                }
        }

        return new MorseReply { Ok = true, Message = string.Join(" ", morse) };
    }
}
