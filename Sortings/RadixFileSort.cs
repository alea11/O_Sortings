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
                // определяем размеры и количество блоков (по порядку, начиная с конца файла)
                BlockInfo[] blocks = new BlockInfo[(N - 1) / _blockSize + 1]; //т.е. округление вверх от N/_blockSize
                long idx = N;
                int j = 0;
                while (idx > 0)
                {
                    idx -= _blockSize;
                    if(idx >= 0)
                    {
                        blocks[j++] = new BlockInfo {poz = idx*_numSize, count = _blockSize, startIdx = idx };
                    }
                    else
                        blocks[j] = new BlockInfo { poz = 0, count = N % _blockSize, startIdx = 0 };
                }

                /////////////////////////////////////
                // формируем секционные маски и массив подсчета

                int maskLen = 8; // байт (можно и 4)
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
                long[,] arrC = new long[basis,sections];

                // маски типа ffff - по секциям числа
                uint[] masks = new uint[sections];
                for (int s = 0; s<sections;s++)
                {
                    masks[s] = ((uint)(0x1 << maskLen) - 1) << (maskLen * s);
                }

                // заполняем массив подсчета значений по секциям

                // цикл по блокам
                for (int readBlockNum = 0; readBlockNum < blocks.Length; readBlockNum++)
                {                    
                    int count = blocks[readBlockNum].count;
                    int[] arr = ReadFixBlock(_file_Inp, _numSize, blocks[readBlockNum].poz, count);

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

                // пересчет элементов массива подсчета - накопление (расчет макс. позиции +1) по каждому значению (в соотв. окне разрядов) - в будущем отсортированном массиве
                for (int s = 0; s < sections; s++)
                {
                    long accumulation = 0;
                    for (int i = 0; i < basis; i++)
                    {
                        accumulation += arrC[i, s];
                        arrC[i, s] = accumulation;
                    }
                }

                RaiseOnProgress($"completed preparation.");

                ////////////////////////////////////////////////////////////////////////
                // в цикле по секциям - формирование отсортированного массива (по определенной секции числа)
                // - в двух вложенных циклах по записываемым и загружаемым блокам

                _file_Inp = _file_0;
                _file_Out = _file_1;
                
                // цикл по секциям
                for (int s = 0; s < sections; s++)
                {
                    int offset = s * maskLen;

                    // очистка выходного файла
                    using (var fs = File.Create(_file_Out)) { }

                    // массив для отметки использования очередного считанного из входного и полмещенного в выходной массив элемента
                    // - для предотвращения повторного распределения элемента на следующей иттерации по выходным блокам (на каждой такой иттерации снова перебираем все входные блоки)
                    long[] usedIdxs = new long[basis];
                    for (int i = 0; i < basis; i++)
                        usedIdxs[i] = -1;

                    // цикл по записываемым блокам
                    for (int writeBlockNum = 0; writeBlockNum < blocks.Length; writeBlockNum++)
                    {                        
                        long startWriteIdx = blocks[writeBlockNum].startIdx;
                        int count = blocks[writeBlockNum].count;
                        long untilIdx = startWriteIdx + count;
                        int[] arrOut = new int[count];

                        // индекс очередного считываемого элемента (по очередности считывания) - сквозной по считываемым блокам внутри иттерации по выходному блоку
                        long idxIn = 0;

                        // цикл по считываемым блокам
                        for (int readBlockNum = 0; readBlockNum < blocks.Length; readBlockNum ++)
                        {
                            // читаем очередной блок (начиная с конца файла)
                            int[] arr = ReadFixBlock(_file_Inp, _numSize, blocks[readBlockNum].poz, blocks[readBlockNum].count);

                            // цикл по элементам массива считанного блока
                            for (int i = blocks[readBlockNum].count - 1; i >= 0; i--)
                            {
                                // определяем макс индекс куда может быть положен элемент:
                                // элемент
                                int val = arr[i];
                                // значение соотв. части элемента (в соотв. окне разрядов)
                                int t = (int)(val & masks[s]) >> offset;
                                // это значение - индекс в массиве подсчета. 
                                // значение в массиве подсчета по этому индексу(с декрементом) - максимальный индекс соотв. элемента в выходном массиве. 
                                // при этом значение в массиве подсчета уменьшается на 1 (для следующего элемента соответствующего этой же маске индекс в выходном массиве будет уменьшен)

                                // но сначала проверим - не учли ли уже считанный элемент
                                if( idxIn > usedIdxs[t])
                                {
                                    // - заносим в соотв. блок на запись (с учетом стартового индекса блока)
                                    long idxOut = arrC[t, s] - 1;
                                    if(idxOut>= startWriteIdx && idxOut< untilIdx)
                                    {
                                        arrOut[idxOut - startWriteIdx] = val;
                                        arrC[t, s]--;
                                        usedIdxs[t] = idxIn;
                                    }
                                }

                                
                                idxIn++;
                            }
                        }

                        // пишем подготовленный блок в выходной файл
                        WriteFixBlock(arrOut, _file_Out, _numSize, blocks[writeBlockNum].poz);
                        RaiseOnProgress($"recorded block {writeBlockNum} on section {s}");
                    }

                    // для первого прохода (когда в первом цикле загружали данные с основного файла) - явно задаем очередной файл, а в последующих проходах  - переключаем файлы
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
