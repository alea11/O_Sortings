using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class InsertionSort : Sorting
    {
        private int _parameter;
        private CancellationToken _ct; 
        internal InsertionSort(int parameter) 
        {
            _parameter = parameter;
            switch (_parameter)
            {
                case 0:
                    _name = "Insertion";                    
                    break;
                case 1:
                    _name = "InsertionShift";                    
                    break;
                case 2:
                    _name = "InsertionBinaryShift";                    
                    break;
                case 3:
                    _name = "InsertionBinaryShift_a";                    
                    break;
            }
        }

        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            Prepare(arr);

            _ct = ct;

            switch (_parameter) 
            {
                case 0:
                    Insertion();
                    break;
                case 1:
                    InsertionShift();
                    break;
                case 2:
                    InsertionBinaryShift();
                    break;
                case 3:
                    InsertionBinaryShift_a();
                    break;
            }

        }


        private void Insertion()
        {            
            for (int i = 1; i < N; i++) //i - граница относительно отсортированной части (первый неотсортированный элемент)
            {
                for (int j = i - 1; j >= 0 && more(j, j + 1); j--)
                {
                    swap(j, j + 1);
                    if (_ct.IsCancellationRequested)
                        break;
                }
                if (_ct.IsCancellationRequested)
                    break;

            }
                
        }


        private void InsertionShift()
        {
            Func<int, int, bool> whileCondition = (_j, _k) =>
            { return _j >= 0 && moreValue(_j, _k); };

            for (int i = 1; i < N; i++) //i - граница относительно отсортированной части (первый неотсортированный элемент)
            {
                swapWithShift(i, whileCondition);
                if (_ct.IsCancellationRequested)
                    break;
            }
        }

        private void InsertionBinaryShift()
        {
            Func<int, int, bool> whileCondition = (_j, _p) =>
            { return _j >= _p; };

            for (int i = 1; i < N; i++) //i - граница относительно отсортированной части (первый неотсортированный элемент)
            {
                int k = Arr[i];
                int p = binarySearch(k, 0, i - 1);
                swapWithShift(i, p, whileCondition);
                if (_ct.IsCancellationRequested)
                    break;
            }
        }

        private void InsertionBinaryShift_a()
        {
            Func<int, int, bool> whileCondition = (_j, _p) =>
            { return _j >= _p; };

            for (int i = 1; i < N; i++) //i - граница относительно отсортированной части (первый неотсортированный элемент)
            {
                int k = Arr[i];
                int p = binarySearch_a(k, 0, i - 1);
                swapWithShift(i, p, whileCondition);
                if (_ct.IsCancellationRequested)
                    break;
            }
        }

        /// <summary>
        /// обмен со сдвигом, пока выполняется условие. условие зависит от счетчика цикла и внутренней переменной
        /// </summary>
        void swapWithShift(int startIdx, Func<int, int, bool> whileCondition)
        {
            RaiseBeforeSwap(startIdx, startIdx);

            int j;
            int cnt = 1; // счетчик к-ва затронутых элементов
            int k = Arr[startIdx];
            _asg++;
            for (j = startIdx - 1; whileCondition(j, k); j--) //j >= 0 && moreK(j, k)
            {
                Arr[j + 1] = Arr[j]; // не полный обмен в 3 приема, а одно присвоение, т.е. получим смещение ряда чисел
                _asg++;
                cnt++;
            }

            if (cnt > 1)
            {
                Arr[j + 1] = k;
                _asg++;

                RaiseOnSwapWithShift(startIdx, cnt);
            }
        }

        /// <summary>
        /// обмен со сдвигом, пока выполняется условие. условие зависит от счетчика цикла и внешней переменной
        ///
        void swapWithShift(int startIdx, int to, Func<int, int, bool> whileCondition)
        {            
            RaiseBeforeSwap(startIdx, startIdx);

            int j;
            int cnt = 1; // счетчик к-ва затронутых элементов
            int k = Arr[startIdx];
            _asg++;
            for (j = startIdx - 1; whileCondition(j, to); j--) // j >= to
            {
                Arr[j + 1] = Arr[j]; // не полный обмен в 3 приема, а одно присвоение, т.е. получим смещение ряда чисел
                _asg++;
                cnt++;
            }

            if (cnt > 1)
            {
                Arr[j + 1] = k;
                _asg++;

                RaiseOnSwapWithShift(startIdx, cnt);                
            }
        }

        int binarySearch(int key, int low, int hight)
        {
            if (hight <= low)
            {
                return  lessValue(low,key)  ? low + 1 : low;
            }

            int mid = (low + hight) / 2;
                        
            if ( equalValue(mid, key))  // этот шаг приводит к потере стабильности,  его можно убирать. однако и без него наш поиск может попасть на серию одинаковых значений и может быть получена позиция гдето в середине серии 
                return mid + 1;
            
            if (lessValue(mid, key))
                return binarySearch(key, mid + 1, hight);
            else
                return binarySearch(key, low, mid - 1); // или до mid , если убрана проверка на == (выше)
        }

        int binarySearch_a(int key, int low, int hight) // та же функция, что выше, но без отдельной проверки  на точное попадание на нужный элемент при разделении последовательности 
        {
            if (hight <= low)
            {
                return lessValue(low, key) ? low + 1 : low;
            }

            int mid = (low + hight) / 2;
            
            if (lessValue(mid, key))
                return binarySearch_a(key, mid + 1, hight);
            else
                return binarySearch_a(key, low, mid); // до mid , т.к. убрана проверка на == 
        }

       

    }

}
