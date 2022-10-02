using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sortings;

namespace SortConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input Array length, or filename for external sorting :");
            string s = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Input range (max value):");
            string srange = Console.ReadLine();
            Console.WriteLine();

            int range;
            if (int.TryParse(srange, out range))
            {
                int len;
                if(int.TryParse(s, out len))
                {  
                    while (Work(len, range)) ;                    
                }
                else // 
                {
                    string file = s;
                    int blockSize, numSize;                    

                    Console.WriteLine();
                    Console.WriteLine("Select sorting:");
                    Console.WriteLine("1 - ExternalMergeSort");
                    Console.WriteLine("2 - RadixFileSort");                    
                    Console.WriteLine("other - quit");
                    char c = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    Console.WriteLine();

                    switch (c)
                    {
                        case '1':
                            PrepareExternalSorting(file, range, out blockSize, out numSize);
                            RunExternalSorting(new ExternalMergeSort(file, numSize, blockSize));
                            break;
                        case '2':
                            PrepareExternalSorting(file, range, out blockSize, out numSize);
                            RunExternalSorting(new RadixFileSort(file, numSize, blockSize, range));
                            break;
                    }                  
                    
                    Console.ReadKey();
                }
            }

                
          
            
            
            
            
        }

        public static bool Work(int len, int range)
        {
            Console.WriteLine();
            Console.WriteLine("Select Presort type:");
            Console.WriteLine("1 - Random");
            Console.WriteLine("2 - Sorted");
            Console.WriteLine("3 - Reverse Sorted");
            Console.WriteLine("other - quit");
            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            Sorting.ePresort presort;
            switch (c)
            {
                case '1':
                    presort = Sorting.ePresort.Random;
                    break;
                case '2':
                    presort = Sorting.ePresort.Sorted;
                    break;
                case '3':
                    presort = Sorting.ePresort.ReverseSorted;
                    break;
                default:
                    return false;
            }


            Sorting sorting;

            Console.WriteLine("Select part:");
            Console.WriteLine("1 - Simple Sorts");
            Console.WriteLine("2 - Shell");
            Console.WriteLine("3 - Heap");
            Console.WriteLine("4 - Quik, Merge");
            Console.WriteLine("5 - Counting...");
            Console.WriteLine("other - quit");

            c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            switch (c)
            {
                case '1':
                    sorting = SelectSimpleSortsPart();
                    break;
                case '2':
                    sorting = SelectShellPart();
                    break;
                case '3':
                    sorting = SelectHeapPart();
                    break;
                case '4':
                    sorting = SelectQuikPart();
                    break;
                case '5':
                    sorting = SelectCountingPart();
                    break;
                default:
                    return false;
            }

            if (sorting == null)
                return false;

            int[] arr = CreateArray(len, range, presort);
            if(arr != null)
                Run(sorting, arr, range);           

            return true;
        }

        private static void PrepareExternalSorting(string file, int max, out int blockSize, out int numSize)
        {
            Console.WriteLine("Create ? - If need - input length. otherwise - 0 :");
            string s1 = Console.ReadLine();
            Console.WriteLine();
            int len = int.Parse(s1);


            Console.WriteLine("blockSize ? :");
            string s2 = Console.ReadLine();
            Console.WriteLine();
            blockSize = int.Parse(s2);

            // вычисляем к-во байт на элемент
            numSize=1;
            int u = max;
            while ((u >>= 8) != 0)
            {
                numSize++;
            }

            if (len > 0)
            {
                Stopwatch sw = new Stopwatch();
                Console.Write("Creating file ...");
                sw.Start();
                InFileSorting.CreateFile(file, numSize, blockSize, len, Sorting.ePresort.Random, max);
                sw.Stop();
                string time = sw.ElapsedMilliseconds < 5000 ?
                    $"{sw.ElapsedMilliseconds} мс"
                    : sw.Elapsed.ToString(@"hh\:mm\:ss");
                Console.WriteLine($" - finish ({time})");
            }           
            
        }

        

        private static Sorting SelectSimpleSortsPart()
        {
            Console.WriteLine("Select algorithm:");
            Console.WriteLine("1 - Bubble");
            Console.WriteLine("2 - Insertion");
            Console.WriteLine("3 - InsertionShift");
            Console.WriteLine("4 - InsertionBinaryShift");
            Console.WriteLine("5 - InsertionBinaryShift_a");
            Console.WriteLine("other - quit");

            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            switch (c)
            {
                case '1':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Bubble);
                case '2':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Insertion);
                case '3':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.InsertionShift);                    
                case '4':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.InsertionBinaryShift);                    
                case '5':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.InsertionBinaryShift_a);
                          
                default:
                    return null;
            }
        }

        private static Sorting SelectShellPart()
        {
            Console.WriteLine("Select algorithm:");
            Console.WriteLine("1 - Shell_Shell");
            Console.WriteLine("2 - Shell_Frank");
            Console.WriteLine("3 - Shell_Hibbard");
            Console.WriteLine("4 - Shell_Papernov");
            Console.WriteLine("5 - Shell_Knuth");
            Console.WriteLine("6 - Shell_Sedgewick_82");
            Console.WriteLine("7 - Shell_Ciura");
            Console.WriteLine("other - quit");

            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            switch (c)
            {
                case '1':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Shell);
                case '2':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Frank);
                case '3':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Hibbard);
                case '4':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Papernov);
                case '5':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Knuth);
                case '6':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Sedgewick_82);
                case '7':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.Shell_Ciura);
                default:
                    return null;
            }
        }

        private static Sorting SelectHeapPart()
        {
            Console.WriteLine("Select algorithm:");
            Console.WriteLine("1 - SelectionSort");
            Console.WriteLine("2 - HeapSort");
            
            Console.WriteLine("other - quit");

            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            switch (c)
            {
                case '1':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.SelectionSort);
                case '2':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.HeapSort);                   
                
                default:
                    return null;
            }
        }

        private static Sorting SelectQuikPart()
        {
            Console.WriteLine("Select algorithm:");
            Console.WriteLine("1 - QuikSort Hoar");
            Console.WriteLine("2 - QuikSort Lomuto");
            Console.WriteLine("3 - MergeSort");

            Console.WriteLine("other - quit");

            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            switch (c)
            {
                case '1':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.QuikSort_Hoar);
                case '2':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.QuikSort_Lomuto);
                case '3':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.MergeSort);                  
                 
                default:
                    return null;
            }            
        }

        private static Sorting SelectCountingPart()
        {
            Console.WriteLine("Select algorithm:");
            Console.WriteLine("1 - BucketSort");
            Console.WriteLine("2 - CountingSort");
            Console.WriteLine("3 - RadixSort");            

            Console.WriteLine("other - quit");

            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            switch (c)
            {
                case '1':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.BucketSort);
                case '2':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.CountingSort);
                case '3':
                    return Sorting.CreateSorting(Sorting.eAlgorithm.RadixSort);                                
                 
                default:
                    return null;
            }            
        }

        

        private static int[] CreateArray(int size, int max, Sorting.ePresort presort)
        {
            try
            {
                int[] a = new int[size];

                if (presort == Sorting.ePresort.Random)
                {
                    Random random = new Random(12345);
                    for (int i = 0; i < size; i++)
                        a[i] = random.Next(max);
                }
                else
                {
                    float k = (float)max / size;

                    if (presort == Sorting.ePresort.Sorted)
                    {
                        for (int i = 0; i < size; i++)
                            a[i] = (int)(i * k);
                    }
                    else
                    {
                        for (int i = 0; i < size; i++)
                            a[i] = (int)((size - 1 - i) * k);
                    }

                }

                return a;
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Error on creatig array: {exc.Message}");
                return null;
            }            
        }        

        public static void Run(Sorting sorting, int[] arr, int range = 0)
        {
            Console.Write("Sorting...  ");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sorting.Sort(arr, range);
            sw.Stop();
            if(sorting.ErrMessage != null)
                Console.WriteLine($"{sorting.Name} , Error: {sorting.ErrMessage}");
            else
                Console.WriteLine($"{sorting.Name}: {sorting.Statistic} , time: {sw.ElapsedMilliseconds} мс" );
            Console.WriteLine("---------------------------------------------");
        }

        public static void RunExternalSorting(InFileSorting sorting)
        {
            Console.Write("Sorting...  ");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string file = sorting .Sort();
            sw.Stop();
            string time = sw.ElapsedMilliseconds < 5000 ?
                $"{sw.ElapsedMilliseconds} мс"
                : sw.Elapsed.ToString(@"hh\:mm\:ss");
            Console.WriteLine($"{sorting} , time: {time}, outFile: {file}");
            Console.WriteLine("---------------------------------------------");
        }
    }
}
