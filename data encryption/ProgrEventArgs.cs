using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace data_encryption
{
    public class ProgrEventArgs : EventArgs
    {
        public readonly int Val;
        public readonly int CalcVal;
        public ProgrEventArgs(int c_val, int val)
        {
            Val = val;
            CalcVal = c_val;
        }
    }
}
