using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDESolver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class MathHelperTests
    {
        public const int NUMBER_SAMPLES = 10;
        public static int DIM = 1000;
        public static double ENTRY_MAX = +100.0;
        public static double ENTRY_MIN = -100.0;
        public static double EPSILON = 10e-9;

        [TestMethod]
        public void TestUpperTriangularInversion()
        {
            var randomUpperTriangularMatrices = GenerateRandomUpperTriangularMatrices(DIM, NUMBER_SAMPLES);
            var randomVectors = GenerateRandomVectors(DIM, NUMBER_SAMPLES);

            for (int i = 0; i < NUMBER_SAMPLES; i++)
            {
                var sol = Helpers.SolveTriangularMatrix(randomUpperTriangularMatrices[i], randomVectors[i]);

                var x = new double[DIM];

                for (int k = 0; k < DIM; k++)
                {
                    for (int l = 0; l < DIM; l++)
                        x[k] += randomUpperTriangularMatrices[i][k][l] * sol[l];

                    if (Math.Abs(x[k] - randomVectors[i][k]) > EPSILON)
                        throw new Exception("Deviation to high!");
                }
            }
        }

        private List<double[]> GenerateRandomVectors(int dim, int nbSamples)
        {
            var retValues = new List<double[]>();
            var rnd = new Random();

            for (int i = 0; i < nbSamples; i++)
            {
                var vector = new double[dim];

                for (int j = 0; j < dim; j++)
                    vector[j] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();

                retValues.Add(vector);

            }

            return retValues;
        }

        private List<double[][]> GenerateRandomUpperTriangularMatrices(int dim, int nbSamples)
        {
            var retValues = new List<double[][]>();
            var rnd = new Random();

            for (int i = 0; i < nbSamples; i++)
            {
                var upperTriangularMatrixArray = new double[dim, dim];
                var isInvertible = false;
                Matrix<double> upperTriangularMatrix = null;

                while (!isInvertible)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        if (j == 0)
                        {
                            upperTriangularMatrixArray[0, 0] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                            upperTriangularMatrixArray[0, 1] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                        }
                        else if (j == dim - 1)
                        {
                            upperTriangularMatrixArray[dim - 1, dim - 2] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                            upperTriangularMatrixArray[dim - 1, dim - 1] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                        }
                        else
                        {
                            upperTriangularMatrixArray[j, j - 1] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                            upperTriangularMatrixArray[j, j] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                            upperTriangularMatrixArray[j, j + 1] = ENTRY_MIN + (ENTRY_MAX - ENTRY_MIN) * rnd.NextDouble();
                        }
                    }

                    upperTriangularMatrix = Matrix<double>.Build.DenseOfArray(upperTriangularMatrixArray);

                    if (Math.Abs(upperTriangularMatrix.Determinant()) > EPSILON)
                    {
                        isInvertible = true;
                        retValues.Add(Enumerable.Range(0, dim).Select(i => Enumerable.Range(0, dim).Select(j => upperTriangularMatrixArray[i, j]).ToArray()).ToArray());
                    }
                }
            }

            return retValues;
        }
    }
}