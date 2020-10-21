using System;

namespace Models
{
    public class BlackScholes : Black76
    {
        public BlackScholes(double mu, double sigma) : base(u => { return mu; }, u => { return sigma; }) { }
    }
}
