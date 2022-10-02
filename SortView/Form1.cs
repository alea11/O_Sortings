using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sortings;

namespace SortView
{
    public partial class Form1 : Form
    {
        private delegate void UpdatestatisticDelegate(string text);
        private UpdatestatisticDelegate _updatestatisticDelegate;

        private CancellationTokenSource _cts;

        Thread _actionThread = null;


        private Sorting _sort = null;
        private int[] _Arr;

        int _delay = 0;
        public Form1()
        {
            InitializeComponent();

            _updatestatisticDelegate = new UpdatestatisticDelegate(Updatestatistic);
        }

        enum ePresort
        {
            Random,
            Sorted,
            ReverseSorted
        }


        private static int[] CreateArray(int size, int max, ePresort presort)
        {
            int[] a = new int[size];

            if(presort == ePresort.Random)
            {
                Random random = new Random(12345);
                for (int i = 0; i < size; i++)
                    a[i] = random.Next(max);
            }
            else
            {
                float k = (float)max / size;

                if(presort == ePresort.Sorted)
                {
                    for (int i = 0; i < size; i++)
                        a[i] = (int)(i *k);
                }
                else
                {
                    for (int i = 0; i < size; i++)
                        a[i] = (int)((size - 1 - i)  * k);
                }

            }            

            return a;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            int len;
            if (!int.TryParse(txtLen.Text, out len)) 
            {
                MessageBox.Show("Задайте размер массива");
                return;
            }

            
            if (!int.TryParse(txtRange.Text, out _range))
            {
                MessageBox.Show("Задайте диапазон (макс. число)");
                return;
            }

            ePresort presort = (ePresort)cmbPresort.SelectedItem;

            _Arr = CreateArray(len, _range, presort);            

            lblResult.Text = "";
            SetSpeed();
            CalcScale();
            Display();
        }

        

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (_Arr == null)
            {
                MessageBox.Show("Задайте размер массива и создайте его (кнопка 'Create')");
                return;
            }
            
            if (_sort != null)
            {
                _sort.OnSwap -= _sort_OnSwap;
                _sort.OnSwapWithShift -= _sort_OnSwapWithShift;
                _sort.BeforeSwap -= _sort_BeforeSwap;
                _sort.OnSetFromOther -= _sort_OnSetFromAdditionalArray;
                _sort.OnSetToOther -= _sort_OnSetToAdditionalArray; 
                _sort.OnProgress -= _sort_Progress;
                _sort.OnError -= _sort_OnError;
            }
            if (cmbAlgorithm.SelectedItem == null)
            {
                MessageBox.Show("Выберите алгоритм!");
                return;
            }

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;           

            _sort = Sorting.CreateSorting((Sorting.eAlgorithm)cmbAlgorithm.SelectedItem);
            lblResult.Text = $"{_sort.Name}: ";
            _sort.OnSwap += _sort_OnSwap;
            _sort.OnSwapWithShift += _sort_OnSwapWithShift;
            _sort.BeforeSwap += _sort_BeforeSwap;
            _sort.OnSetFromOther += _sort_OnSetFromAdditionalArray;
            _sort.OnSetToOther += _sort_OnSetToAdditionalArray;
            _sort.OnProgress += _sort_Progress; 
            _sort.OnError += _sort_OnError;

            Action<int[], int, CancellationToken> act = _sort.Sort;            
            
