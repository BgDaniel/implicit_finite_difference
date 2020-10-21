using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Vasicek : HullWhite
    {
        public Vasicek(double b, double a, double sigma) 
            : base(u => { return b; }, u => { return a; }, u => { return sigma; }) { }
    }
}
