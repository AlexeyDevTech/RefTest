using Ref;

internal class Program
{
    private static void Main(string[] args)
    {
        var helper = new DeviceConnectHelper();
        
        
        var device = helper.FindPort("R120#", "R120_OK", 115200);

        Console.WriteLine($"device: {device?.port?.PortName ?? "<NULL>"}");
    }
}