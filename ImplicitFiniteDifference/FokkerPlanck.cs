using MathNet.Numerics.Optimization;
using Models;
using PDESolver;
using System;
using System.Runtime.InteropServices;

namespace ImplicitFiniteDifference
{
    public class FokkerPlanck
    {
        private int m_stepsX;
        private int m_stepsT;
        private double m_xMax;
        private double m_xMin;
        private double m_tMax;
        private double m_dx;
        private double m_dt;
        private double[][] m_density;
        
        // alpha = (dA / dx)^2 + A * d^2A / dx^2 - B_dot
        private double[][] m_alpha;

        // beta = 2 * A * dA / dx * dp / dx
        private double[][] m_beta;

        // gamma = 1/ 2 * A^2
        private double[][] m_gamma;

        private double[][] m_a;
        private double[][] m_b;
        private double[][] m_c;

        // drift
        private Func<double[], double> m_drift;
        private double[][] m_B;
        private double[][] m_dBdx;

        // diffusion
        private Func<double[], double> m_diffusion;
        private double[][] m_A;
        private double[][] m_dAdX;
        private double[][] m_d2AdX2;

        private double[] m_t;
        private double[] m_x;

        public double[] T => m_t;
        public double[] X => m_x;

        public FokkerPlanck(ModelConfig modelConfig,
            Func<double, double> p_upper, Func<double, double> p_lower)
        {
            m_stepsX = modelConfig.Grid.StepsX;
            m_stepsT = modelConfig.Grid.StepsT;
            m_xMin = modelConfig.Grid.XMin;
            m_xMax = modelConfig.Grid.XMax;
            m_tMax = modelConfig.Grid.TMax;
            m_dx = modelConfig.Grid.Dx;
            m_dt = modelConfig.Grid.Dt;

            m_drift = modelConfig.Model.Drift;
            m_diffusion = modelConfig.Model.Diffusion;

            m_density = new double[m_stepsT][];

            m_t = new double[m_stepsT];

            for (int iTime = 0; iTime < m_stepsT; iTime++)
                m_t[iTime] = iTime * m_dt;

            m_x = new double[m_stepsX];

            for (int iStepX = 0; iStepX < m_stepsX; iStepX++)
                m_x[iStepX] = m_xMin + iStepX * m_dx;

            m_A = new double[m_stepsT][];
            m_dAdX = new double[m_stepsT][];
            m_d2AdX2 = new double[m_stepsT][];

            m_B = new double[m_stepsT][];
            m_dBdx = new double[m_stepsT][];

            m_alpha = new double[m_stepsT][];
            m_beta = new double[m_stepsT][];
            m_gamma = new double[m_stepsT][];

            m_a = new double[m_stepsT][];
            m_b = new double[m_stepsT][]; 
            m_c = new double[m_stepsT][];

            for (int iTime = 0; iTime < m_stepsT; iTime++)
            {
                m_density[iTime] = new double[m_stepsX];
                
                m_A[iTime] = new double[m_stepsX+2];
                m_dAdX[iTime] = new double[m_stepsX];
                m_d2AdX2[iTime] = new double[m_stepsX];

                m_B[iTime] = new double[m_stepsX + 2];
                m_dBdx[iTime] = new double[m_stepsX];

                m_alpha[iTime] = new double[m_stepsX];
                m_beta[iTime] = new double[m_stepsX];
                m_gamma[iTime] = new double[m_stepsX];

                m_a[iTime] = new double[m_stepsX];
                m_b[iTime] = new double[m_stepsX];
                m_c[iTime] = new double[m_stepsX];
            }

            for (int iStepX = 0; iStepX < m_stepsX; iStepX++)
                m_density[0][iStepX] = modelConfig.P_0(m_xMin + iStepX * m_dx);

            for (int iTime = 0; iTime < m_stepsT; iTime++)
                m_density[iTime][m_stepsX - 1] = p_upper(iTime * m_dt);

            for (int iTime = 0; iTime < m_stepsT; iTime++)
                m_density[iTime][0] = p_lower(iTime * m_dt);

            for (int iTime = 0; iTime < m_stepsT; iTime++)
            {
                for (int iStepX = 0; iStepX < m_stepsX+2; iStepX++)
                {
                    m_A[iTime][iStepX] = m_diffusion(new double[] { iTime * m_dt, m_xMin + (iStepX - 1) * m_dx });
                    m_B[iTime][iStepX] = m_drift(new double[] { iTime * m_dt, m_xMin + iStepX * m_dx });
                }
            }
        }

