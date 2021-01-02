using Grpc.Core;

using Morse.Service;
using System.Threading.Tasks;
using PiTop.MakerArchitecture.Foundation;
using System.Threading;
using PiTop;
using PiTop.MakerArchitecture.Foundation.Components;
using System;
using System.Text;
using System.Diagnostics;

public class MorseService : MorseTransmitter.MorseTransmitterBase
{
    static object _lock = new object();
    readonly Led _led;
    readonly Buzzer _buzzer;

    public MorseService(PiTop4Board board)
    {
        FoundationPlate plate = board.GetOrCreateFoundationPlate();
        _led = plate.GetOrCreateLed(DigitalPort.D0);
        _buzzer = plate.GetOrCreateBuzzer(DigitalPort.D1);
    }

    public override async Task<MorseReply> TransmitMessage(MorseRequest request, ServerCallContext context)
    {
        var morse = request.Message.ToMorse();
        Stopwatch sw;

        var t = 1000 * 60d / 50d / request.Wpm;

        double tf;
        if (request.Fwpm > 0)
        {
            tf = (50d * request.Wpm - 31d * request.Fwpm) / (19d * request.Fwpm) * t;
        }
        else
        {
            tf = t;
        }

        var unit = TimeSpan.FromMilliseconds(t);
        var funit = TimeSpan.FromMilliseconds(tf);

        bool lockTaken = false;
        try
        {
            Monitor.TryEnter(_lock, TimeSpan.FromMilliseconds(1000), ref lockTaken);

            if (!lockTaken)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, "The morse transmitter is busy with the previous message"));
            }

            sw = Stopwatch.StartNew();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < morse.Length; i++)
            {
                var character = morse[i];
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return new MorseReply { Ok = false, Message = $"Interrupted. Sent so far: {sb}" };
                }

                sb.Append(character);
                switch (character)
                {
                    case '.':
                        if (i > 0 && morse[i-1] is '.' or '-')
                            Thread.Sleep(1 * unit);
                        Beep(unit);
                        break;
                    case '-':
                        if (i > 0 && morse[i - 1] is '.' or '-')
                            Thread.Sleep(1 * unit);
                        Beep(3*unit);
                        break;
                    case ' ':
                        Thread.Sleep(3 * funit);
                        break;
                    case '/':
                        Thread.Sleep(7 * funit);
                        break;
                }
            }
        }
        finally
        {
            if (lockTaken) Monitor.Exit(_lock);
        }

        return new MorseReply { Ok = true, Message = morse, DurationInTicks = sw.Elapsed.Ticks };
    }
    void Beep(TimeSpan duration)
    {
        _led.On();
        _buzzer.On();
        Thread.Sleep(duration);
        _led.Off();
        _buzzer.Off();
    }
}
