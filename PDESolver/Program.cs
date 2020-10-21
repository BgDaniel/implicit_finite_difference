using ExampleFactory;
using ImplicitFiniteDifference;
using System;

namespace PDESolver
{
    class Program
    {
        static void Main(string[] args)
        {
            // var modelConfig = Examples.Normal1();
            var modelConfig = Examples.BlackScholes1();

            Func<double, double> p_upper = x0 => { return .0; };
            Func<double, double> p_lower = x0 => { return .0; };

            var fokkerPlanck = new FokkerPlanck(modelConfig, p_upper, p_lower);

            var p = fokkerPlanck.Solve();

            Helpers.WriteToFile(p, String.Format("density_{0}.csv", modelConfig.Name), fokkerPlanck.T, fokkerPlanck.X);
        }
    }
}