        private void ComputeHelperVariables()
        {
            for (int iTime = 0; iTime < m_stepsT; iTime++)
            {
                for (int iStepX = 0; iStepX < m_stepsX; iStepX++)
                {
                    m_dAdX[iTime][iStepX] = (m_A[iTime][iStepX+2] - m_A[iTime][iStepX]) / (2.0 * m_dx);
                    m_d2AdX2[iTime][iStepX] = (m_A[iTime][iStepX + 2] - 2.0 * m_A[iTime][iStepX+1]  + m_A[iTime][iStepX]) / (m_dx * m_dx);
                    m_dBdx[iTime][iStepX] = (m_B[iTime][iStepX + 2] - m_B[iTime][iStepX]) / (2.0 * m_dx);

                    m_alpha[iTime][iStepX] = m_dAdX[iTime][iStepX] * m_dAdX[iTime][iStepX] 
                        + m_A[iTime][iStepX] * m_d2AdX2[iTime][iStepX] - m_dBdx[iTime][iStepX];
                    m_beta[iTime][iStepX] = 2.0 * m_A[iTime][iStepX] * m_dAdX[iTime][iStepX] - m_B[iTime][iStepX];
                    m_gamma[iTime][iStepX] = .5 * m_A[iTime][iStepX] * m_A[iTime][iStepX];

                    m_a[iTime][iStepX] = 1.0 - m_alpha[iTime][iStepX] * m_dt + 2.0 * m_gamma[iTime][iStepX] * m_dt / (m_dx * m_dx);
                    m_b[iTime][iStepX] = - m_beta[iTime][iStepX] * m_dt / (2.0 * m_dx) - m_gamma[iTime][iStepX] * m_dt / (m_dx * m_dx);
                    m_c[iTime][iStepX] = m_beta[iTime][iStepX] * m_dt / (2.0 * m_dx) - m_gamma[iTime][iStepX] * m_dt / (m_dx * m_dx);
                }
            }
        }

        public double[][] Solve()
        {
            ComputeHelperVariables();

            for (int iTime = 1; iTime < m_stepsT; iTime++)
            {
                var M = new double[m_stepsX - 2][];

                for (int iStepX = 0; iStepX < m_stepsX - 2; iStepX++)
                {
                    M[iStepX] = new double[m_stepsX - 2];

                    for (int jStepX = 0; jStepX < m_stepsX - 2; jStepX++)
                    {
                        if (iStepX == jStepX)
                        {
                            M[iStepX][jStepX] = m_a[iTime][iStepX + 1];
                        }
                        else if (iStepX == jStepX + 1)
                        {
                            M[iStepX][jStepX] = m_c[iTime][iStepX + 1];
                        }
                        else if (iStepX == jStepX - 1)
                        {
                            M[iStepX][jStepX] = m_b[iTime][iStepX + 1];
                        }
                    }
                }

                var y = new double[m_stepsX - 2];

                for (int iStepX = 0; iStepX < m_stepsX - 2; iStepX++)
                {
                    y[iStepX] = m_density[iTime - 1][iStepX + 1];

                    if (iStepX == 0)
                    {
                        y[iStepX] -= m_c[iTime][1] * m_density[iTime][0];
                    }
                    else if (iStepX == m_stepsX - 3)
                    {
                        y[iStepX] -= m_b[iTime][m_stepsX - 2] * m_density[iTime][m_stepsX - 1];
                    }
                }

                var sol = Helpers.SolveTriangularMatrix(M, y);

                for (int iStepX = 0; iStepX < m_stepsX - 2; iStepX++)
                {
                    m_density[iTime][iStepX + 1] = sol[iStepX];
                }
            }

            return m_density;
        }
    }
}
