using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseUrls("https://pi-top:5001").UseStartup<Startup>();
    })
    .Build().Run();

public class Startup
{
    public void ConfigureServices(IServiceCollection services) => services.AddGrpc();
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MorseService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
    }
}