using Ref.Interfaces;

namespace Ref.BaseClasses
{
    public class ControllerData : IControllerData
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

}

