using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class RadixSort : Sorting // одна из 'линейных' - поразрядная сортировка
    {        
        internal RadixSort() 
        {            
            _name = "RadixSort"; 
        }

        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))// range - правая граница диапазона, а левая =0
        {
            try 
            {
                Prepare(arr);                

                int maskLen = 8; // байт (можно и 4 (бита) )
                int basis = 1 << maskLen;

                // определяем актуальное к-во разрядов
                
                // к-во двоичных разрядов (округленно по maskLen)
                int bits = maskLen;
                int u = range;
                while((u >>= maskLen) != 0)
                {
                    bits+=maskLen;
                }

                uint mask0 = (uint)(0x1 << maskLen) - 1;
                // цикл по секциям (диапазонам разрядов) числа
                for (int offset = 0; offset < bits; offset += maskLen)
                {
                    // массив подсчета количества по каждому варианту значений(в соотв. окне разрядов) - в будущем отсортированном массиве
                    int[] arrC = new int[basis];

                    // выделение соотв. части чисел (элементов сортируемого массива) и подсчет этих частичных значений 
                    uint mask = mask0 << offset;
                    for (int i = 0; i < N; i++)
                    {
                        if (ct.IsCancellationRequested)
                            return;
                        int t = (int)(arr[i] & mask) >> offset;
                        arrC[t]++;
                    }

                    // накопление (расчет макс. позиции) по каждому значению (в соотв. окне разрядов)
                    int accumulation = 0;
                    for (int i = 0; i < basis; i++)
                    {
                        if (ct.IsCancellationRequested)
                            return;
                    
                        accumulation += arrC[i];
                        arrC[i] = accumulation;
                    }

                    int[] arrOut = new int[N];

                    for (int i = N - 1; i >= 0; i--)
                    {
                        if (ct.IsCancellationRequested)
                            return;

                        // определяем макс индекс куда может быть положен элемент:
                        // элемент
                        int val = arr[i];
                        // значение соотв. части элемента (в соотв. окне разрядов)
                        int t = (int)(val & mask) >> offset;
                        // это значение - индекс в массиве подсчета. 
                        // значение в массиве подсчета по этому индексу(с декрементом) - максимальный индекс соотв. элемента в выходном массиве. 
                        // при этом значение в массиве подсчета уменьшается на 1 (для следующего элемента соответствующего этой же маске индекс в выходном массиве будет уменьшен)
                        arrOut[--arrC[t]] = val;
                    }

                    Array.Copy(arrOut, arr, N);

                    // событие о прогрессе
                    RaiseOnProgress($"processed {offset/ maskLen +1} / {bits/maskLen}"); 
                }

                           
            }
            catch(Exception exc)
            {
                ErrMessage = exc.Message;
                RaiseOnError($"Exception: {ErrMessage}");
            }
        }
    }
}
