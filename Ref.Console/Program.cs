using Newtonsoft.Json;
using Ref.BaseClasses.Commands;
using Ref.Controllers.MainController;
using Ref.Controllers.VoltageSyncroController;
using Ref.Enums;

internal class Program
{
    private static async Task Main(string[] args)
    {

        List<CommandBase> commands = new List<CommandBase>()
        {
            new ReqResCommand("#LAB?", "AngstremLabController"),
            new StandardCommand("#ReadTrial")
        };
        
        var res = JsonConvert.SerializeObject(commands);

        await Console.Out.WriteLineAsync(res);

        var r = JsonConvert.DeserializeObject<List<object>>(res);

        var a = (ReqResCommand)r[0];
            


        //await Console.Out.WriteLineAsync(r);
        /*var VC = new VoltageSyncroController()
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
        Thread.Sleep(5);
        VC.GetMode();
        Thread.Sleep(5);
        VC.GetVoltage();
        Thread.Sleep(5);

        main.SpeedFast();
        Thread.Sleep(5);
        main.SpeedFast();
        Thread.Sleep(5);
        main.ReadTrial();

        await Console.Out.WriteLineAsync("======Chain started======");

        var res = await main.ExecuteChain();

        string r = res ? "======Chain completed======" : "======Chain NOT completed======";
        await Console.Out.WriteLineAsync(r);

        await Console.Out.WriteLineAsync("======Chain started======");

        res = await VC.ExecuteChain();

        r = res ? "======Chain completed======" : "======Chain NOT completed======";
        await Console.Out.WriteLineAsync(r);*/

        Console.ReadLine();

    }
}