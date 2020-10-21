using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Black76 : Model
    {
        public Black76(Func<double, double> theta, Func<double, double> sigma)
        {
            m_drift = u =>
            {
                var t = u[0];
                var x = u[1];
                return theta(t) * x;
            };

            m_diffusion = u =>
            {
                var t = u[0];
                var x = u[1];
                return sigma(t) * x;
            };
        }
    }
}
