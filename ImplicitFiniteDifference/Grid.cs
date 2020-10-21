using System;
using System.Collections.Generic;
using System.Text;

namespace ImplicitFiniteDifference
{
    public class Grid
    {
        public int StepsX { get; }
        public int StepsT { get; }
        public double XMin { get; }
        public double XMax { get; }
        public double TMax { get; }
        public double Dx { get; }
        public double Dt { get; }

        public Grid(int stepsX, int stepsT, double xMin, double xMax, double tMax)
        {
            StepsX = stepsX;
            StepsT = stepsT;
            XMin = xMin;
            XMax = xMax;
            TMax = tMax;
            Dx = (xMax - xMin) / (double)stepsX;
            Dt = tMax / (double)stepsT;
        }
    }
}
