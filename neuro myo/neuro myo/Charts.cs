using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuro_myo {
    class Charts {

        private MainForm mainForm;

        public Charts(MainForm mainForm) {

            this.mainForm = mainForm;
            SetEmgAndRmsCharts();
            SetOrientationChart();
            SetRmsColumnChart();
        }

        public void SetChartScrollPanels() {
            foreach (var chartArea in mainForm.chart1.ChartAreas) {
                chartArea.AxisX.ScaleView.Zoom(-500, 500);
                chartArea.AxisX.Maximum = (mainForm.chart1.Series[0].Points.Count);
            }
        }

        private void SetEmgAndRmsCharts() {

            for (int i = 0; i < mainForm.numberOfSensors; i++) {
                mainForm.chart1.Series[i].Points.Clear();
                mainForm.chart1.Series[mainForm.numberOfSensors + i].Points.Clear();

                mainForm.chart1.ChartAreas[i].AxisY.Maximum = 150;
                mainForm.chart1.ChartAreas[i].AxisY.Minimum = -150;

                mainForm.chart1.ChartAreas[i].AxisX.Maximum = 500;
                mainForm.chart1.ChartAreas[i].AxisX.Minimum = 0;

                for (int j = 0; j < 500; j++) {
                    mainForm.chart1.Series[i].Points.AddY(0);
                    mainForm.chart1.Series[mainForm.numberOfSensors + i].Points.AddY(0);
                }
            }
        }

        private void SetOrientationChart() {

            mainForm.chart1.ChartAreas[mainForm.numberOfSensors].AxisY.Maximum = 1.5;
            mainForm.chart1.ChartAreas[mainForm.numberOfSensors].AxisY.Minimum = -1.5;

            mainForm.chart1.ChartAreas[mainForm.numberOfSensors].AxisX.Maximum = 500;
            mainForm.chart1.ChartAreas[mainForm.numberOfSensors].AxisX.Minimum = 0;

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 500; j++) {
                    mainForm.chart1.Series[(mainForm.numberOfSensors * 2) + i].Points.AddY(0);
                }
            }
        }

        private void SetRmsColumnChart() {

            mainForm.chart2.ChartAreas[0].AxisY.Maximum = 120;
            mainForm.chart2.ChartAreas[0].AxisY.Minimum = 0;
            for (int i = 0; i < mainForm.numberOfSensors; i++) {
                mainForm.chart2.Series[0].Points.AddXY(0, 75);
            }
        }

        public void UpdateEmg() {

            for (int j = 0; j < mainForm.numberOfSensors; j++) {
                for (int i = 0; i < mainForm.myoEMG[j].Count; i++) {
                    mainForm.chart1.Series[j].Points.RemoveAt(0);
                    mainForm.chart1.Series[j].Points.AddY(mainForm.myoEMG[j].ElementAt(i));
                }
            }
        }

        public void UpdateRmsLineChart() {

            for (int i = 0; i < mainForm.numberOfSensors; i++) {
                mainForm.chart1.Series[i + 8].Points.RemoveAt(0);
                mainForm.chart1.Series[i + 8].Points.AddY(mainForm.rmsData[i].ElementAt(mainForm.rmsData[i].Count - 1));
            }
        }

        public void UpdateRmsColumnChart() {

            double chartDot;
            mainForm.chart2.Series[0].Points.Clear();
            for (int i = 0; i < mainForm.numberOfSensors; i++) {                
                chartDot = mainForm.rmsData[i].ElementAt(mainForm.rmsData[i].Count - 1);
                mainForm.chart2.Series[0].Points.AddXY(0, chartDot);
            }
        }

        public void UpdateOrientation() {

            for (int j = 0; j < 4; j++) {
                for (int i = 0; i < mainForm.myoOrientation[j].Count; i++) {
                    mainForm.chart1.Series[j + 16].Points.RemoveAt(0);
                    mainForm.chart1.Series[j + 16].Points.AddY(mainForm.myoOrientation[j].ElementAt(i));
                }
            }
        }


    }
}
