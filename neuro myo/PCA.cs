using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuro_myo {
    class PCA {

        private double[,] correlationMatrix;
        private double[] meanVector, standardDeviation;

        public void startPCA() {

            var pca = new PrincipalComponentAnalysis() {
                Method = PrincipalComponentMethod.CorrelationMatrix,
                Means = meanVector,
                StandardDeviations = standardDeviation
            };

            MultivariateLinearRegression transform = pca.Learn();

        }

        public void MeanVector(int numberOfSensors, List<List<double>> rmsData) {

            meanVector = new double[numberOfSensors];
            for (int i = 0; i < rmsData.Count; i++) {
                meanVector[i] = rmsData[i].Sum() / rmsData[i].Count;
            }
        }

        public void StandardDeviation(int numberOfSensors, List<List<double>> rmsData) {

            standardDeviation = new double[numberOfSensors];
            double[] deviationOfEachData;
            for (int i = 0; i < rmsData.Count; i++) {
                deviationOfEachData = new double[rmsData[i].Count];
                for (int j = 0; j < rmsData[i].Count; j++) {
                    deviationOfEachData[j] = Math.Pow((rmsData[i][j] - meanVector[i]), 2);
                }
                standardDeviation[i] = Math.Sqrt((deviationOfEachData.Sum() / deviationOfEachData.Length));
            }
        }

        public void CorrelationMatrix(int numberOfSensors, List<List<double>> rmsData) {

            correlationMatrix = new double[numberOfSensors, numberOfSensors];
            double matrixElement;
            int m;

            for (int i = 0; i < rmsData.Count; i++) {
                for (int j = 0; j < rmsData[i].Count; j++) {
                    rmsData[i][j] -= meanVector[i];
                }
            }

            for (int i = 0; i < rmsData.Count; i++) {
                m = 0;
                for (int j = 0; j < rmsData.Count; j++) {
                    matrixElement = 0;
                    for (int k = 0; k < rmsData[i].Count; k++) {
                        matrixElement += (rmsData[i].ElementAt(k)) * (rmsData[j].ElementAt(k));
                    }
                    matrixElement = matrixElement / (rmsData[i].Count/* - 1*/);
                    correlationMatrix[i, m] = matrixElement;
                    ++m;
                }
            }

            ShowMatrix(correlationMatrix);
        }

        public void ShowMatrix(double[,] matrixResult) {

            for (int i = 0; i < (matrixResult.Length / 8); i++) {
                for (int j = 0; j < (matrixResult.Length / 8); j++) {
                    Console.Write(matrixResult[i, j] + " ");
                }
                Console.WriteLine();
            }
        }


    }
}
