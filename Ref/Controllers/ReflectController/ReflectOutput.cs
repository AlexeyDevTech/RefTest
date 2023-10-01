using System.ComponentModel;

namespace Ref.Controllers.ReflectController
{
    public struct ReflectOutput
    {
        public ushort[] reflectData;
        public int gs;
        public int Lehght;
        public Channels channel;

        public static bool operator ==(ReflectOutput left, ReflectOutput right)
        {
            return left.Equals(right);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator !=(ReflectOutput left, ReflectOutput right)
        {
            return !(left == right);
        }
    }
    public enum Channels : int
    {
        [Description("Канал L1")]
        Channel1 = 1,
        [Description("Канал L2")]
        Channel2 = 2,
        [Description("Канал L3")]
        Channel3 = 3,
        [Description("Канал ARC")]
        IDMChannel = 4,
        [Description("Канал Decay")]
        DecayChannel = 5,
        [Description("Канал ICE")]
        ICEChannel = 6,
        [Description("Канал по умолчанию")]
        ChannelDefault = 0,
    }
    public enum Mode : int
    {
        [Description("По умолчанию")]
        Default = 0,
        [Description("Импульсный")]
        Impulse = 1,
        [Description("ИДМ")]
        IDM = 2,
        [Description("Волна по току")]
        ICE = 3,
        [Description("Волна по напряжению")]
        Decay = 4
    }
    public enum Amplitude : int
    {
        [Description("30")]
        V30 = 30,
        [Description("60")]
        V60 = 60,
        [Description("90")]
        V90 = 90
    }
    public enum Resistance : int
    {
        [Description("10 Ом")]
        Ohm_10 = 10,
        [Description("11 Ом")]
        Ohm_11 = 11,
        [Description("12 Ом")]
        Ohm_12 = 12,
        [Description("13 Ом")]
        Ohm_13 = 13,
        [Description("14 Ом")]
        Ohm_14 = 14,
        [Description("15 Ом")]
        Ohm_15 = 15,
        [Description("16 Ом")]
        Ohm_16 = 16,
        [Description("17 Ом")]
        Ohm_17 = 17,
        [Description("18 Ом")]
        Ohm_18 = 18,
        [Description("19 Ом")]
        Ohm_19 = 19,
        [Description("20 Ом")]
        Ohm_20 = 20,
        [Description("21 Ом")]
        Ohm_21 = 21,
        [Description("22 Ом")]
        Ohm_22 = 22,
        [Description("23 Ом")]
        Ohm_23 = 23,
        [Description("24 Ом")]
        Ohm_24 = 24,
        [Description("25 Ом")]
        Ohm_25 = 25,
        [Description("26 Ом")]
        Ohm_26 = 26,
        [Description("27 Ом")]
        Ohm_27 = 27,
        [Description("28 Ом")]
        Ohm_28 = 28,
        [Description("29 Ом")]
        Ohm_29 = 29,
        [Description("30 Ом")]
        Ohm_30 = 30,
        [Description("32 Ом")]
        Ohm_32 = 32,
        [Description("33 Ом")]
        Ohm_33 = 33,
        [Description("38 Ом")]
        Ohm_38 = 38,
        [Description("40 Ом")]
        Ohm_40 = 40,
        [Description("43 Ом")]
        Ohm_43 = 43,
        [Description("46 Ом")]
        Ohm_46 = 46,
        [Description("50 Ом")]
        Ohm_50 = 50,
        [Description("60 Ом")]
        Ohm_60 = 60,
        [Description("67 Ом")]
        Ohm_67 = 67,
        [Description("75 Ом")]
        Ohm_75 = 75,
        [Description("86 Ом")]
        Ohm_86 = 86,
        [Description("100 Ом")]
        Ohm_100 = 100,
        [Description("120 Ом")]
        Ohm_120 = 120,
        [Description("150 Ом")]
        Ohm_150 = 150,
        [Description("200 Ом")]
        Ohm_200 = 200,
        [Description("300 Ом")]
        Ohm_300 = 300,
        [Description("600 Ом")]
        Ohm_600 = 600,
    }
}

