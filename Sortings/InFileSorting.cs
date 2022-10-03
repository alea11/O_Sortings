using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sortings
{
    public abstract class InFileSorting
    {
        public event EventHandler<ProgressEventArgs> OnProgress;

        protected string _file_0;
        protected int _numSize;
        protected int _blockSize; // размер блока , части массива чисел - для операций ввода-вывода 
        protected int N;

        protected string _name;
        public string ErrMessage = null;

        public InFileSorting(string file, int numSize, int blockSize) 
        {
            _file_0 = file;
            _numSize = numSize;
            _blockSize = blockSize;
        }

        public static void CreateFile(string file, int numSize, int blockSize, int size, Sorting.ePresort presort, int range)// создаем (или пересоздаем) и заполняем
        {
            using (var fs = File.Create(file)) { }
            

            Saver saver = new Saver(file, numSize, blockSize);

            if(presort == Sorting.ePresort.Random)
            {
                Random random = new Random(12345);
                for (int i = 0; i < size; i++)
                {
                    int num = random.Next(range);
                    saver.Put(num);
                }
            }
            else
            {
                float k = (float)range / size;

                if (presort == Sorting.ePresort.Sorted)
                {
                    for (int i = 0; i < size; i++)
                        saver.Put((int)(i * k));
                }
                else
                {
                    for (int i = 0; i < size; i++)
                        saver.Put((int)((size - 1 - i) * k));
                }
            }
            
            saver.SaveTail();

        }

        public abstract string Sort(); // возвращаемое значение - имя выходного файла
                

        internal static int[] ReadBlock(string file, int numSize, ref long poz, ref int count)
        {
            // numSize - размер числа в байтах
            byte[] bytes = new byte[count * numSize];

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(poz, SeekOrigin.Begin);
                int res = fs.Read(bytes, 0, count * numSize);
                poz += res;

                count = res / numSize;
            }

            int[] arr = new int[count];
            for (int i = 0; i < count; i++)
            {
                // формирование числа из нескольких байт. сначала(слева) - старшие байты
                for (int w = 0; w < numSize; w++)
                {
                    arr[i] += bytes[i * numSize + w] << (8 * (numSize - w - 1));
                }
            }

            return arr;
        }

        internal static int[] ReadFixBlock(string file, int numSize, long poz, int count)
        {
            // numSize - размер числа в байтах
            byte[] bytes = new byte[count * numSize];

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(poz, SeekOrigin.Begin);
                int res = fs.Read(bytes, 0, count * numSize);               
            }

            int[] arr = new int[count];
            for (int i = 0; i < count; i++)
            {
                // формирование числа из нескольких байт. сначала(слева) - старшие байты
                for (int w = 0; w < numSize; w++)
                {
                    arr[i] += bytes[i * numSize + w] << (8 * (numSize - w - 1));
                }
            }

            return arr;
        }

        internal static void WriteBlock(int[] arr, string file, int numSize, ref long poz, int count)
        {
            byte[] bytes = new byte[count * numSize];
            for (int i = 0; i < count; i++)
            {
                // разложение числа на байты. сначала(слева) - старшие байты
                for (int w = 0; w < numSize; w++)
                {
                    bytes[i * numSize + w] = (byte)(arr[i] >> (8 * (numSize - w - 1)));
                }

            }

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Write))
            {
                fs.Seek(poz, SeekOrigin.Begin);
                fs.Write(bytes, 0, count * numSize);
            }

            poz += count * numSize;
        }

        internal static void WriteFixBlock(int[] arr, string file, int numSize, long poz)
        {
            int count = arr.Length;
            byte[] bytes = new byte[count * numSize];
            for (int i = 0; i < count; i++)
            {
                // разложение числа на байты. сначала(слева) - старшие байты
                for (int w = 0; w < numSize; w++)
                {
                    bytes[i * numSize + w] = (byte)(arr[i] >> (8 * (numSize - w - 1)));
                }
            }

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Write))
            {
                fs.Seek(poz, SeekOrigin.Begin);
                fs.Write(bytes, 0, count * numSize);
            }            
        }

        internal static void WriteNum(int num, FileStream fs, int numSize, int idx)
        {
            byte[] bytes = new byte[numSize];
            
            // разложение числа на байты. сначала(слева) - старшие байты
            for (int w = 0; w < numSize; w++)
            {
                bytes[w] = (byte)(num >> (8 * (numSize - w - 1)));
            }

            fs.Seek(idx * numSize, SeekOrigin.Begin);
            fs.Write(bytes, 0, numSize);
        }

        protected void RaiseOnProgress(string text)
        {
            if (OnProgress != null)
                OnProgress.Invoke(this, new ProgressEventArgs(text));
        }


        class Saver
        {
            private int[] _arrOut; //выходной массив
            private int _idxOut = 0; // позиция в выходном массиве

            private string _file;
            private int _blockSize;
            private int _numSize;


            private long _writePoz;


            public Saver(string file, int numSize, int blockSize)
            {
                _file = file;
                _writePoz = 0;
                _blockSize = blockSize;
                _numSize = numSize;
                _arrOut = new int[blockSize];
            }

            public void Put(int value)
            {
                // если дошли до конца выходного массива, то записываем массив
                if (_idxOut == _blockSize)
                {
                    WriteBlock(_arrOut, _file, _numSize, ref _writePoz, _idxOut);
                    _arrOut = new int[_blockSize];
                    _idxOut = 0;
                }

                // занесение очередного значения в выходной массив
                _arrOut[_idxOut++] = value;
            }

            public void SaveTail() // записываем последний блок
            {
                WriteBlock(_arrOut, _file, _numSize, ref _writePoz, _idxOut);
            }           

        }


    }
}
