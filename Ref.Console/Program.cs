using Ref;

internal class Program
{
    private static void Main(string[] args)
    {
        var main = new MEAController()
        {
            Settings = new()
            {
                Request = "R120#",
                Response = "R120_OK",
                BaudRate = 115200
            }
        };
        if (main.Start())
        {
            Thread.Sleep(1000);

            main.Stop();
        }
    }
}