            _actionThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                act.Invoke(_Arr, _range, ct);
                sw.Stop();
                if(!this.IsDisposed)
                    this.Invoke(_updatestatisticDelegate, $" , time: {sw.ElapsedMilliseconds}");
            });
            _actionThread.Start();           

        }

        private void _sort_OnError(object sender, ErrorEventArgs e)
        {
            if (!this.IsDisposed)
                this.Invoke(_updatestatisticDelegate, e.ErrMessage);
        }

        private void _sort_Progress(object sender, ProgressEventArgs e)
        {
            if (!this.IsDisposed)
                this.Invoke(_updatestatisticDelegate, e.Text);
            Display();
        }

        private void _sort_OnSetToAdditionalArray(object sender, SetToAdditionalArrayEventArgs e)
        {
            DisplaySetTo(e.IdxFrom); //, e.IdxTo, e.Count
        }

        private void _sort_OnSetFromAdditionalArray(object sender, SetFromAdditionalArrayEventArgs e)
        {
            //DisplaySetFrom(e.IdxFrom, e.IdxTo, e.Count);
            DisplayInterval(e.IdxTo, e.IdxTo + e.Count -1);


        }

        private void Updatestatistic (string text)
        {            
            lblResult.Text += Environment.NewLine + text;
        }


        private void _sort_OnSwap(object sender, SwapItemsEventArgs e)
        {
            if(!this.IsDisposed)
                DisplaySwap(e.I1, e.I2);
        }

        private void _sort_OnSwapWithShift(object sender, SwapWithShiftItemsEventArgs e)
        {
            if (!this.IsDisposed)
                DisplayInterval(e.I1 - e.Count +1, e.I1);
        }

        private void _sort_BeforeSwap(object sender, SwapItemsEventArgs e)
        {
            if (!this.IsDisposed)
                DisplayBeforeSwap(e.I1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var item in Enum.GetValues(typeof(Sorting.eAlgorithm)))
            {
                cmbAlgorithm.Items.Add(item);
            }

            foreach (var item in Enum.GetValues(typeof(ePresort)))
            {
                cmbPresort.Items.Add(item);
            }
            cmbPresort.SelectedIndex = 0;
            sbSpeed.Value = 100;
            chkRange.Checked = true;
        }


        private double _dx;
        private double _dy;
        private float _y0;
        private int _n;
        private int _range;

        private void CalcScale()
        {
            _n = _Arr.Length;
            _dx = (double)(panel1.Width - 10) / (_n - 1);
            _dy = (double)(panel1.Height - 10) / (_range - 1);
            _y0 = panel1.Height - 5;
        }

        private void Display()
        {
            if (_Arr == null)
                return;

            using (var g = panel1.CreateGraphics())
            {
                g.Clear(Color.White);                

                using (var pen = new Pen(Color.Green, 2))
                {                    
                    for (uint i = 0; i < _n; i++)
                    {
                        float y = panel1.Height -5- (float)(_dy * (_Arr[i] ));
                        float x = (float)_dx * i +5;
                        g.DrawLine(pen, x, _y0, x, y);
                    }                    
                }
            }

        }

        private void DisplayBeforeSwap(int i)
        {
            if (_Arr == null)
                return;            

            using (var g = panel1.CreateGraphics())
            {
                float y1 = panel1.Height - 5 - (float)(_dy * (_Arr[i]));
                float x1 = (float)_dx * i + 5;
                
                using (var pen = new Pen(Color.Blue, 2))
                {
                    g.DrawLine(pen, x1, _y0, x1, y1);
                }

                Thread.Sleep(_delay);
                
            }

        }

        private void DisplaySwap(int i1, int i2)
        {
            if (_Arr == null)
                return;            

            using (var g = panel1.CreateGraphics())
            {
                float y1 = panel1.Height - 5 - (float)(_dy * (_Arr[i1]));
                float x1 = (float)_dx * i1 + 5;
                float y2 = panel1.Height - 5 - (float)(_dy * (_Arr[i2]));
                float x2 = (float)_dx * i2 + 5;

                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawLine(pen, x1, 0, x1, panel1.Height);
                    g.DrawLine(pen, x2, 0, x2, panel1.Height);
                }

                using (var pen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(pen, x1, _y0, x1, y1);
                    g.DrawLine(pen, x2, _y0, x2, y2);                    
                }

                Thread.Sleep(_delay);

                using (var pen = new Pen(Color.Green, 2))
                {
                    g.DrawLine(pen, x1, _y0, x1, y1);
                    g.DrawLine(pen, x2, _y0, x2, y2);                    
                }
            }

        }

        

        private void DisplaySetTo(int idxFrom) //, int idxTo , int count
        {
            if (_Arr == null)
                return;

            using (var g = panel1.CreateGraphics())
            {
                float y1 = panel1.Height - 5 - (float)(_dy * (_Arr[idxFrom]));
                float x1 = (float)_dx * idxFrom + 5;
                //float y2 = panel1.Height - 5 - (float)(_dy * (_Arr[i2]));
                //float x2 = (float)_dx * i2 + 5;
                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawLine(pen, x1, 0, x1, panel1.Height);
                    //
                }
                using (var pen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(pen, x1, _y0, x1, y1);
                    //g.DrawLine(pen, x2, _y0, x2, y2);
                }
                Thread.Sleep(_delay);
                using (var pen = new Pen(Color.Green, 2))
                {
                    g.DrawLine(pen, x1, _y0, x1, y1);
                    //g.DrawLine(pen, x2, _y0, x2, y2);
                }
            }
            
        }

        //private void DisplayInterval(int i1, int count)
        //{
        //    if (_Arr == null)
        //        return;            

        //    using (var g = panel1.CreateGraphics())
        //    {
        //        int exeptIdx = i1 - count;

        //        using (var pen = new Pen(Color.White, 2))
        //        {
        //            for (int i = exeptIdx+1; i <= i1; i++)
        //            {                        
        //                float x = (float)_dx * i + 5;
        //                g.DrawLine(pen, x, 0, x, panel1.Height);
        //            }                   
        //        }

        //        using (var pen = new Pen(Color.Red, 2))
        //        {
        //            for (int i = exeptIdx + 1; i <= i1; i++)
        //            {
        //                float y = panel1.Height - 5 - (float)(_dy * (_Arr[i]));
        //                float x = (float)_dx * i + 5;
        //                g.DrawLine(pen, x, _y0, x, y);
        //            }
        //        }

        //        Thread.Sleep(_delay);

        //        using (var pen = new Pen(Color.Green, 2))
        //        {
        //            for (int i = exeptIdx + 1; i <= i1; i++)
        //            {
        //                float y = panel1.Height - 5 - (float)(_dy * (_Arr[i]));
        //                float x = (float)_dx * i + 5;
        //                g.DrawLine(pen, x, _y0, x, y);
        //            }
        //        }

        //    }

        //}

        private void DisplayInterval(int begin, int end)  // exeptIdx + 1, i1 , int exeptIdx = i1 - count;
        {
            if (_Arr == null)
                return;

            using (var g = panel1.CreateGraphics())
            {
                using (var pen = new Pen(Color.White, 2))
                {
                    for (int i = begin; i <= end; i++)
                    {
                        float x = (float)_dx * i + 5;
                        g.DrawLine(pen, x, 0, x, panel1.Height);
                    }
                }

                using (var pen = new Pen(Color.Red, 2))
                {
                    for (int i = begin; i <= end; i++)
                    {
                        float y = panel1.Height - 5 - (float)(_dy * (_Arr[i]));
                        float x = (float)_dx * i + 5;
                        g.DrawLine(pen, x, _y0, x, y);
                    }
                }

                Thread.Sleep(_delay);

                using (var pen = new Pen(Color.Green, 2))
                {
                    for (int i = begin; i <= end; i++)
                    {
                        float y = panel1.Height - 5 - (float)(_dy * (_Arr[i]));
                        float x = (float)_dx * i + 5;
                        g.DrawLine(pen, x, _y0, x, y);
                    }
                }

            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CalcScale();
            Display();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
            //if (_actionThread != null)
            //    _actionThread.Abort();
        }

        private void sbSpeed_Scroll(object sender, ScrollEventArgs e)
        {
            SetSpeed();
        }

        private void SetSpeed()
        {
            int d = sbSpeed.Maximum - sbSpeed.Value;
            _delay = d * d /20;
        }

        private void chkRange_CheckedChanged(object sender, EventArgs e)
        {
            txtRange.Enabled = !chkRange.Checked;
            if (chkRange.Checked)
                txtRange.Text = txtLen.Text;
        }

        private void txtLen_TextChanged(object sender, EventArgs e)
        {
            if (chkRange.Checked)
                txtRange.Text = txtLen.Text;
        }
    }
}
