using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class HoLee : Model
    {
        public HoLee(Func<double, double> theta, Func<double, double> sigma)
        {
            m_drift = u =>
            {
                var t = u[0];
                return theta(t);
            };

            m_diffusion = u =>
            {
                var t = u[0];
                return sigma(t);
            };
        }
    }
}
