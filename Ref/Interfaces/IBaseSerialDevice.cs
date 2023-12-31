﻿using System.IO.Ports;

namespace Ref.Interfaces
{
    public interface IBaseSerialDevice : IBaseDevice
    {
        SerialPort? port { get; }
        bool IsFinded { get; set; }
        void SetPort(SerialPort? port);
        void SetPort(string portName, int BaudRate = 9600);


    }

}

