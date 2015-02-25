using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class FieldInfo
    {
        public readonly int divisor;
        public readonly int a;
        public readonly int b;

        public FieldInfo(int a, int b, int d)
        {
            this.a = a;
            this.b = b;
            this.divisor = d;
        }
    }
}
