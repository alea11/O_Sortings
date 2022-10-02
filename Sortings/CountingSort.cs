using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class CountingSort : Sorting // одна из 'линейных'
    {
        internal CountingSort()
        {
            _name = "CountingSort";
        }

        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            try 
            { 
                Prepare(arr);
                
                // для этого типа сортировки критично чтобы диапазон значений (или в нашем случае макс. значение) - должен быть не больше чем размер массива

                // массив подсчета количества вариантов значений
                int[] arrC = new int[N];

                for (int i = 0; i < N; i++)
                {
                    arrC[arr[i]]++;
                    if (ct.IsCancellationRequested)
                        break;
                }

                int accumulation = 0;
                for (int i = 0; i < N; i++)
                {
                    accumulation += arrC[i];
                    arrC[i] = accumulation;
                    if (ct.IsCancellationRequested)
                        break;
                }

                int[] arrOut = new int[N];

                for (int i = N-1; i >= 0; i--)
                {
                    int val = arr[i];
                    arrOut[--arrC[val]] = val;
                    if (ct.IsCancellationRequested)
                        break;
                }

                Array.Copy(arrOut, arr, N);

                // событие о прогрессе
                RaiseOnProgress("finish");
            }
            catch(Exception exc)
            {
                ErrMessage = $"{exc.Message} \r\nДля этого типа сортировки критично чтобы диапазон значений (или в нашем случае макс. значение) - должен быть не больше чем размер массива";
                RaiseOnError($"Exception: {ErrMessage}") ;
            }

}
    }
}
