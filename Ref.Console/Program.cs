using Ref;
using Ref.Controllers;

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

        var MO = new MOController()
        {
            Settings = new()
            {
                Request = "#Get_Packege",
                Response = "MO",
                BaudRate = 9600
            }
        };

        main.Start();
        MO.Start();

        for(int i = 0 ; i < 50; i++)
        {
            MO.SetCommand("#Get_Packege");
            Thread.Sleep(100);
        }

        Console.ReadLine();

        MO.Stop();

        /*if (main.Start())
        {
            Thread.Sleep(3000);

            main.Stop();
        }*/
    }
}