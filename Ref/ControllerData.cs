namespace Ref
{
    public class ControllerData : IControllerData
    { 
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
    public interface IControllerData
    {
        string Message { get; set; }
        bool IsSuccess { get; set; }
    }

}

