using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class BucketSort : Sorting // одна из 'линейных'
    {
        internal BucketSort() 
        {
            _name = "BucketSort"; 
        }

        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                Prepare(arr);

                // вариант с использованием List для Bucket - относительно быстрее, но больше расход памяти (раньше наступает OverflowException)
                //Sort1(arr, ct);

                // вариант с использованием своего типа для Bucket - относительно медленнее 
                // (т.к. приходится создавать объект на каждый элемент массива), а памяти раходуется меньше, чем в первом варианте (структура Bucket - минимальная)
                Sort2(arr, ct);                
            }
            catch(Exception exc)
            {
                ErrMessage = exc.Message;
                RaiseOnError($"Exception: {ErrMessage}");
            }            
        }

        void Sort1(int[] arr, CancellationToken ct = default(CancellationToken))
        {
            // массив блоков
            List<int>[] buckets = new List<int>[N];

            // определяем диапазон значений во входном массиве и коэфф. пересчета значения к индексу
            int max = arr[0];
            int min = arr[0];

            for (int i = 1; i < N; i++)
            {
                if (arr[i] > max)
                    max = arr[i];
                if (arr[i] < min)
                    min = arr[i];

                if (ct.IsCancellationRequested)
                    break;
            }
            double k = (double)N / (max - min + 1);


            // распределяем элементы по блокам (с сортировкой в блоке при вставке)
            for(int i = 0;i<N;i++)
            {
                // номер блока
                int idx = (int)((arr[i]-min) * k);

                if (buckets[idx] == null)
                    buckets[idx] = new List<int>();                
                
                // поиск индекса вставки в блоке
                int j = 0;
                while(j < buckets[idx].Count && arr[i] > buckets[idx][j])
                {
                    j++;
                } 

                // вставка
                buckets[idx].Insert(j, arr[i]);
                if (ct.IsCancellationRequested)
                    break;
            }

            // перенос элементов в основной массив из блоков по порядку
            int resIdx = 0;
            for(int idx = 0; idx < buckets.Length; idx++ )
            {
                if(buckets[idx] != null)
                {
                    for (int j = 0; j < buckets[idx].Count; j++)
                        arr[resIdx++] = buckets[idx][j];
                }
                if (ct.IsCancellationRequested)
                    break;
            }

            // событие о прогрессе
            RaiseOnProgress("finish");
        }


        void Sort2(int[] arr, CancellationToken ct = default(CancellationToken))
        {            
            // массив блоков
            Bucket[] buckets = new Bucket[N];

            // определяем диапазон значений во входном массиве и коэфф. пересчета значения к индексу
            int max = arr[0];
            int min = arr[0];

            for (int i = 1; i < N; i++)
            {
                if (arr[i] > max)
                    max = arr[i];
                if (arr[i] < min)
                    min = arr[i];
                if (ct.IsCancellationRequested)
                    break;
            }
            double k = (double)N / (max - min + 1);


            // распределяем элементы по блокам (с сортировкой в блоке при вставке)
            for (int i = 0; i < N; i++)
            {
                // номер блока
                int idx = (int)((arr[i] - min) * k);

                if (buckets[idx] == null)
                    buckets[idx] = new Bucket(arr[i]);                
                else
                    // вставка
                    buckets[idx].Insert(arr[i]);

                if (ct.IsCancellationRequested)
                    break;
            }

            // перенос элементов в основной массив из блоков по порядку
            int resIdx = 0;
            for (int idx = 0; idx < buckets.Length; idx++)
            {
                if (buckets[idx] != null)
                {
                    Item item = buckets[idx].First;
                    arr[resIdx++] = item.Value;                    
                    while((item = item.Next) != null )
                    {
                        arr[resIdx++] = item.Value;
                    }
                }
                if (ct.IsCancellationRequested)
                    break;
            }

            // событие о прогрессе
            RaiseOnProgress("finish");
        }


        private class Item
        {
            public readonly int Value;
            public Item Next = null;
            public Item(int value)
            {
                Value = value;
            }
        }

        private class Bucket
        {
            public Item First = null;
            public Bucket(int val)
            {
                First = new Item(val);
            }
            public void Insert(int val)
            {
                Item newItem = new Item(val);
                Item last = null;
                Item current = First;

                while (current.Value < val)
                {
                    last = current;
                    current = current.Next;
                    if (current == null)
                        break;
                }
                if (last == null)
                {
                    First = newItem;
                }
                else
                {
                    last.Next = newItem;
                }
                newItem.Next = current;
            }
        }


    }
}
