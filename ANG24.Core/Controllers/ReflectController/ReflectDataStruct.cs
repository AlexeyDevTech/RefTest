using System.Runtime.InteropServices;

namespace ANG24.Core.Controllers.ReflectController
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ReflectDataStruct
    {
        public int lengthArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65536)]
        public ushort[] IntArr;
        public int GS;
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public static bool operator ==(ReflectDataStruct left, ReflectDataStruct right) => left.Equals(right);
        public static bool operator !=(ReflectDataStruct left, ReflectDataStruct right) => !(left == right);
    }

}

