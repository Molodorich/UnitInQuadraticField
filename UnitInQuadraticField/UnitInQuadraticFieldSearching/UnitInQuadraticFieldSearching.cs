using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class UnitInQuadraticFieldSearching
    {
        delegate Int64 CalcNext(Int64 prev, Int64 prevPrev, FieldInfo fieldInfo);

        static CalcNext calcNextFor1 = (prev, prevPrev, fieldInfo) => ((2 * fieldInfo.a + fieldInfo.b) * prev - (fieldInfo.a * fieldInfo.a + fieldInfo.a * fieldInfo.b - (fieldInfo.divisor - 1) * (fieldInfo.b * fieldInfo.b) / 4) * prevPrev);
        static CalcNext calcNextFor2or3 = (prev, prevPrev, fieldInfo) => ((2 * fieldInfo.a) * prev + (-fieldInfo.a * fieldInfo.a + fieldInfo.divisor * (fieldInfo.b * fieldInfo.b)) * prevPrev);

        public static Int64 GetPeriod(FieldInfo fieldInfo, Int64 number)
        {
            var alg = fieldInfo.divisor % 4 == 1 ? calcNextFor1 : calcNextFor2or3;

            Int64 result = 1;
            Int64 cur = fieldInfo.b;
            Int64 prev = 0;

            while (cur != 0)
            {
                var tmp = cur;
                cur = alg(cur, prev, fieldInfo) % number;
                prev = tmp;
                ++result;
            }

            return result;
        }

        public static Int64 GetGeneralPeriod(IEnumerable<Int64> periods){
            Int64 result = 1;

            foreach (Int64 period in periods)
            {
                result = LCM(result, period);
            }            

            return result;
        }

        static Int64 GCD(Int64 a, Int64 b)
        {
            return b != 0 ? GCD(b, a % b) : a;//алгоритм Евклида
        }

        static Int64 LCM(Int64 a, Int64 b)
        {
            return a * b / GCD(a, b);
        }
    }
}
