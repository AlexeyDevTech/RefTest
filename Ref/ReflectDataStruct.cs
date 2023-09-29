using System.Runtime.InteropServices;

namespace Ref
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ReflectDataStruct
    {
        public int lengthArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65536)]
        public ushort[] IntArr;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(ReflectDataStruct left, ReflectDataStruct right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(ReflectDataStruct left, ReflectDataStruct right)
        {
            return !(left == right);
        }
    }
   
}

