using ANG24.Core.Interfaces;

namespace ANG24.Core.BaseClasses
{
    public class ControllerData : IControllerData
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

}

