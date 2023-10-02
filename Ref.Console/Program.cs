using Ref;
using Ref.Controllers;
using Ref.Controllers.MainController;

internal class Program
{
    private static void Main(string[] args)
    {
        var main = new MEAController()
        {
            Settings = new()
            {
                Request = "#LAB?",
                Response = "AngstremLabController",
                BaudRate = 9600
            }
        };
        main.Start();

        Console.ReadLine();

        /*if (main.Start())
        {
            Thread.Sleep(3000);

            main.Stop();
        }*/
    }
}