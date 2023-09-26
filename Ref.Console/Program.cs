using Ref;

internal class Program
{
    private static void Main(string[] args)
    {
        var refl = new ReflectController();
        refl.Start();
        Thread.Sleep(5000);
        refl.Stop();
    }
}