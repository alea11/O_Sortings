using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sortings
{
    public class RadixFileSort : InFileSorting // одна из 'линейных' - поразрядная сортировка (на файлах)
    {
        int _range;

        // 3 файла: 
        // file_0 - файл с исходным массивом, подлежащим сортировке
        // file_1, file_2 - вспомогательные файлы. результирующий массив в итоге будет в одном из них
        private string _file_1;
        private string _file_2;

        //  те же файлы но с указанной специализацией, переключаемой при обработке очередной секции
        private string _file_Inp;
        private string _file_Out;

        public RadixFileSort(string file, int numSize, int blockSize, int range) : base(file, numSize, blockSize) // range - правая граница диапазона, а левая =0
        {
            _name = "RadixFileSort";
            _range = range;

            _file_1 = "file_1";
            _file_2 = "file_2";
        }

        public override string Sort()// возвращаемое значение - имя выходного файла
        {
            try
            {
                long len = new FileInfo(_file_0).Length;
                N = (int)(len / _numSize); //  к-во элементов

                /////////////////////////////////////
                // определяем размеры и количество блоков (по порядку, начиная с НАЧАЛА файла)
                BlockInfo[] blocks = new BlockInfo[(N - 1) / _blockSize + 1]; //т.е. округление вверх от N/_blockSize
                long idx = 0;
                int j = 0;
                while (idx < N)
                {
                    long rem = N - idx;
                    int cnt = (rem < _blockSize) ? (int)rem : _blockSize;
                    
                    blocks[j++] = new BlockInfo { poz = idx * _numSize, count = cnt, startIdx = idx };
                    
                    idx += _blockSize;
                }

                /////////////////////////////////////
                // формируем секционные маски и массив подсчета

                int maskLen = 8; // байт (можно и 4 (бита) )
                int basis = 1 << maskLen;


                // к-во секций (длиной maskLen)
                int sections = 1;
                int u = _range;
                while ((u >>= maskLen) != 0)
                {
                    sections++;
                }

                _file_Inp = _file_0;
                _file_Out = _file_1;


                // массив подсчета количества по каждому варианту значений(в соотв. окне разрядов) 
                long[,] arrC = new long[basis, sections];

                // маски типа ffff - по секциям числа
                uint mask0 = (uint)(0x1 << maskLen) - 1;
                uint[] masks = new uint[sections];
                for (int s = 0; s < sections; s++)
                {
                    masks[s] = mask0 << (maskLen * s);
                }

                // заполняем массив подсчета значений по секциям

                // цикл по блокам
                for (int blockNum = 0; blockNum < blocks.Length; blockNum++)
                {
                    int count = blocks[blockNum].count;
                    int[] arr = ReadFixBlock(_file_Inp, _numSize, blocks[blockNum].poz, count);

                    // подсчет к-ва по каждому значению, раздельно по секциям
                    for (int i = 0; i < count; i++)
                    {
                        for (int s = 0; s < sections; s++)
                        {
                            int t = (int)(arr[i] & masks[s]) >> (s * maskLen);
                            arrC[t, s]++;
                        }
                    }
                }

                // пересчет элементов массива подсчета - накопление (расчет СТАРТОВОЙ) по каждому значению (в соотв. окне разрядов) - в будущем отсортированном массиве
                for (int s = 0; s < sections; s++)
                {
                    long accumulation = 0;
                    for (int i = 0; i < basis; i++)
                    {
                        long t = arrC[i, s];
                        arrC[i, s] = accumulation;
                        accumulation += t;                        
                    }
                }

                RaiseOnProgress($"completed preparation.");

                //////////////////////////////////////////////////////////////////////// 
                // в цикле по секциям - формирование отсортированного массива (по определенной секции числа)
                
                _file_Inp = _file_0;
                _file_Out = _file_1;

                // цикл по секциям
                for (int s = 0; s < sections; s++)
                {
                    int offset = s * maskLen;

                    // очистка выходного файла
                    using (var fs = File.Create(_file_Out)) { }                    

                    // цикл по считываемым блокам
                    for (int blockNum = 0; blockNum < blocks.Length; blockNum++)
                    {
                        BlockInfo bi = blocks[blockNum];
                        
                        // читаем очередной блок (начиная с НАЧАЛА файла)
                        int[] arr = ReadFixBlock(_file_Inp, _numSize, bi.poz, bi.count);

                        // заполняем массив подсчета значений в блоке (для текущей секции)                        
                        int[] arrBC = new int[basis];

                        // подсчет к-ва по каждому значению
                        for (int i = 0; i < bi.count; i++)
                        {                                
                            int t = (int)(arr[i] & masks[s]) >> offset;
                            arrBC[t]++;                                
                        }

                        // пересчет элементов массива подсчета - накопление (расчет СТАРТОВОЙ) по каждому значению (в соотв. окне разрядов)                        
                        int accumulation = 0;
                        for (int i = 0; i < basis; i++)
                        {
                            int t = arrBC[i];
                            arrBC[i] = accumulation;
                            accumulation += t;                            
                        }                        

                        // массив для результата сортировки в пределах блока
                        int[] arrOut = new int[arr.Length];

                        // заполняем...

                        // цикл по элементам массива считанного блока
                        for (int i =  0; i < bi.count; i++)
                        {
                            // определяем индекс куда класть элемент:
                            
                            // элемент
                            int val = arr[i];
                            // значение соотв. части элемента (в соотв. окне разрядов)
                            int t = (int)(val & masks[s]) >> offset;
                            // это значение - индекс в массиве подсчета. 
                            long idxOut = arrBC[t]++;
                            // по найденной позиции, полученной из массива подсчета - заносим значение в массив результатов по блоку
                            arrOut[idxOut] = val;                            
                        }

                        // после отработки массива подсчета - он содержит уже индексы начала следующего интервала (или индекс за границей - для последнего)

                        // запись интервалов блока в выходной файл
                        int startIdx = 0; //индекс стартового элемента интервала в массиве arrOut
                        for (int i = 0; i < basis; i++)
                        {
                            int count = arrBC[i] - startIdx;
                            if(count > 0)
                            {
                                WriteInterval(arrOut, startIdx, count, _file_Out, _numSize, arrC[i,s]*_numSize);
                                arrC[i, s] += count;
                                startIdx += count;
                            }
                        }
                       

                        RaiseOnProgress($"recorded block {blockNum} on section {s}");

                    }                 

                    // для первого прохода (когда в первом цикле загружали данные с основного файла) - явно задаем очередной файл, а в последующих проходах по секциям  - переключаем файлы
                    string file = s == 0 ? _file_2 : _file_Inp;
                    _file_Inp = _file_Out;
                    _file_Out = file;

                }
                return _file_Inp; // т.е. _file_Out на последнем проходе

            }
            catch (Exception exc)
            {
                ErrMessage = exc.Message;
                return null;
            }
        }


        public override string ToString()
        {
            return $"{_name}: Length: {N}, blockSize: {_blockSize}";
        }


        private struct BlockInfo
        {
            public long poz;
            public int count;
            public long startIdx;
        }
        
    }
}
