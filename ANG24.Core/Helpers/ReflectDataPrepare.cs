using ANG24.Core.Controllers.ReflectController;
//using ANG24.Sys.Application.Types.CommunicationControllerTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartPoint = ANG24.Core.Controllers.ReflectController.ChartPoint;

namespace ANG24.Core.Helpers
{

    public sealed class ReflectDataPrepare
    {

        private int length;
        public int XOffset { get; set; } = 0;
        public Channels CurrentChannel { get; set; }
        public Channels ComparedChannel1 { get; set; }
        public Channels ComparedChannel2 { get; set; }
        public bool IsCompare { get; set; }
        public IList<Channels> _Channels { get; set; }
        public byte Mode2Stage { get; set; }
        public int Length
        {
            get => length;
            set => length = value < 50_000 ? value : 50_000;
            // Почему 50_000? Потому что иди ..., вот почему
            // возможно надо будет убрать
        }
        public int LenMas { get; set; }
        private bool isSmootherEnabled = false;
        // при желании можно изменить число отправляемых точек тут
        // не изменять в ходе работы программы
        private const ushort pointsCount = 1755;
        private readonly ushort[,] channelsData = new ushort[8, pointsCount]; // последние значения с каналов, подготовленные к выводу
        private readonly Filter<int> smoother = Filter<int>.NewFilter(5);

        /// <summary>
        /// Собрать данные всех текущих каналов
        /// </summary>
        /// <param name="array">массив текущего канала</param>
        /// <param name="isDataUpdated">Проверочная переменная, получилось ли заполнить массив</param>
        /// <returns></returns>
        public ChartPoint[] GetChannelsData(ushort[] array, out bool isDataUpdated)
        {
            #region если ИДМ
            if (CurrentChannel != Channels.IDMChannel)
                isDataUpdated = true;
            else
                isDataUpdated = false;
            var mode = Mode2Stage;
            #endregion

            #region Приведение к числу точек для графики
            // уменьшение числа значений, фильтрация и прочее
            ReduceData(ref array, length, pointsCount); //lenght = LenMass
            #endregion

            #region Сглаживание
            // включить сглаживающий фильтр
            if (isSmootherEnabled) Smoother(ref array);
            #endregion

            #region Создание массива подготовленных данных 
            //заполнение хранилища данных
            if (CurrentChannel != 0 && array[5] != 0)
            {
                Parallel.For(0, pointsCount,
                   (i) =>
                   {
                       switch (CurrentChannel)
                       {
                           case Channels.Channel1:
                           case Channels.Channel2:
                           case Channels.Channel3:
                               channelsData[(byte)CurrentChannel - 1, i] = array[i];
                               break;
                           case Channels.IDMChannel:
                               if (mode == 1)
                                   channelsData[3, i] = array[i];
                               else
                                   channelsData[4, i] = array[i];
                               break;
                           case Channels.DecayChannel:
                           case Channels.ICEChannel:
                               channelsData[(byte)CurrentChannel, i] = array[i];
                               break;
                           /*case Channels.FileChannel:
                               channelsData[7, i] = array[i];
                               break;*/
                       }
                   });
                if (Mode2Stage == 2)
                {
                    Mode2Stage = 0;
                    isDataUpdated = true;
                }
            }
            #endregion


            //заполнение структуры для отправки в API
            var newData = new ChartPoint[pointsCount];
            var xStep = (double)LenMas / pointsCount; //LenMass = lenght
            Parallel.For(0, pointsCount, channelsFill);
            if (IsCompare) Parallel.For(0, pointsCount, MatchChannelFill);
            return newData;
            void channelsFill(int i)
            {
                // заполнение списка в случае наличия данного канала в списке каналов
                if (i < XOffset)
                    newData[i].X = (int)-(xStep * (XOffset - i));
                else
                    newData[i].X = (int)(xStep * (i - XOffset));
                foreach (var channel in _Channels)
                    switch (channel)
                    {
                        case Channels.Channel1:
                            newData[i].Channel1 = channelsData[(byte)channel - 1, i];
                            break;
                        case Channels.Channel2:
                            newData[i].Channel2 = channelsData[(byte)channel - 1, i];
                            break;
                        case Channels.Channel3:
                            newData[i].Channel3 = channelsData[(byte)channel - 1, i];
                            break;
                        case Channels.IDMChannel:
                            newData[i].Channel41 = channelsData[(byte)channel - 1, i];
                            newData[i].Channel42 = channelsData[(byte)channel, i];
                            break;
                        case Channels.DecayChannel:
                            newData[i].Channel5 = channelsData[(byte)channel, i];
                            break;
                        case Channels.ICEChannel:
                            newData[i].Channel6 = channelsData[(byte)channel, i];
                            break;
                        /*case Channels.FileChannel:
                            newData[i].ChannelFile = channelsData[(byte)channel, i];
                            break;*/
                    }
            }
            void MatchChannelFill(int i)
            {
                checked
                {
                    var y = (ushort)(channelsData[(byte)ComparedChannel1 - 1, i] + 128 - channelsData[(byte)ComparedChannel2 - 1, i]);
                    newData[i].MatchedChannel = y;
                }
            }
        }
        private void ReduceData<T>(ref T[] array, int max, int pointsCount)
        {
            // возможно придется обрезать массив до 50_000 точек тут
            var xD = (double)max / pointsCount; // берём переодичность считывания данных
            double j = 0;
            for (int i = 0; i < pointsCount && j < array.Length; i++, j += xD)
                array[i] = array[(int)j];
            Array.Resize(ref array, pointsCount); // уменьшаем до максимума для текущих настроек
        }
        private void Smoother(ref ushort[] array)
        { // фильтр
            for (int i = 0; i < pointsCount; i++)
            {
                smoother.AVG = array[i];
                array[i] = (ushort)smoother.AVG;
            }
        }
        // переключатель сглаживающего фильтра
        public void TurnSmoother() => isSmootherEnabled = !isSmootherEnabled;
    }

}
