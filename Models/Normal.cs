using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Normal : HoLee
    {
        public Normal(double mu, double sigma) : base(u => { return mu; }, u => { return sigma; }) { }
    }
}
