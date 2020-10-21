using ImplicitFiniteDifference;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PDESolver
{
    public static class Helpers
    {
        public static Func<double, double> GetDiracDelta(double x0, Grid grid, double pValue)
        {
            var dx = (grid.XMax - grid.XMin) / (double)grid.StepsX;
            var sigma = dx / (2.0 * Normal.InvCDF(.0, 1.0, .5 * (1.0 + pValue)));
            var direcDelta = GetNormDistr(x0, sigma);

            var integral = .0;

            for (int iStepX = 0; iStepX < grid.StepsX; iStepX++)
                integral += direcDelta(grid.XMin + iStepX * dx) * dx;

            Func<double, double> _direcDelta = x =>
            {
                return direcDelta(x) / integral;
            };

            return _direcDelta;
        }

        public static Func<double, double> GetNormDistr(double mu, double sigma)
        {
            Func<double, double> normDistr = x =>
            {
                return 1.0 / (Math.Sqrt(2.0 * Math.PI) * sigma) * Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma * sigma));
            };

            return normDistr;
        }

        public static double[] SolveTriangularMatrix(double[][] U, double[] q)
        {
            var ell = U.Length;

            var l = new double[ell];
            var u = new double[ell];
            var z = new double[ell];

            u[0] = U[0][0];
            z[0] = U[0][1];
            l[0] = U[1][0] / u[0];

            for (int i = 1; i < ell - 1; i++)
            {
                u[i] = U[i][i] - U[i - 1][i] * U[i][i - 1] / u[i - 1];
                z[i] = U[i][i+1];
                l[i] = U[i + 1][i] / u[i];
            }

            u[ell - 1] = U[ell - 1][ell - 1] - U[ell - 2][ell - 1] * U[ell - 1][ell - 2] / u[ell - 2];

            var y = new double[ell];
            var x = new double[ell];

            y[0] = q[0];

            for (int i = 1; i < ell; i++)
                y[i] = q[i] - U[i][i - 1] * y[i - 1] / u[i - 1];

            x[ell - 1] = y[ell - 1] / u[ell - 1];

            for (int i = ell - 2; i > -1; i--)
                x[i] = (y[i] - U[i][i + 1] * x[i + 1]) / u[i];

            return x;
        }

        public static void WriteToFile(double[][] array, string fileName, double[] t, double[] x)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;

            var fullPath = Path.Combine(new string[] { projectDirectory, "Data", fileName });

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath))
            {
                string outstr = "";

                //header
                for (int j = 0; j < array[0].Length; j++)
                    outstr += ";" + x[j];

                sw.WriteLine(outstr);

                for (int i = 0; i < array.Length; i++)
                {
                    outstr = t[i].ToString();
                    for (int j = 0; j < array[0].Length; j++)
                    {
                        outstr += ";" + array[i][j];
                    }
                    sw.WriteLine(outstr);
                }
            }
        }
    }
}
