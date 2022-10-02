using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sortings
{
    public class SwapItemsEventArgs :EventArgs
    {
        public readonly int I1;
        public readonly int I2;

        public SwapItemsEventArgs(int i1, int i2)
        {
            I1 = i1;
            I2 = i2;
        }
    }

    public class SwapWithShiftItemsEventArgs : EventArgs
    {
        public readonly int I1;
        public readonly int Count;

        public SwapWithShiftItemsEventArgs(int i1, int count)
        {
            I1 = i1;
            Count = count;
        }
    }

    public class SetToAdditionalArrayEventArgs : EventArgs
    {
        public readonly int IdxFrom;
        public readonly int IdxTo;
        public readonly int Count;

        public SetToAdditionalArrayEventArgs(int idxFrom, int idxTo, int count)
        {
            IdxFrom = idxFrom;
            IdxTo = idxTo;
            Count = count;
        }
    }

    public class SetFromAdditionalArrayEventArgs : EventArgs
    {
        public readonly int IdxFrom;
        public readonly int IdxTo;
        public readonly int Count;

        public SetFromAdditionalArrayEventArgs(int idxFrom, int idxTo, int count)
        {
            IdxFrom = idxFrom;
            IdxTo = idxTo;
            Count = count;
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public readonly string Text;       

        public ProgressEventArgs(string text)
        {
            Text = text;            
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        public readonly string ErrMessage;

        public ErrorEventArgs(string errMessage)
        {
            ErrMessage = errMessage;
        }
    }
}
