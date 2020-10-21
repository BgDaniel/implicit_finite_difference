using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public abstract class Model
    {
        protected Func<double[], double> m_diffusion;
        protected Func<double[], double> m_drift;

        public Func<double[], double> Diffusion => m_diffusion;
        public Func<double[], double> Drift => m_drift;        
    }
}
