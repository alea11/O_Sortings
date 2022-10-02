using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class MergeSort: Sorting
    {
        private CancellationToken _ct;
        // сортировка слиянием
        internal MergeSort()
        {
            _name = "MergeSort"; 
        }
        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            Prepare(arr);
            _ct = ct;

            MSort(0, N - 1);
        }

        private void MSort(int L, int R)
        {
            if (_ct.IsCancellationRequested)
                return;
            if (L >=R) return;
            int M = (L + R) / 2;

            MSort(L, M);
            MSort(M+1, R);
            Merge(L, M, R);
        }

        private void Merge(int L, int M, int R)
        {
            int[] arr2 = new int[R - L + 1];
            int a = L;
            int b = M + 1;
            int idx2 = 0;

            while(a<=M && b <=R)
            {
                if (less(a, b))
                    setToAdditionalArray(arr2, a++, idx2++);
                else
                    setToAdditionalArray(arr2, b++, idx2++);
                if (_ct.IsCancellationRequested)
                    return;
            }    
            
            while (a <= M)
            {
                setToAdditionalArray(arr2, a++, idx2++);
                if (_ct.IsCancellationRequested)
                    return;
            }
                

            while (b <= R)
            {
                setToAdditionalArray(arr2, b++, idx2++);
                if (_ct.IsCancellationRequested)
                    return;
            }
               

            // результат слияния - обратно в основной массив
            setFromAdditionalArray(arr2, 0, L, R - L + 1);

            //for (int i = L; i <= R; i++)
            //    setFromAdditionalArray(arr2, i - L, i);

        }
    }
   
}
