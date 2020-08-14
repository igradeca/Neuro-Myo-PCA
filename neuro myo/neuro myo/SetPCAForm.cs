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
    public partial class SetPCAForm : Form {

        private MainForm mainForm;
        private int movementCounter, elapsedTime;
        private int listeningTimeInSec = 20;

        public SetPCAForm(MainForm mainForm, Myo myo) {

            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void SetPCAForm_Load(object sender, EventArgs e) {
            
            LoadMovements();
        }

        private void addNewMovButton_Click(object sender, EventArgs e) {

            mainForm.movements.Add(new Movement("Movement " + movementCounter, mainForm.numberOfSensors));
            LoadMovements();
        }

        private void renameButton_Click(object sender, EventArgs e) {

            if (movementsListBox.SelectedItem != null) {
                RenameMovementForm renameForm = new RenameMovementForm(this, movementsListBox.SelectedItem.ToString());
                renameForm.Show();
            }            
        }

        private void startListeningButton_Click(object sender, EventArgs e) {

            if (movementsListBox.SelectedItem != null) {
                mainForm.SetUpArraysForMyo();
                mainForm.CreateMyoArmbandObject();

                Thread rmsThread = new Thread(new ThreadStart(RmsThreadsWork));
                mainForm.myoArmband.StartMyo();
                rmsThread.Start();

                startListeningButton.Enabled = false;
                elapsedTime = 0;
                timer1.Start();
            }            
        }

        private void RmsThreadsWork() {

            while (mainForm.myoArmband.myoIsWorking) {
                Thread.Sleep(250);
                if (mainForm.myoEMG[0].Count >= 50) {
                    mainForm.CalulateRootMeanSquareForEMG();
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e) {

            if (movementsListBox.SelectedItem != null) {
                for (int i = 0; i < mainForm.movements.Count; i++) {
                    if (mainForm.movements[i].name == movementsListBox.SelectedItem.ToString()) {
                        mainForm.movements.RemoveAt(i);
                        break;
                    }
                }
                LoadMovements();
            }            
        }

        public void ChangeName(string name) {

            foreach (Movement item in mainForm.movements) {
                if (item.name == movementsListBox.SelectedItem.ToString()) {
                    item.name = name;
                    break;
                }
            }
            LoadMovements();
        }

        private void LoadMovements() {

            movementsListBox.Items.Clear();
            movementCounter = 0;
            foreach (Movement item in mainForm.movements) {
                movementsListBox.Items.Add(item.name);
                ++movementCounter;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {

            ++elapsedTime;
            if (elapsedTime >= listeningTimeInSec) {
                mainForm.myoArmband.StopMyo();
                timer1.Stop();

                SaveMovementRmsValues(movementsListBox.SelectedIndex, mainForm.rmsData);

                startListeningButton.Enabled = true;
            }
        }

        private void SaveMovementRmsValues(int movementIndex, List<List<double>> capturedRmsData) {

            for (int i = 0; i < capturedRmsData.Count; i++) {
                for (int j = 0; j < capturedRmsData[i].Count; j++) {
                    mainForm.movements[movementIndex].rmsData[i].Add(capturedRmsData[i][j]);
                }                
            }
        }


    }
}
