using ImplicitFiniteDifference;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ModelConfig
    {
        public Model Model { get; }
        public Grid Grid { get; }
        public Func<double, double> P_0 { get; }

        public ModelConfig(Model model, Grid grid, Func<double, double> p_0)
        {
            Model = model;
            Grid = grid;
            P_0 = p_0;
        }
    }
}
