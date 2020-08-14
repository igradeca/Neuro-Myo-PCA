using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neuro_myo {
    class PCA {

        public double[][] eigenVectors;
        public double[] eigenValues;
        public double sumOfEigenValues;
        public double[][] principalComponents;
        public List<List<double>> rmsMinusMean = new List<List<double>>();

        private double[][] correlationMatrix;
        private double[] meanVector, standardDeviation;

        public void startPCA() {

            var pca = new PrincipalComponentAnalysis() {
                Method = PrincipalComponentMethod.CorrelationMatrix,
                Means = meanVector,
                StandardDeviations = standardDeviation                
            };

            MultivariateLinearRegression transform = pca.Learn(correlationMatrix);

            eigenVectors = pca.ComponentVectors;
            eigenValues = pca.Eigenvalues;

            sumOfEigenValues = 0;
            for (int i = 0; i < eigenValues.Length; i++) {
                sumOfEigenValues += eigenValues[i];
            }
        }

        public void MeanVector(int numberOfSensors, List<List<double>> rmsData) {

            meanVector = null;
            meanVector = new double[numberOfSensors];
            for (int i = 0; i < rmsData.Count; i++) {
                meanVector[i] = rmsData[i].Sum() / rmsData[i].Count;
            }
        }

        public void StandardDeviation(int numberOfSensors, List<List<double>> rmsData) {

            standardDeviation = null;
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

            correlationMatrix = null;
            correlationMatrix = new double[numberOfSensors][];

            SetRmsMinusMeanData(rmsData);

            double matrixElement;
            for (int i = 0; i < rmsMinusMean.Count; i++) {
                correlationMatrix[i] = new double[numberOfSensors];
                for (int j = 0; j < rmsMinusMean.Count; j++) {
                    matrixElement = 0;
                    for (int k = 0; k < rmsMinusMean[i].Count; k++) {
                        matrixElement += (rmsMinusMean[i].ElementAt(k)) * (rmsMinusMean[j].ElementAt(k));
                    }
                    matrixElement = matrixElement / (rmsMinusMean[i].Count);
                    correlationMatrix[i][j] = matrixElement;
                }
            }
        }

        public void SetRmsMinusMeanData(List<List<double>> rmsData) {

            rmsMinusMean.Clear();
            rmsMinusMean = new List<List<double>>();

            for (int i = 0; i < rmsData.Count; i++) {
                rmsMinusMean.Add(new List<double>());
                for (int j = 0; j < rmsData[i].Count; j++) {
                    rmsMinusMean[i].Add(rmsData[i][j] - meanVector[i]);
                }
            }
        }

        public void CalculatePrincipalComponents(List<List<double>> rmsData, int[] chosenVectors) {

            if (principalComponents != null) {
                principalComponents = null;
            }

            principalComponents = new double[3][];

            for (int i = 0; i < 3; i++) {
                principalComponents[i] = new double[rmsData[0].Count];
                for (int j = 0; j < rmsData[0].Count; j++) {
                    for (int k = 0; k < rmsData.Count; k++) {
                        int index = chosenVectors[i];
                        principalComponents[i][j] = MatrixMultiplicator(eigenVectors[index], rmsData, j);
                    }
                }
            }
        }

        private double MatrixMultiplicator(double[] eigen, List<List<double>> rmsData, int j) {

            double result = 0;

            for (int i = 0; i < rmsData.Count; i++) {
                result += eigen[i] * rmsData[i][j];
            }
            return result;
        }

        public void MatlabPlotPcaChart(double[][] components) {

            object output = null;
            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Execute(@"cd c:\");
            //matlab.Execute(@"cd e:\Workspaces\FERI\semestar II\Neuro nano in kvantno racunalnistvo\neuro myo");            
            matlab.Feval("PlotPcaChart", 0, out output, components[0], components[1], components[2]);
        }

        public void MatlabPlotEnergiesChart() {

            double[] eigenvaluesEnergies = new double[eigenValues.Length];
            for (int i = 0; i < eigenvaluesEnergies.Length; i++) {
                eigenvaluesEnergies[i] = (eigenValues[i] / sumOfEigenValues) * 100;
            }

            object output = null;
            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Execute(@"cd c:\");
            //matlab.Execute(@"cd e:\Workspaces\FERI\semestar II\Neuro nano in kvantno racunalnistvo\neuro myo");            
            matlab.Feval("PlotEnergiesChart", 0, out output, eigenvaluesEnergies);
        }

        public void CalculatePrincipalComponentsForEachMovement(Movement movement) {

            List<List<double>> movementRms = new List<List<double>>();

            foreach (List<double> rmsRow in movement.rmsData) {
                movementRms.Add(new List<double>());
                foreach (double rmsValue in rmsRow) {
                    movementRms[movementRms.Count - 1].Add((rmsValue - meanVector[movementRms.Count - 1]));
                }
            }
            CalculatePrincipalComponents(movementRms, new int[] { 0, 1, 2 });
            
            for (int i = 0; i < movement.location.Length; i++) {
                movement.location[i] = (principalComponents[i].Sum() / principalComponents[i].Count());
            }            
        }

        public string LastMovementPrincipalComponent(List<List<double>> rmsData, List<Movement> movements) {

            List<List<double>> lastMovementRms = new List<List<double>>();
            int i = 0;
            foreach (List<double> rmsRow in rmsData) {
                lastMovementRms.Add(new List<double>());
                lastMovementRms[i].Add(rmsRow[(rmsRow.Count - 1)] - meanVector[i]);
                ++i;
            }
            CalculatePrincipalComponents(lastMovementRms, new int[] { 0, 1, 2 });
            return DetermineMovement(movements);
        }

        private string DetermineMovement(List<Movement> movements) {

            string movementName = "";
            double shortestDistance = double.MaxValue;
            foreach (Movement movement in movements) {
                double distance = 0;
                for (int i = 0; i < 3; i++) {
                    distance += Math.Pow((principalComponents[i][0] - movement.location[i]), 2);
                }
                distance = Math.Abs(Math.Sqrt(distance));

                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    movementName = movement.name;
                }
            }
            return movementName;
        }


    }
}
