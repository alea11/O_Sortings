using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class ShellSort: Sorting
    {
        //private GapGenerator _gen;
        private int _parameter;

        internal ShellSort(int parameter)
        {
            _name = "ShellSort";
            _parameter = parameter;
            //_gen = CreateGapGenerator(parameter, arrayLength);
            switch (parameter)
            {
                case 0:
                    _name += "/Gap: Shell";
                    break;
                case 1:
                    _name += "/Gap: Frank";
                    break;
                case 2:
                    _name += "/Gap: Hibbard";
                    break;
                case 3:
                    _name += "/Gap: Papernov";
                    break;
                case 4:
                    _name += "/Gap: Knuth";
                    break;
                case 5:
                    _name += "/Gap: Sedgewick_82";
                    break;
                case 6:
                    _name += "/Gap: Ciura";
                    break;
                default:
                    throw new Exception("unknown sorting");
            }
        }
        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            Prepare(arr);
            GapGenerator gen = CreateGapGenerator(_parameter);

            // традиционный вариант: ShellGapGenerator:
            //for (int gap = N / 2; gap > 0; gap /= 2) // уменьшающийся размер шага 'расчески', от половины N

            int k = 0;
            int gap;
            while ((gap = gen.GetGap(k++)) > 0)
            {
                
                for (int i = gap; i < N; i++)
                {
                    if (ct.IsCancellationRequested)
                        return;
                    for (int j = i; j >= gap && more(j - gap, j); j -= gap)
                        swap(j, j - gap);
                }
                    
            }
        }

        private GapGenerator CreateGapGenerator(int parameter)
        {
            switch (parameter)
            {
                case 0:                    
                    return  new ShellGapGenerator(N);
                case 1:                    
                    return new FrankGapGenerator(N);
                case 2:
                    return new HibbardGapGenerator(N);
                case 3:
                    return new PapernovGapGenerator(N);
                case 4:
                    return new KnuthGapGenerator(N);
                case 5:
                    return new SedgewickGapGenerator(N);
                case 6:
                    return new CiuraGapGenerator(N);
                default:
                    throw new Exception("unknown sorting");
            }
        }

        private abstract class GapGenerator
        {
            protected int N;
            protected int maxGap;
            protected int maxK;
            protected int[] gaps;

            public GapGenerator(int n)
            {  
                N = n;
                int log = 1;
                int nn = n;
                while (nn > 0)
                {
                    nn = nn >> 1;
                    log++;
                }
                gaps = new int[log]; // даже с запасом
            }
            public virtual int GetGap(int k) // k  = 0...
            {
                // по умолчанию - список gaps - инверсный, от 1, а отрабатывать должны сначала максимальные , поэтому достаем в обратном порядке
                if (k > maxK)
                    return 0;

                return gaps[maxK - k];
            }
        }

        private class ShellGapGenerator :GapGenerator
        {
            public ShellGapGenerator(int n) : base(n) { }
            public override int GetGap(int k)
            {
                return N >> (k+1); // N/(2^(k+1))
            }
        }

        private class FrankGapGenerator : GapGenerator
        {
            private bool done;
            public FrankGapGenerator(int n) : base(n) { }
            public override int GetGap(int k)
            {
                if (done)
                    return 0;
                int gap = ((N >> (k + 2))<<1) +1; // N/(2^(k+2)) +1
                if (gap == 1)
                    done = true;
                return gap;
            }
        }

        private class HibbardGapGenerator : GapGenerator
        {
            public HibbardGapGenerator(int n) : base(n) 
            {
                maxGap = (N >> 1);

                int k;
                for (k =0; k<gaps.Length; k++)
                {
                    int gap = (2 << k) - 1;
                    if(gap > maxGap)
                    {
                        break;
                    }
                    gaps[k] = gap;
                }
                maxK = k-1;
            }            
        }
        private class PapernovGapGenerator : GapGenerator
        {            
            public PapernovGapGenerator(int n) : base(n) 
            { 
                maxGap = (N >> 1);

                gaps[0] = 1;

                int k;
                for (k = 1; k < gaps.Length; k++)
                {
                    int gap = (2 << (k - 1)) + 1;
                    if (gap > maxGap)
                    {
                        break;
                    }
                    gaps[k] = gap;
                }
                maxK = k-1;
            }
            
        }

        private class KnuthGapGenerator : GapGenerator
        {

            public KnuthGapGenerator(int n) : base(n) 
            { 
                maxGap = n / 3 + 1;

                int baseGap = 1;

                int k;
                for (k = 0; k < gaps.Length; k++)
                {
                    baseGap *= 3;
                    int gap = (baseGap - 1) >> 1;
                    if (gap > maxGap)
                    {
                        break;
                    }
                    gaps[k] = gap;
                }
                maxK = k - 1;

            }
            
        }

        private class SedgewickGapGenerator : GapGenerator
        {            
            public SedgewickGapGenerator(int n) : base(n) 
            {
                maxGap = (N >> 1);

                gaps[0] = 1;

                int k;
                for (k = 1; k < gaps.Length; k++)
                {
                    int gap = (1 << (k * 2)) + 3 * (1 << (k - 1)) + 1;
                    if (gap > maxGap)
                    {
                        break;
                    }
                    gaps[k] = gap;
                }
                maxK = k - 1;

            }
            
        }

        private class CiuraGapGenerator : GapGenerator
        {
            public CiuraGapGenerator(int n) : base(n) 
            {                
                maxGap = (N >> 1);   
                
                // значения подобраны экспериментально (A102549), опубликованы первые 9
                int[] _originalGaps = new int[] { 1, 4, 10, 23, 57, 132, 301, 701, 1750 };

                int factor = 1;

                int k;
                for (k = 0; k < gaps.Length; k++)
                {
                    int gap;
                    if (k <= 8)
                        gap = _originalGaps[k];
                    else
                    {
                        // более старшие коэффициенты не представлены. приближенно - каждый следующий *2.25 
                        factor = (int)(factor * 2.25);
                        gap = gaps[8] * factor;
                    }                        

                    if (gap > maxGap)
                    {
                        break;
                    }
                    gaps[k] = gap;
                }
                maxK = k - 1;

            }
            
        }
         

      


    }
    
}
