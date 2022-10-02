using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    // развитие Selectionsort, с оптимизацией поиска максимального элемента на каждом проходе - массив перестраиваем в кучу и берем первый (т.е. максимальный элемент)
    // - первое построение кучи идет относительно долго, но последующие ее корректировки - за логарифмическое время
    public class HeapSort: Sorting
    {
        internal HeapSort()
        {
            _name = "HeapSort"; 
        }

        private CancellationToken _ct;
        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            _ct = ct;
            Prepare(arr);
            

            // готовим кучу (выносим максимальный элемент в корень), начинаем с родителя последнего элемента
            for (int root = N / 2 - 1; root >= 0; root--)
            {
                heapify(root, N);

                if (_ct.IsCancellationRequested)
                    break;
            }

            for (int i = N - 1; i > 0; i--)
            {
                // переносим максимальный элемент (верхний в куче) - в правую часть массива (начало отсортированной части)
                swap(0, i);

                // восстанавливаем кучу: максимальный элемент - наверх. начинаем с корня , т.к. нижележащие подкучи - не искажены.
                // т.е. новый корень кучи (взятый с конца кучи, обмененный с максимальным элементом),  - снова опускаем обменами по максимальной ветке 
                heapify(0, i);

                if (_ct.IsCancellationRequested)
                    break;
            }
        }

        void heapify(int root, int untilIdx )
        {
            // индексы левого и правого детей текущего узла кучи
            int L = 2 * root + 1;
            int R = 2 * root + 2;

            int x = root;
            if (L < untilIdx && more(L, x))
                x = L;            
            if (R < untilIdx && more(R, x))
                x = R;

            if (_ct.IsCancellationRequested)
                return;

            if (x == root)
                return;
            swap(x, root);
            heapify(x, untilIdx);
        }
    }
   
}
