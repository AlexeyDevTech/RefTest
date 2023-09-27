using Ref;

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
            }
        };
        if (main.Start())
        {
            Thread.Sleep(10000);

            main.Stop();
        }
    }
}