using Sortings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testing
{
    class Tester
    {
        private string _checkFilesPath;
        private int _maxDuration;
        private Sorting _sorting;
        private int _minTestNumber;
        private int _maxTestNumber;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public int FixRange { get; set; } = 0;

        public Tester(Sorting sorting, string checkFilesPath, int minTestNumber = 0, int maxTestNumber = 0, int maxDuration = 0)
        {
            _sorting = sorting;
            _checkFilesPath = checkFilesPath;
            _maxDuration = maxDuration;
            _minTestNumber = minTestNumber;
            _maxTestNumber = maxTestNumber;

            if (_minTestNumber < 0 || _maxTestNumber < _minTestNumber)
                throw new Exception("invalid test number range.");

            if (maxDuration < 0)
                throw new Exception("invalid MaxDuration.");
        }

        public void Run()
        {
            Console.WriteLine($"Method: {_sorting.Name}");
            for (int testNumber = _minTestNumber; _maxTestNumber > 0 ? testNumber <= _maxTestNumber : true; testNumber++)
            {
                string inFile = Path.Combine(_checkFilesPath, $"test.{testNumber}.in");
                string outFile = Path.Combine(_checkFilesPath, $"test.{testNumber}.out");

                if (!File.Exists(inFile) || !File.Exists(outFile))
                {
                    // если диапазон номеров тестов указан - пропускаем тест по отсутствующему файлу
                    if (_maxTestNumber > 0)
                    {
                        Console.WriteLine($"Test #{testNumber}: - skipped");
                        continue;
                    }
                    else // если диапазон номеров тестов НЕ определен , то отсутствие файлов - это признак завершения
                    {
                        break;
                    }
                }

                Console.Write($"Test #{testNumber}:   ");

                string[] data = File.ReadAllLines(inFile);                

                int count = int.Parse(data[0]);
                int[] arr = data[1].Split(' ').Select(s => int.Parse(s)).ToArray();

                string expect = File.ReadAllText(outFile).Trim(); 
                int[] expectArr = expect.Split(' ').Select(s => int.Parse(s)).ToArray();

                int range = FixRange > 0 ? FixRange : count;

                if (RunTest(arr, range, expectArr) == false) // если случился таймаут, то следующие, более тяжелые тесты и не запускаем
                    break;
            }
        }

        private bool RunTest(int[] arr, int range, int[] expectArr)
        {
            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            long duration = 0;

            if (_maxDuration > 0)
            {
                using (Timer t1 = new Timer((t)=> { _cts.Cancel(); }, null, _maxDuration, Timeout.Infinite))
                {
                    _RunTest(arr, range, ct, ref duration);
                }

                if (_cts.IsCancellationRequested == true)
                {
                    Console.WriteLine($"Terminated by timeout, duration: {duration} ms");
                    return false;
                }
            }
            else
            {
                _RunTest(arr, range,  ct, ref duration);
            }


            if (arr.Length == expectArr.Length)
            {
                bool success = true;
                for (int i = 0; i < arr.Length; i++)
                {
                    success = arr[i] == expectArr[i];
                    if (!success)
                        break;
                }
                Console.WriteLine($"{success}, length: {arr.Length},  duration: {duration} ms");
            }
            else
                Console.WriteLine($"Error:  actuals.Length != expects.Length.");

            return true;
        }

        private void _RunTest(int[] arr, int range, CancellationToken ct, ref long duration)
        {
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {      
                _sorting.Sort(arr, range, ct);
            }
            catch (Exception exc)
            {
                string errmsg = exc.InnerException == null ? exc.Message : exc.InnerException.Message;
                Console.WriteLine($"Exception:  {errmsg}");
            }

            sw.Stop();
            duration = sw.ElapsedMilliseconds;           
        }

    }
}
