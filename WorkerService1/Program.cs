using System.Globalization;
using WorkerService1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

host.WaitForShutdownAsync();

var cancellationTokenSource = new CancellationTokenSource();
var token = cancellationTokenSource.Token;

var threadTwo = new Thread(o =>
{
    Task.Run(async delegate
    {
        await Task.Delay(5000);
        cancellationTokenSource.Cancel();
    });
});

threadTwo.Start();

var threadOne = new Thread(() =>
{
    Task.Run(async delegate
    {
        while (!token.IsCancellationRequested)
        {
            Console.WriteLine(1);
        }
        // await Task.Delay(2000);
        Console.WriteLine(2);
        await host.StopAsync();
    });
   
});
threadOne.Start();

host.Run();
