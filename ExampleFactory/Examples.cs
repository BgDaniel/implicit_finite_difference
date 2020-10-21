using ImplicitFiniteDifference;
using Models;
using PDESolver;
using System;

namespace ExampleFactory
{
    public static class Examples
    {
        public static ModelConfig Normal1()
        {
            var stepsX = 1000;
            var stepsT = 750;
            var xMin = -4.0;
            var xMax = +4.0;
            var tMax = 4.0;

            var grid = new Grid(stepsX, stepsT, xMin, xMax, tMax);

            var sigma = .35;
            var mu = .2;
            var normalModel = new Normal(mu, sigma);

            var x0 = .2;
            var p_0 = Helpers.GetDiracDelta(x0, grid, .99);

            var integral0 = .0;
            var dx = (xMax - xMin) / (double)stepsX;

            for (int iStepX = 0; iStepX < stepsX; iStepX++)
                integral0 += p_0(xMin + iStepX * dx) * dx;
            
            return new ModelConfig(normalModel, grid, p_0);
        }

        public static ModelConfig BlackScholes1()
        {
            var stepsX = 1000;
            var stepsT = 750;
            var xMin = .001;
            var xMax = +6.0;
            var tMax = 4.0;

            var grid = new Grid(stepsX, stepsT, xMin, xMax, tMax);

            var sigma = .35;
            var mu = .2;
            var blackScholes = new BlackScholes(mu, sigma);

            var x0 = .9;
            var p_0 = Helpers.GetDiracDelta(x0, grid, .99);

            var integral0 = .0;
            var dx = (xMax - xMin) / (double)stepsX;

            for (int iStepX = 0; iStepX < stepsX; iStepX++)
                integral0 += p_0(xMin + iStepX * dx) * dx;

            return new ModelConfig(blackScholes, grid, p_0);
        }
    }
}
