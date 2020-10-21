using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CoxIngersollRoss : CoxIngersollRossExtended
    {
         public CoxIngersollRoss(double a, double b, double sigma) 
            : base(u => { return a * b; }, u => { return a; }, u => { return sigma; }) { }
    }
}
