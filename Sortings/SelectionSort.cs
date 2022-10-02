using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class SelectionSort: Sorting
    {
        internal SelectionSort()
        {
            _name = "SelectionSort"; 
        }
        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            Prepare(arr);            

            int x = findMax(N);
            for (int i = N - 1; i > 0; i--)
            {
                if (ct.IsCancellationRequested)
                    return;
                swap(x, i);
                x = findMax(i);
            }
        }

        int findMax(int untilIdx)
        {
            int idx = 0;
            for (int i = 1; i < untilIdx; i++)
            {
                if (more(i, idx))
                    idx = i;
            }
            return idx;
        }
    }
    
}
