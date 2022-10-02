using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class BubbleSort : Sorting
    {
        internal BubbleSort() 
        {
            _name = "BubbleSort"; 
        }               

        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            Prepare(arr);            

            for (int i = 1; i < N; i++)
            {
                for (int j = 0; j < N - i; j++)
                {
                    if (more(j, j + 1))
                        swap(j, j + 1);
                    if (ct.IsCancellationRequested)
                        break;
                }
                if (ct.IsCancellationRequested)
                    break;
            }
        }
        
    }
}
