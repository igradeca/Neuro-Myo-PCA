using Accord.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neuro_myo {
    public partial class MainForm : Form {

        public int numberOfSensors = 8;

        public List<List<int>> myoEMG;
        public List<List<double>> myoOrientation;
        public List<List<double>> rmsData;
        public List<Movement> movements = new List<Movement>();
        public Myo myoArmband;
        
        private FileReadAndWrite fromFiles;
        private Charts chartsDrawing;
        private PCA pca;
        private Thread emgThread, rmsThread, orientationThread;
        private bool EVComboBoxChangeEnabled = false;

        public string movement;

        public MainForm() {
            InitializeComponent();            
        }

        private void MainForm_Load(object sender, EventArgs e) {
            
            fromFiles = new FileReadAndWrite(this);
            chartsDrawing = new Charts(this);
            pca = new PCA();

            SetUpArraysForMyo();
        }

        private void InitializeThreads() {

            emgThread = new Thread(new ThreadStart(EmgThreadsWork));
            rmsThread = new Thread(new ThreadStart(RmsThreadsWork));
            orientationThread = new Thread(new ThreadStart(OrientationThreadWork));

            emgThread.IsBackground = true;
            rmsThread.IsBackground = true;
            orientationThread.IsBackground = true;
        }

        private void StartThreads() {

            emgThread.Start();
            rmsThread.Start();
            orientationThread.Start();     
        }

        public void SetUpArraysForMyo() {

            if ((myoEMG != null) || (myoOrientation != null) || (rmsData != null)) {
                myoEMG.Clear();
                myoOrientation.Clear();
                rmsData.Clear();
            }

            myoEMG = new List<List<int>>();
            myoOrientation = new List<List<double>>();
            rmsData = new List<List<double>>();

            for (int i = 0; i < numberOfSensors; i++) {
                myoEMG.Add(new List<int>());
                rmsData.Add(new List<double>());
            }

            for (int i = 0; i < 4; i++) {
                myoOrientation.Add(new List<double>());
            }
        }

        private void EmgThreadsWork() {

            while (myoArmband.myoIsWorking) {
                Thread.Sleep(25);
                if (EMGCheckBox.Checked == true) {
                    InvokeSensorData();
                }                
            }
        }

        private void RmsThreadsWork() {

            while (myoArmband.myoIsWorking) {
                Thread.Sleep(250);
                if (myoEMG[0].Count >= 50) {
                    CalulateRootMeanSquareForEMG();
                    if (movements.Count > 0) {
                        movement = pca.LastMovementPrincipalComponent(rmsData, movements);
                    }                    
                    InvokeMovementText();
                    if (RMSCheckBox.Checked == true) {
                        InvokeRmsData();
                    }
                }                             
            }
        }

        private void OrientationThreadWork() {

            while (myoArmband.myoIsWorking) {
                Thread.Sleep(25);
                if (OrientationCheckBox.Checked == true) {
                    InvokeOrientationData();
                }
            }
        }

        private void InvokeRmsData() {

            if (InvokeRequired) {
                Invoke(new Action(InvokeRmsData));
                return;
            }
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPageChart"]) {
                chartsDrawing.UpdateRmsLineChart();
            } else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPagePCA"]) {
                chartsDrawing.UpdateRmsColumnChart();
            }            
        }

        private void InvokeMovementText() {
            if (InvokeRequired) {
                Invoke(new Action(InvokeMovementText));
                return;
            }
            movementLabel.Text = movement;
        }

        public void CalulateRootMeanSquareForEMG() {

            double elementVal;
            for (int i = 0; i < numberOfSensors; i++) {
                elementVal = 0;
                int k = 0;
                for (int j = (myoEMG[i].Count - 1); j >= (myoEMG[i].Count - 50); j--) {
                    elementVal += Math.Pow(myoEMG[i].ElementAt(j), 2);
                    ++k;
                }
                elementVal /= 50;
                rmsData[i].Add(Math.Sqrt(elementVal));
            }
        }

        private void InvokeSensorData() {
            if (InvokeRequired) {
                Invoke(new Action(InvokeSensorData));
                return;
            }
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPageChart"]) {
                chartsDrawing.UpdateEmg();
            }            
        }

        private void InvokeOrientationData() {
            if (InvokeRequired) {
                Invoke(new Action(InvokeOrientationData));
                return;
            }
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPageChart"]) {
                chartsDrawing.UpdateOrientation();
            }            
        }
                
        private void openMyoDataToolStripMenuItem_Click(object sender, EventArgs e) {
            
            fromFiles.BinaryRead();
            chartsDrawing.SetChartScrollPanels();
        }

        private void saveMyoDataToolStripMenuItem_Click(object sender, EventArgs e) {

            fromFiles.BinaryWrite();
        }

        private void fromFileDataToolStripMenuItem_Click(object sender, EventArgs e) {

            CalculatePCA();
            SetEigenVectors();
            SetEigenValuesLabels();

        }

        private void fromMovementsToolStripMenuItem_Click(object sender, EventArgs e) {

            GetRmsDataFromMovements();

            CalculatePCA();
            SetEigenVectors();

            pca.CalculatePrincipalComponents(pca.rmsMinusMean, new int[] {0, 1, 2});

            foreach (Movement movement in movements) {
                pca.CalculatePrincipalComponentsForEachMovement(movement);
            }
        }
        
        private void GetRmsDataFromMovements() {

            SetUpArraysForMyo();
            foreach (Movement movement in movements) {
                for (int i = 0; i < numberOfSensors; i++) {
                    for (int j = 0; j < movement.rmsData[i].Count; j++) {
                        rmsData[i].Add(movement.rmsData[i][j]);
                    }
                }
            }
        }

        private void CalculatePCA() {
            StatusLabel.Text = "PCA is calculating...";
            pca.MeanVector(numberOfSensors, rmsData);
            pca.StandardDeviation(numberOfSensors, rmsData);
            pca.CorrelationMatrix(numberOfSensors, rmsData);
            pca.startPCA();            
            StatusLabel.Text = "PCA calculated succesfully.";
        }

        private void SetEigenVectors() {

            EVComboBoxChangeEnabled = false;
            ComboBox[] EVComboBox = {EV1comboBox, EV2comboBox, EV3comboBox};

            for (int i = 0; i < EVComboBox.Length; i++) {
                EVComboBox[i].Items.Clear();
                for (int j = 0; j < pca.eigenVectors.Length; j++) {
                    EVComboBox[i].Items.Add("EV " + (j+1));
                }
                EVComboBox[i].SelectedIndex = i;
            }
            EVComboBoxChangeEnabled = true;
        }

        private void SetEigenValuesLabels() {

            Label[] eigenValues = {
                energyLabel1, energyLabel2, energyLabel3, energyLabel4,
                energyLabel5, energyLabel6, energyLabel7, energyLabel8
            };

            string labelText = "";
            double energyOfEachValue;

            for (int i = 0; i < eigenValues.Length; i++) {
                labelText = (i + 1) + ": ";
                energyOfEachValue = (pca.eigenValues[i] / pca.sumOfEigenValues) * 100;
                energyOfEachValue = Math.Round(energyOfEachValue, 2);

                labelText += energyOfEachValue + " %";
                eigenValues[i].Text = labelText;
            }

            double energyOfSelectedVectors = 0;
            for (int i = 0; i < 3; i++) {
                energyOfSelectedVectors += pca.eigenValues[i];
            }
            energyOfSelectedVectors = (energyOfSelectedVectors / pca.sumOfEigenValues) * 100;
            energyOfSelectedVectors = Math.Round(energyOfSelectedVectors, 2);
            totalEnergyLabel.Text = energyOfSelectedVectors + " %";
        }

        private void setMovementsToolStripMenuItem_Click(object sender, EventArgs e) {

            SetPCAForm setMovements = new SetPCAForm(this, myoArmband);
            setMovements.Show();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e) {

            SetUpArraysForMyo();
            CreateMyoArmbandObject();            
            myoArmband.StartMyo();
            InitializeThreads();
            StartThreads();

            disconnectToolStripMenuItem.Enabled = true;
            connectToolStripMenuItem.Enabled = false;
        }

        public void CreateMyoArmbandObject() {

            myoArmband = null;

            if (myoArmband == null) {
                myoArmband = new Myo(this);
                myoArmband.InitializeMyo();
            }            
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e) {

            if (myoArmband != null) {
                myoArmband.StopMyo();
                connectToolStripMenuItem.Enabled = true;
                disconnectToolStripMenuItem.Enabled = false;
            }                        
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {

            StopMyoIfInstantiated();
            Close();
        }

        private void calculateComponentsButton_Click(object sender, EventArgs e) {

            CalculatePCsWithChosenEigenVectors();
            pca.MatlabPlotPcaChart(pca.principalComponents);
        }

        public void CalculatePCsWithChosenEigenVectors() {

            int[] chosenVectors = {
                EV1comboBox.SelectedIndex,
                EV2comboBox.SelectedIndex,
                EV3comboBox.SelectedIndex,
            };
            pca.CalculatePrincipalComponents(pca.rmsMinusMean, chosenVectors);

            StatusLabel.Text = "Principal components calculated succesfully.";
        }

        private void EigenValues_comboBox_TextChanged(object sender, EventArgs e) {

            if (EVComboBoxChangeEnabled) {
                ComboBox[] EVComboBox = { EV1comboBox, EV2comboBox, EV3comboBox };

                int valueIndex;
                double totalEnergy = 0;
                for (int i = 0; i < EVComboBox.Length; i++) {
                    valueIndex = EVComboBox[i].SelectedIndex;
                    totalEnergy += pca.eigenValues[valueIndex];
                }

                totalEnergy = (totalEnergy / pca.sumOfEigenValues) * 100;
                totalEnergy = Math.Round(totalEnergy, 2);
                totalEnergyLabel.Text = totalEnergy + " %";
            }            
        }

        private void drawEnergyGraphButton_Click(object sender, EventArgs e) {

            pca.MatlabPlotEnergiesChart();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {

            StopMyoIfInstantiated();
        }        

        private void StopMyoIfInstantiated() {

            if (myoArmband != null) {
                myoArmband.StopMyo();
            }
        }


    }
}
