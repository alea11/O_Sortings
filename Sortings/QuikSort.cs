using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sortings
{
    public class QuikSort: Sorting
    {
        private CancellationToken _ct;
        private Func<int, int, int> _splitFunk; // функция разделения массива на 2 части, выбирается в зависимости от параметра
        internal QuikSort(int parameter)
        {            
            switch (parameter)
            {
                case 0:
                    _name = "QuikSort_Hoar";
                    _splitFunk = Split_0;                    
                    break;
                case 1:
                    _name = "QuikSort_Lomuto";
                    _splitFunk = Split_1;                    
                    break;
            }  
        }

        public override void Sort(int[] arr, int range = 0, CancellationToken ct = default(CancellationToken))
        {
            _ct = ct;
            try 
            {        
                Prepare(arr);                      
                QSort(0, N - 1);
            }
            catch(Exception exc)
            {
                ErrMessage = exc.Message;
            }
        }

        void QSort(int L, int R)
        {
            if (_ct.IsCancellationRequested)
                return;

            // делим массив на 2 части (с переносом элементов) - элементы <= p и элементы >p. 
            // m - конец первой части
            int m = _splitFunk(L, R); 

            // рекурсивно каждый из 2-х подмассивов (если еще не выродился в 1 элемент) делим дальше
            if (L < m-1)
                QSort(L, m-1);
            if (m+1 < R)
                QSort(m+1, R);
        }

        
        // Разбиение Хоара (+   с фиксацией опорного элемента )
        int Split_0(int L, int R)
        {
            // L и R - левая и правая границы массива 

            int p = Arr[L]; // опорный элемент - первый           

            // 3 части массива: 1 - элементы <= опорного; 2 - пока неотсортированная часть; 3 - элементы > опорного
            // 2 индекса отделяют левый и правый подмассивы от неотсортированной части, движутся слева и справа навстречу друг другу.
            // каждый из них доходит до элемента, который не соответствует условию своего подмассива. эти элементы обмениваем.
            // цикл повторяется пока эти 2 индекса не сойдутся
            
            int i = L;
            int j = R+1;
            while (true)
            {
                if (_ct.IsCancellationRequested)
                    break;
                while (++i < j      && lessOrEqualValue(i, p))
                {}
                while (i <= --j     && moreValue(j, p))
                {}
                
                if (i >= j) break;
                swap(i, j); // обмен 2х найденных элементов - в свои подмассивы
            }

            // переносим опорный элемент на свое место
            swap(L, j); 

            return j;
        }
        
        // Разбиение Ломуто
        int Split_1(int L, int R)  
        {
            // L и R - левая и правая границы массива

            int p = Arr[R]; // опорный элемент - последний

            // 3 части массива: 1 - элементы <= опорного; 2 - элементы > опорного; 3 - пока неотсортированная часть
            
            int m = L - 1;  // переменная указывает на границу первой части (в которой накапливаются элементы <= p)
            for (int i = L; i <= R; i++) // переменная цикла указывает на начало необработанной части
            {
                if (_ct.IsCancellationRequested)
                    break;
                if ( lessOrEqualValue(i, p)   &&   ++m < i) // передвигаем границу первой части если выполняется первое условие
                    swap(m, i);                 // и обмениваем если выполняются оба условия

                // можно упростить условие, убрать '&& +m < i' (и иногда обменивать элементы сами с собой) - т.к. оно не выполняется только для первых нескольких элементов, пока не повстречался элемент >p
                // однако без этого условия будем излишне обменивать при сортировке уже отсортированного массива
                //if (lessOrEqualValue(i, p)) 
                //    swap(++m, i);                // передвигаем границу первой части и обмениваем если выполняются оба условия
                
            }
            return m;
        }


        
    }
    
}
