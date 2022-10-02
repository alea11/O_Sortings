using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public abstract class Sorting
    {
        public event EventHandler<SwapItemsEventArgs> OnSwap;
        public event EventHandler<SwapWithShiftItemsEventArgs> OnSwapWithShift;
        public event EventHandler<SwapItemsEventArgs> BeforeSwap;
        public event EventHandler<SetFromAdditionalArrayEventArgs> OnSetFromOther;
        public event EventHandler<SetToAdditionalArrayEventArgs> OnSetToOther;
        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<ErrorEventArgs> OnError;
        
        public string ErrMessage = null;

        public int[] Arr;
        protected int N;

        protected uint _cmp = 0;
        protected uint _asg = 0;

        protected string _name;        

        public abstract void Sort(int[] arr, int range=0, CancellationToken ct = default(CancellationToken));
        
        protected void Prepare(int[] arr)
        {
            Arr = arr;
            N = Arr.Length;
            _cmp = 0;
            _asg = 0;
            ErrMessage = null;
        }

        protected bool more(int i1, int i2)
        {
            _cmp++;
            return Arr[i1] > Arr[i2];
        }

        protected bool less(int i1, int i2)
        {
            _cmp++;
            return Arr[i1] < Arr[i2];
        }

        protected bool lessOrEqual(int i1, int i2)
        {
            _cmp++;
            return Arr[i1] <= Arr[i2];
        }

        protected bool moreValue(int i1, int val)
        {
            _cmp++;
            return Arr[i1] > val;
        }

        protected bool lessOrEqualValue(int i1, int val)
        {
            _cmp++;
            return Arr[i1] <= val;
        }

        protected bool lessValue(int i1, int val)
        {
            _cmp++;
            return Arr[i1] < val;
        }

        protected bool equalValue(int i1, int val)
        {
            _cmp++;
            return Arr[i1] == val;
        }


        protected void swap(int i1, int i2)
        {
            RaiseBeforeSwap(i1, i2);
            _asg += 3;
            int t = Arr[i1];
            Arr[i1] = Arr[i2];
            Arr[i2] = t;

            RaiseOnSwap(i1, i2);
        }

        protected void setFromAdditionalArray(int[] array, int from, int to)
        {
            Arr[to] = array[from];
            _asg++;
            RaiseOnSetFromAdditionalArray(from, to, 1);
        }

        protected void setFromAdditionalArray(int[] array, int from, int to, int count)
        {
            Array.Copy(array, from, Arr, to, count);
            _asg+=(uint)count;
            RaiseOnSetFromAdditionalArray(from, to, count);
        }

        protected void setToAdditionalArray(int[] array, int from, int to)
        {
            array[to] = Arr[from];
            _asg++;
            RaiseOnSetToAdditionalArray(from, to, 1);
        }

        protected void RaiseOnSwap(int i1, int i2)
        {
            if (OnSwap != null)
                OnSwap.Invoke(this, new SwapItemsEventArgs(i1, i2));
        }

        protected void RaiseOnSwapWithShift(int i1, int count)
        {
            if (OnSwapWithShift != null)
                OnSwapWithShift.Invoke(this, new SwapWithShiftItemsEventArgs(i1, count));
        }

        protected void RaiseBeforeSwap(int i1, int i2)
        {
            if (BeforeSwap != null)
                BeforeSwap.Invoke(this, new SwapItemsEventArgs(i1, i2));
        }

        protected void RaiseOnSetFromAdditionalArray(int i1, int i2, int count)
        {
            if (OnSetFromOther != null)
                OnSetFromOther.Invoke(this, new SetFromAdditionalArrayEventArgs(i1, i2, count));
        }

        protected void RaiseOnSetToAdditionalArray(int i1, int i2, int count)
        {
            if (OnSetToOther != null)
                OnSetToOther.Invoke(this, new SetToAdditionalArrayEventArgs(i1, i2, count));
        }
        
        protected void RaiseOnProgress(string text)
        {
            if (OnProgress != null)
                OnProgress.Invoke(this, new ProgressEventArgs(text));
        }

        protected void RaiseOnError(string errMessage)
        {
            if (OnError != null)
                OnError.Invoke(this, new ErrorEventArgs(errMessage));
        }

        public string Name { get {return _name; } }
        public string Statistic { get { return $"Length: {N}, cmp: {_cmp}, asg: {_asg}"; } }

        

        public static Sorting CreateSorting(eAlgorithm alg)
        {
            switch(alg)
            {
                case eAlgorithm.Bubble:
                    return new BubbleSort();
                case eAlgorithm.Insertion:
                    return new InsertionSort(0);
                case eAlgorithm.InsertionShift:
                    return new InsertionSort(1);
                case eAlgorithm.InsertionBinaryShift:
                    return new InsertionSort(2);
                case eAlgorithm.InsertionBinaryShift_a:
                    return new InsertionSort(3);
                case eAlgorithm.Shell_Shell:
                    return new ShellSort(0);
                case eAlgorithm.Shell_Frank:
                    return new ShellSort(1);
                case eAlgorithm.Shell_Hibbard:
                    return new ShellSort(2);
                case eAlgorithm.Shell_Papernov:
                    return new ShellSort(3);
                case eAlgorithm.Shell_Knuth:
                    return new ShellSort(4);
                case eAlgorithm.Shell_Sedgewick_82:
                    return new ShellSort(5);
                case eAlgorithm.Shell_Ciura:
                    return new ShellSort(6);
                case eAlgorithm.SelectionSort:
                    return new SelectionSort();
                case eAlgorithm.HeapSort:
                    return new HeapSort();
                case eAlgorithm.QuikSort_Hoar:
                    return new QuikSort(0);
                case eAlgorithm.QuikSort_Lomuto:
                    return new QuikSort(1);
                case eAlgorithm.MergeSort:
                    return new MergeSort();
                case eAlgorithm.BucketSort:
                    return new BucketSort();
                case eAlgorithm.CountingSort:
                    return new CountingSort();
                case eAlgorithm.RadixSort:
                    return new RadixSort();
                

                default:
                    throw new Exception("Not resolved.");
            }
        }


        public enum eAlgorithm
        {
            Bubble,
            Insertion,
            InsertionShift,
            InsertionBinaryShift,
            InsertionBinaryShift_a,
            Shell_Shell,
            Shell_Frank,
            Shell_Hibbard,
            Shell_Papernov,
            Shell_Knuth,
            Shell_Sedgewick_82,
            Shell_Ciura,
            SelectionSort,
            HeapSort,
            QuikSort_Hoar,
            QuikSort_Lomuto,
            MergeSort,
            //ExternalMergeSort,
            CountingSort,
            RadixSort,
            BucketSort,

        }

        public enum ePresort
        {
            Random,
            Sorted,
            ReverseSorted
        }


    }
}
