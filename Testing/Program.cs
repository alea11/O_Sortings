using Sortings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        // пути к папкам с данными тестирования (относительно места сборки)
        private const string path_random = "..\\..\\..\\TestingFiles\\0.random";
        private const string path_digits = "..\\..\\..\\TestingFiles\\1.digits";
        private const string path_sorted = "..\\..\\..\\TestingFiles\\2.sorted";
        private const string path_revers = "..\\..\\..\\TestingFiles\\3.revers";




        //static void Main(string[] args)
        //{
        //    Tester tester1 = new Tester(new O2_LuckyTickets.DirectlyWay6(), path_02, 2, 2);
        //    tester1.Run();

        //    Tester tester2 = new Tester(new O2_LuckyTickets.DirectlyWay6_a(), path_02, 2, 2);
        //    tester2.Run();

        //    Console.WriteLine("\n\rpress any key");
        //    Console.ReadKey();

        //}



        static void Main(string[] args)
        {
            while (Work()) ;
        }

        public static bool Work() //int len, int max
        {
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Select Source type:");
            Console.WriteLine("1 - Random");
            Console.WriteLine("2 - Digits");
            Console.WriteLine("3 - Sorted");
            Console.WriteLine("4 - Revers");
            Console.WriteLine("other - quit");
            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            string testingfilesPath;
            int fixrange = 0;
            
            switch (c)
            {
                case '1':
                    testingfilesPath = path_random;
                    break;
                case '2':
                    testingfilesPath = path_digits;
                    fixrange = 10;
                    break;
                case '3':
                    testingfilesPath = path_sorted;
                    break;
                case '4':
                    testingfilesPath = path_revers;
                    break;
                default:
                    return false;
            }

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

            Sorting sorting;

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

            Console.WriteLine("Input max duration for each test (seconds):");
            string s1 = Console.ReadLine();
            Console.WriteLine();
            int maxDuration = int.Parse(s1);

            Tester tester = new Tester(sorting, testingfilesPath, maxDuration: maxDuration * 1000);           

            tester.FixRange = fixrange;

            tester.Run();

            return true;
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

    }
}
