namespace ANG24.Core.Controllers.ReflectController
{
    public static class CalculatorDDS
    {
        private enum TauUnitMetric : int
        {
            ns = 0,
            us = 3,
            ms = 6,
            s = 9
        }
        private struct TauMetricValue
        {
            public TauUnitMetric Unit;
            public double value;
            public TauMetricValue(double val)
            {
                var counter = 0;
                var res = val * Math.Pow(10, 9);
                while (res > 1000 && counter < 9)
                {
                    res /= 1000;
                    counter += 3;
                }
                Unit = (TauUnitMetric)counter;
                value = res;
            }
            public double TranslateTo(TauUnitMetric unit)
            {
                var delta = Unit - unit;
                return value * Math.Pow(10, delta);
            }
        }

        //входные данные 
        public static double F // частота OSC
=> OSCManager.Instance.StandardSR;
        public static double P
        {
            get
            {
                var freqD = F;
                var len = (int)(L * 2 * K * freqD / (3 * Math.Pow(10, 8)));
                if (len == 0 || len > 65535)
                {
                    len = 65535;
                }
                return len;
            }
        }// количество видимых выборок
        //=> OSCManager.Instance.LenMass;
        public static double N // pWave значение
=> OSCManager.Instance.PWave;
        public static double Z  // pPeriod значение
=> OSCManager.Instance.PPeriod;
        public static double DZI // DZI значение
=> OSCManager.Instance.PulseLen;
        public static double K // коэффициент укорочения
=> OSCManager.Instance.K;
        public static double L //длина обзора
        {
            get
            {
                //так как при L < 1000 портится математика работы ИДМ принято решение записать L не меньше 1000
                var l = OSCManager.Instance.Lenght;
                if (l < 1000) return 1000;
                else return l;
            }
        }

        private static double RS => N / Z;
        private static double T => 1 / F;
        private static double TauDSO => T * P;
        private static TauMetricValue TauMetricDSO => new TauMetricValue(TauDSO);
        private static TauMetricValue TauDDS
        {
            get
            {
                var tau = TauMetricDSO;
                tau.value += 1;
                return tau;
            }
        }
        public static double FDDS => 1 / TauDDS.TranslateTo(TauUnitMetric.s);
        private static double T1 => TauDDS.TranslateTo(TauUnitMetric.s) / RS;
        public static double M => DZI / Math.Pow(10, 9) / T1 < 1 ? 1 : DZI / Math.Pow(10, 9) / T1;

    }

}

