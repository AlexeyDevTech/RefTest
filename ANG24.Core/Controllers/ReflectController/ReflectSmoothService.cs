using ANG24.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANG24.Core.Controllers.ReflectController
{
    public class ReflectSmoothService : IReflectSmoothService
    {
        /// <summary>
        /// Adjust this as you see fit to suit the scenario
        /// </summary>
        private const int MaximumWindowSize = 150;
        private readonly object SyncRoot = new object();
        private int _windowSize;
        private readonly Dictionary<int, QueueExt<SmoothDataModel>> Buffers;
        //private QueueExt<SmoothDataModel> Buffer;
        private readonly ushort[] resArr;
        private readonly int MinLenght;
        private bool _enabled;

        public int WindowSize
        {
            get => _windowSize;
            set
            {
                if (value < MaximumWindowSize)
                    _windowSize = value;
                else _windowSize = MaximumWindowSize;
            }
        }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled && _enabled != value)
                {
                    Clear();
                }
                _enabled = value;
            }
        }
        public ReflectSmoothService()
        {
            WindowSize = 10;
            MinLenght = 65536;
            //Buffer = new QueueExt<SmoothDataModel>();
            var buf1 = new QueueExt<SmoothDataModel>();
            buf1.CollectionChanged += Buffer_CollectionChanged;
            var buf2 = new QueueExt<SmoothDataModel>();
            buf2.CollectionChanged += Buffer_CollectionChanged;
            var buf3 = new QueueExt<SmoothDataModel>();
            buf3.CollectionChanged += Buffer_CollectionChanged;
            Buffers = new Dictionary<int, QueueExt<SmoothDataModel>>()
            {
                { 1, buf1 },
                { 2, buf2 },
                { 3, buf3 }
            };
            resArr = new ushort[MinLenght];
            //Buffer.CollectionChanged += Buffer_CollectionChanged;
        }
        public ushort[] AddAndGetResult(ushort[] data, int channel)
        {
            lock (SyncRoot)
            {
                var el = Buffers[channel];
                try
                {

                    el.Enqueue(new SmoothDataModel
                    {
                        Data = data,
                        len = data.Length,
                        result = new ushort[data.Length]
                    });
                    Calc(el);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Error to SmoothService");
                }
                return el.result;
            }

        }
        private void Calc(QueueExt<SmoothDataModel> el)
        {
            lock (SyncRoot)
            {
                try
                {
                    int Size = el.Count;
                    if (Size < WindowSize)
                    {
                        CycleCalc(el);
                    }
                    else
                    {
                        while (el.Count > WindowSize)
                            _ = el.Dequeue();
                        CycleCalc(el);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: {1}", ex, ex.Message);
                }
            }
        }

        private void CycleCalc(QueueExt<SmoothDataModel> el)
        {
            int Size = el.Count;
            for (int i = 0; i < el.MinValue; i++)
            {
                double Sum = 0.0;
                foreach (var item in el) Sum += item.Data[i];
                el.result[i] = (ushort)(Sum / Size);
            }
        }
        public void Clear()
        {
            lock (SyncRoot) foreach (var item in Buffers.Values) item.Clear();
        }
        public ushort[] GetResult()
        {
            return resArr ?? new ushort[1];
        }
        private void Buffer_CollectionChanged(object sender, CollectionChangedType type)
        {
            try
            {
                var buffer = (sender as QueueExt<SmoothDataModel>);
                lock (SyncRoot)
                {
                    buffer.MinValue = buffer.Select(x => x.len).Min();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex, ex.Message);
            }
        }
    }

    internal class SmoothDataModel
    {
        internal ushort[] Data;
        internal int len;
        internal ushort[] result;
    }

    internal enum CollectionChangedType
    {
        Add,
        Remove
    }

    internal delegate void Queue_CollectionChanged(object sender, CollectionChangedType type);

    /// <summary>
    /// Представляет коллекцию типа <see cref="Queue{T}"/> с возможностью оповещения об обновлении коллекции через события.
    /// </summary>
    /// <typeparam name="T">Предлагаемый тип коллекции</typeparam>
    internal class QueueExt<T> : Queue<T>
    {
        internal event Queue_CollectionChanged CollectionChanged;
        public QueueExt() : base()
        {
            result = new ushort[65536];
        }
        public int MinValue { get; set; }
        public ushort[] result { get; set; }
        //public Func<int> MinValue { get; set; }
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            //MinValue?.Invoke();
            CollectionChanged?.Invoke(this, CollectionChangedType.Add);
        }
        public new T Dequeue()
        {
            var res = base.Dequeue();
            //MinValue?.Invoke();
            CollectionChanged?.Invoke(this, CollectionChangedType.Remove);
            return res;
        }
    }
}
