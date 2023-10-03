using Ref;
using Ref.Controllers;
using Ref.Controllers.MainController;
using Ref.Controllers.VoltageSyncroController;

internal class Program
{
    private static void Main(string[] args)
    {
        var voltageSyncroOperator = new VoltageSyncroController()
        {
            Settings = new()
            {
                Request = "#LAB?",
                Response = "Voltage Regulator Synchronizer",
                BaudRate = 9600
            }
        };
        voltageSyncroOperator.Start();

        Thread.Sleep(5000);

        voltageSyncroOperator.GetMode();


        Console.ReadLine();

    }
}