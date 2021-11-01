using PiTop.MakerArchitecture.Foundation;
using PiTop.MakerArchitecture.Foundation.Sensors;
using PiTop.MakerArchitecture.Foundation.Components;
using PiTop.MakerArchitecture.Expansion;
using PiTop.MakerArchitecture.Expansion.Sensors;
using PiTop;
using System.Threading.Tasks;
using UnitsNet;
using System;
using System.Threading;

var rover = new Rover();
Console.ReadKey();
rover.Stop();


class Rover
{
    PiTop4Board board;
    FoundationPlate plate;
    ExpansionPlate expansion;

    Led frontLeftLed;
    Led frontRightLed;
    Led backLeftLed;
    Led backRightLed;
    Led[] leds;
    private Buzzer buzzer;
    private UltrasonicSensor frontdistanceSensor;
    private UltrasonicSensor backdistanceSensor;
    EncoderMotor leftMotor;
    EncoderMotor rightMotor;

    public Rover()
    {
        board = PiTop4Board.Instance;
        plate = board.GetOrCreateFoundationPlate();
        expansion = board.GetOrCreateExpansionPlate();
        board.SelectButton.Pressed += async (s, e) => await Start();
        board.CancelButton.Pressed += (s,e) => Stop();
        backRightLed = plate.GetOrCreateLed(DigitalPort.D0);
        frontRightLed = plate.GetOrCreateLed(DigitalPort.D2);
        frontLeftLed = plate.GetOrCreateLed(DigitalPort.D5);
        backLeftLed = plate.GetOrCreateLed(DigitalPort.D7);

        buzzer = plate.GetOrCreateBuzzer(DigitalPort.D1);
        frontdistanceSensor = plate.GetOrCreateUltrasonicSensor(DigitalPort.D3);
        backdistanceSensor = plate.GetOrCreateUltrasonicSensor(DigitalPort.D6);
        leftMotor = expansion.GetOrCreateEncoderMotor(EncoderMotorPort.M1);
        rightMotor = expansion.GetOrCreateEncoderMotor(EncoderMotorPort.M4);
        leftMotor.ForwardDirection = ForwardDirection.CounterClockwise;
        rightMotor.ForwardDirection = ForwardDirection.Clockwise;

        leds = new[] { backLeftLed, frontLeftLed, frontRightLed, backRightLed };
    }

    public Speed Speed { get; set; } = Speed.FromCentimetersPerSecond(10);

    public async Task Start()
    {
        try
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var led in leds)
                    led.On();
                await Task.Delay(100);
                foreach (var led in leds)
                    led.Off();
                await Task.Delay(100);
                await Beep(TimeSpan.FromMilliseconds(100));
            }


            while (true)
            {
                try
                {
                    Forward();

                    while (frontdistanceSensor.Distance > Length.FromCentimeters(10))
                    {
                        await Task.Delay(100);
                    }

                    var s1 = new CancellationTokenSource();
                    var blinkingLeft = BlinkLeft(s1.Token);
                    TurnLeft();

                    while (frontdistanceSensor.Distance < Length.FromCentimeters(50))
                    {
                        await Task.Delay(100);
                    }
                    s1.Cancel();
                    Stop();
                    await blinkingLeft;

                    Forward();

                    while (frontdistanceSensor.Distance > Length.FromCentimeters(10))
                    {
                        await Task.Delay(100);
                    }

                    var s2 = new CancellationTokenSource();
                    var blinkingRight = BlinkRight(s2.Token);
                    TurnRight();

                    while (frontdistanceSensor.Distance < Length.FromCentimeters(50))
                    {
                        await Task.Delay(100);
                    }
                    s2.Cancel();
                    Stop();
                    await blinkingRight;
                }
                catch (SensorReadException)
                {

                }
            }

        }
        finally
        {
            await Beep(TimeSpan.FromMilliseconds(500));
            Stop();
        }

    }
    public void Stop()
    {
        leftMotor.Stop();
        rightMotor.Stop();
        buzzer.Off();
        foreach (var led in leds) led.Off();
    }

    private void Forward()
    {
        rightMotor.SetSpeed(Speed);
        leftMotor.SetSpeed(Speed);
        foreach (var led in leds) led.On();
        buzzer.Off();
    }
    private void Backward()
    {
        rightMotor.SetSpeed(-Speed);
        leftMotor.SetSpeed(-Speed);
    }

    private void TurnLeft()
    {
        var rspeed = rightMotor.Rpm;
        rightMotor.Rpm = -rspeed;
    }
    private Task BlinkLeft(CancellationToken token) => Blink(new[] { backLeftLed, frontLeftLed }, token);
    private Task BlinkRight(CancellationToken token) => Blink(new[] { backRightLed, frontRightLed }, token);

    private async Task Blink(Led[] leds, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var led in leds)
                led.Off();
            var beeping = Beep(TimeSpan.FromMilliseconds(300));
            await Task.Delay(500, token);
            foreach (var led in leds)
                led.On();
            await beeping;
            await Task.Delay(500, token);
        }
    }
    public void TurnRight()
    {
        var lspeed = leftMotor.Rpm;
        leftMotor.Rpm = -lspeed;
    }

    public async Task Beep(TimeSpan time)
    {
        buzzer.On();
        await Task.Delay(time);
        buzzer.Off();
    }

}