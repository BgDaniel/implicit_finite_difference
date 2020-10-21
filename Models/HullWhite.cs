using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class HullWhite : Model
    {
        public HullWhite(Func<double, double> theta, Func<double, double> a, Func<double, double> sigma)
        {
            m_drift = u =>
            {
                var t = u[0];
                var x = u[1];
                return theta(t) - a(t) * x;
            };

            m_diffusion = u =>
            {
                var t = u[0];
                return sigma(t);
            };
        }
    }
}
