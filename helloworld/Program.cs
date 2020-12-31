using System;
using PiTop;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;

var module = PiTop4Board.Instance;
var display = module.Display;

var message = "Hello from .NET 5";

var font = SystemFonts.Collection.Find("Roboto").CreateFont(16);

module.Display.Draw((context,cr) => {
                context.Clear(Color.Black);
                var rect = TextMeasurer.Measure(message, new RendererOptions(font));
                var x = (cr.Width - rect.Width) / 2;
                var y = (cr.Height + rect.Height) / 2;
                context.DrawText(message, font, Color.White, new PointF(0, 0));
            });

Console.WriteLine("Press enter to exit");
Console.ReadLine();
