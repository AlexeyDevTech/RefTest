using Ref.Controllers.MainController;
using Ref.Controllers.VoltageSyncroController;
using Ref.Enums;

internal class Program
{
    private static async Task Main(string[] args)
    {

        var VC = new VoltageSyncroController()
        {
            Settings = new()
            {
                Request = "#LAB?",
                Response = "Voltage Regulator Synchronizer",
                BaudRate = 9600
            }
        };

        var main = new MEAController()
        {
            Settings = new()
            {
                Request = "#LAB?",
                Response = "AngstremLabController",
                BaudRate = 9600
            }
        };

        VC.Start();
        main.Start();

        main.SetChain(ChainState.ChainAuto);
        VC.SetChain(ChainState.ChainAuto);

        VC.GetDevice();
        VC.GetMode();
        VC.GetVoltage();

        main.SpeedFast();
        main.SpeedFast();
        main.ReadTrial();

        await Console.Out.WriteLineAsync("======Chain started======");

        var res = await main.ExecuteChain();

        string r = res ? "======Chain completed======" : "======Chain NOT completed======";
        await Console.Out.WriteLineAsync(r);

        await Console.Out.WriteLineAsync("======Chain started======");

        res = await VC.ExecuteChain();

        r = res ? "======Chain completed======" : "======Chain NOT completed======";
        await Console.Out.WriteLineAsync(r);

        Console.ReadLine();

    }
}