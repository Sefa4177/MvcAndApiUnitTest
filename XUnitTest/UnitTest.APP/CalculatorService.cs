using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.APP
{
    public class CalculatorService : ICalculatorService
    {
        public int Add(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0; 
            }
               
            return a + b;
        }
    }
}
