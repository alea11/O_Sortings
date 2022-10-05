using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class ExternalMergeSort  : InFileSorting
    {
        // внешняя сортировка слиянием        

        // 5 файлов: 
        // file_0 - файл с исходным массивом, подлежащим сортировке
        // file_1, file_2 и file_3, file_4 - две пары вспомогательных файлов. результирующий массив в итоге будет в одном из них
        
        private string _file_1;
        private string _file_2;
        private string _file_3;
        private string _file_4;

        // 
        private string _file_Inp1;
        private string _file_Inp2;
        private string _file_Out1;
        private string _file_Out2;

        private int _mergers = 0; // к-во слияний

        int _range;


        public ExternalMergeSort( string file, int numSize, int blockSize, int range) : base(file, numSize, blockSize)
        { 
            _name = "ExternalMergeSort";
            _range = range;
            
            _file_1 = "file_1";
            _file_2 = "file_2";
            _file_3 = "file_3";
            _file_4 = "file_4";

            _file_Inp1 = _file_1;
            _file_Inp2 = _file_2;
            _file_Out1 = _file_3;
            _file_Out2 = _file_4;            
        }

        

        public override  string Sort() // возвращаемое значение - имя выходного файла
        {
            using (var fs = File.Create(_file_Inp1)) { }
            using (var fs = File.Create(_file_Inp2)) { }

            // сначала последовательно считываем блоки данных, сортировка в каждом блоке, и складываем эти блоки поочередно в 2 файла
            SortBlocks();

            using (var fs = File.Create(_file_Out1)) { }
            using (var fs = File.Create(_file_Out2)) { }

            _mergers = 0;

            while (Merge() == false) 
            {
                // переключение промежуточных файлов: входные <-> выходные
                string i1 = _file_Inp1;
                string i2 = _file_Inp2;
                _file_Inp1 = _file_Out1;
                _file_Inp2 = _file_Out2;
                _file_Out1 = i1;
                _file_Out2 = i2;

                // очистка выходных файлов
                using (var fs = File.Create(_file_Out1)) { }
                using (var fs = File.Create(_file_Out2)) { }
            }

            return _file_Out1;
        }

        private void SortBlocks()
        {
            Sorting sorting = Sorting.CreateSorting(Sorting.eAlgorithm.RadixSort); //Sorting.eAlgorithm.HeapSort   .QuikSort_Hoar     .MergeSort
            long readPoz = 0;
            long len = new FileInfo(_file_0).Length;
            N = (int)(len / _numSize); //  к-во элементов

            bool parity = false;

            long writePoz_1 = 0;
            long writePoz_2 = 0;

            int nb = 0;
            int blocks = (N - 1) / _blockSize + 1; //т.е. округление вверх от N/_blockSize

            while (readPoz < len) 
            {
                int size = _blockSize;

                // читаем очередной блок, в аргументе size - возвращается реальное к-во прочитанных элементов (в конечном блоке может отличаться от заявленного)
                int[] arr = ReadBlock(_file_0, _numSize, ref readPoz, ref size);

                // каждый блок сортируется индивидуально одним из эффективных алгоритмов, напр. QuikSort
                sorting.Sort(arr, _range);

                // и записываем отсортированный блок поочередно в один из входных(для следующих операций) файлов
                if(parity == false)
                {
                    WriteBlock(arr, _file_Inp1, _numSize, ref writePoz_1, size);
                }
                else
                {
                    WriteBlock(arr, _file_Inp2, _numSize, ref writePoz_2, size);
                }

                parity = !parity;
                              
                RaiseOnProgress($"sorted {++nb} blocks from {blocks}");
            }
        }

        private bool Merge()
        {
            int mergeBlockSize = _blockSize / 2; // работаем с 2-мя блоками в памяти, и т.к. размер блока - ограничен размером памяти, 
                                                 // то в этой функции оперирующей с 2-мя блоками - их размер задаем вдвое меньшим              

            EmsReader reader = new EmsReader(_file_Inp1, _file_Inp2, _numSize, mergeBlockSize);
            EmsSaver saver = new EmsSaver(_file_Out1, _file_Out2, _numSize, mergeBlockSize);

            bool finishReading = false;

            int baseValue = reader.GetNext(ref finishReading);
            reader.ChangeParity();            

            int valToSave = 0;

            while (true)
            {
                // определение очередного выводимого элемента
                
                int value = reader.GetNext(ref finishReading);

                if(value < valToSave && baseValue >= valToSave) // пришло значение меньшее, чем сохранялось на предыдущей иттерации
                {
                    valToSave = baseValue;
                    baseValue = value;
                    reader.ChangeParity();
                }
                else if (baseValue < valToSave && value >= valToSave) // базовое (полученное ранее) значение меньшее, чем сохранялось на предыдущей иттерации
                {
                    valToSave = value;
                }
                else // оба значения (и базовое, и текущее) - больше сохраненного на предыдущей иттерации
                {
                    if(baseValue < value)
                    {
                        valToSave = baseValue;
                        baseValue = value;
                        reader.ChangeParity();
                    }
                    else
                    {
                        valToSave = value;
                    }
                }

                saver.Put(valToSave);                

                // если обработаны оба входных файла - выходим
                if (finishReading)
                {
                    saver.Put(baseValue);
                    saver.SaveTail();
                    break;
                }
                    
            }

            _mergers++;

            RaiseOnProgress($"merged {_mergers} pairs of sequences.");
            return saver.isMerged; // если была запись во второй файл, т.е. было разделение собираемой последовательности  - это признак необходимости продолжения сортировки
        }

       
        public override string ToString()
        {
            return $"{_name}: Length: {N}, blockSize: {_blockSize}, mergers: {_mergers}";
        }



        private class EmsReader
        {
            private string _file1;
            private string _file2;
            private int _blockSize;
            private int _numSize;

            // размеры файлов
            long _len_1;
            long _len_2;

            // позиции в файлах
            long readPoz_1;
            long readPoz_2;

            // реальные размеры считанных блоков
            int _count1;
            int _count2;

            //входные массивы
            private int[] _arr1;
            private int[] _arr2;

            // позиции во входных массивах
            long idx1 = 0;
            long idx2 = 0;

            private bool _readParity = true; // флаг - откуда взят новый элемент. false - из arr2 , true - из arr1 (а базовый при этом - из противоположного)

            private bool _parityBlocked = false;

            public EmsReader(string file1, string file2, int numSize, int blockSize)
            {
                _file1 = file1;
                _file2 = file2;
                _blockSize = blockSize;
                _numSize = numSize;
                _len_1 = new FileInfo(_file1).Length;
                _len_2 = new FileInfo(_file2).Length;

                readPoz_1 = 0;
                readPoz_2 = 0;

                // сначала загружаем первые блоки из 2-х файлов
                _count1 = _blockSize;
                _count2 = _blockSize;
                _arr1 = ReadBlock(_file1, _numSize, ref readPoz_1, ref _count1);
                _arr2 = ReadBlock(_file2, _numSize, ref readPoz_2, ref _count2);

            }

            /// <summary>
            ///  получение очередного элемента из блока. Если блок выбран полностью - подгружается следующий блок
            /// </summary>
            /// <param name="finish"></param>
            /// <returns></returns>
            public int GetNext(ref bool finish)
            {
                int val = _readParity ? _arr1[idx1++] : _arr2[idx2++];

                // если дошли до конца входного массива, то считываем в него следующую порцию 
                if (idx1 == _count1)
                {
                    idx1 = 0;
                    if (readPoz_1 < _len_1 - 1)
                    {
                        _count1 = _blockSize;
                        _arr1 = ReadBlock(_file1, _numSize, ref readPoz_1, ref _count1);
                    }
                    else // т.е. дошли до конца первого файла
                    {
                        if (_parityBlocked)
                            finish = true;
                        else
                        {
                            // теперь будем забирать элементы только из второго файла
                            _readParity = false;
                            _parityBlocked = true;
                        }
                    }
                }

                // аналогично по 2-му массиву
                if (idx2 == _count2)
                {
                    idx2 = 0;
                    if (readPoz_2 < _len_2 - 1)
                    {
                        _count2 = _blockSize;
                        _arr2 = ReadBlock(_file2, _numSize, ref readPoz_2, ref _count2);
                    }
                    else // т.е. дошли до конца второго файла
                    {
                        if (_parityBlocked)
                            finish = true;
                        else
                        {
                            // теперь будем забирать элементы только из первого файла
                            _readParity = true;
                            _parityBlocked = true;
                        }                        
                    }
                }

                return val;
            }

            public void ChangeParity()
            {
                if(!_parityBlocked)
                    _readParity = !_readParity;
            }

        }

        private class EmsSaver
        {
            private int[] _arrOut; //выходной массив
            private int _idxOut = 0; // позиция в выходном массиве
            
            private string _file1;
            private string _file2;
            private int _blockSize;
            private int _numSize;

            private int _lastValue;
            private long writePoz_1;
            private long writePoz_2;
            bool _writeParity = true; // флаг - куда пишем выходной массив. false - в _file_Out2 , true - в _file_Out1 


            public EmsSaver(string file1, string file2,  int numSize,  int blockSize)
            {
                _numSize = numSize;
                _file1 = file1;
                _file2 = file2;
                writePoz_1 = 0;
                writePoz_2 = 0;
                _blockSize = blockSize;
                _arrOut = new int[blockSize];
                _lastValue = 0; //сначала минимальное из возможных значений (либо меньше)
            }

            public void Put(int value)
            {
                // если очередное значение меньше последнего введенного, то записываем массив и переключаем выходной файл
                if (value < _lastValue)
                { 
                    if (_writeParity)
                    {
                        WriteBlock(_arrOut, _file1, _numSize, ref writePoz_1, _idxOut);
                        _writeParity = false;
                    }
                    else
                    {
                        WriteBlock(_arrOut, _file2, _numSize, ref writePoz_2, _idxOut);
                        _writeParity = true;
                    }

                    _arrOut = new int[_blockSize];
                    _idxOut = 0;
                    //bChangingArray = false;
                }

                // если дошли до конца выходного массива, то записываем массив (без переключения)
                else if (_idxOut == _blockSize)
                {
                    if (_writeParity)
                    {
                        WriteBlock(_arrOut, _file1, _numSize, ref writePoz_1, _idxOut);
                    }
                    else
                    {
                        WriteBlock(_arrOut, _file2, _numSize, ref writePoz_2, _idxOut);
                    }

                    _arrOut = new int[_blockSize];
                    _idxOut = 0;
                }

                // занесение очередного значения в выходной массив
                _arrOut[_idxOut++] = value;
                _lastValue = value;
            }

            public void SaveTail() // записываем последний блок
            {
                if (_writeParity)
                {
                    WriteBlock(_arrOut, _file1, _numSize, ref writePoz_1, _idxOut);
                }
                else
                {
                    WriteBlock(_arrOut, _file2, _numSize, ref writePoz_2, _idxOut);
                }
            }

            public bool isMerged
            { get { return writePoz_2 == 0; } }
        }

    }
   
}
