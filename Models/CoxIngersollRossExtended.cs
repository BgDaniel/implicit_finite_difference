using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CoxIngersollRossExtended : Model
    {
        public CoxIngersollRossExtended(Func<double, double> theta, Func<double, double> alpha,
            Func<double, double> sigma)
        {
            m_drift = u =>
            {
                var t = u[0];
                var x = u[1];
                return theta(t) - alpha(t) * x;
            };

            m_diffusion = u =>
            {
                var t = u[0];
                var x = u[1];
                return sigma(t) * Math.Sqrt(x);
            };
        }
    }
}
