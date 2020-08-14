using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neuro_myo {
    class FileReadAndWrite {

        private BinaryWriter fileWriter;
        private BinaryReader fileReader;
        private MainForm mainForm;

        public FileReadAndWrite(MainForm mainForm) {

            this.mainForm = mainForm;
        }

        public void BinaryWrite() {

            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() != DialogResult.OK) {
                return;
            }
            fileWriter = new BinaryWriter(new FileStream(saveFile.FileName, FileMode.Create));

            FileDataWrite(mainForm.myoEMG);
            FileDataWrite(mainForm.rmsData);
            FileDataWrite(mainForm.myoOrientation);

            fileWriter.Close();
        }

        private void FileDataWrite(List<List<int>> data) {

            int elementsNum = 0;
            for (int i = 0; i < data.Count; i++) {
                elementsNum += data[i].Count;
            }
            fileWriter.Write(elementsNum);
            fileWriter.Write(data.Count);

            for (int i = 0; i < data.Count; i++) {
                for (int j = 0; j < data[i].Count; j++) {
                    fileWriter.Write(data[i].ElementAt(j));
                }
            }
        }

        private void FileDataWrite(List<List<double>> data) {

            int elementsNum = 0;
            for (int i = 0; i < data.Count; i++) {
                elementsNum += data[i].Count;
            }
            fileWriter.Write(elementsNum);

            for (int i = 0; i < data.Count; i++) {
                for (int j = 0; j < data[i].Count; j++) {
                    fileWriter.Write(data[i].ElementAt(j));
                }
            }
        }

        public void BinaryRead() {

            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() != DialogResult.OK) {
                return;
            }

            fileReader = new BinaryReader(new FileStream(openFile.FileName, FileMode.Open));

            int elementsNum = fileReader.ReadInt32();
            int sensorNum = fileReader.ReadInt32();

            ClearExistingData();

            FileDataRead(sensorNum, elementsNum, mainForm.myoEMG);
            DrawChartsFromData(sensorNum, mainForm.myoEMG);

            elementsNum = fileReader.ReadInt32();
            FileDataRead(sensorNum, elementsNum, mainForm.rmsData);
            DrawChartsFromData(sensorNum, mainForm.rmsData);

            elementsNum = fileReader.ReadInt32();
            FileDataRead(4, elementsNum, mainForm.myoOrientation);
            DrawChartsFromData(4, mainForm.myoOrientation);

            fileReader.Close();
        }

        private void ClearExistingData() {

            string message = "Do you want to clear data that are already loaded in program?";
            DialogResult dialogResult = MessageBox.Show(message, "Clear data", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                mainForm.SetUpArraysForMyo();
            }
        }

        private void ClearArrays(int sensorNum) {

            mainForm.myoEMG.Clear();
            mainForm.rmsData.Clear();
            for (int i = 0; i < sensorNum; i++) {
                mainForm.myoEMG.Add(new List<int>());
                mainForm.rmsData.Add(new List<double>());
            }

            mainForm.myoOrientation.Clear();
            for (int i = 0; i < 4; i++) {
                mainForm.myoOrientation.Add(new List<double>());
            }
        }

        private void FileDataRead(int sensorCount, int elementsCount, List<List<int>> data) {
            for (int i = 0; i < sensorCount; i++) {
                for (int j = 0; j < (elementsCount / sensorCount); j++) {
                    data[i].Add(fileReader.ReadInt32());
                }
            }
        }

        private void FileDataRead(int sensorCount, int elementsCount, List<List<double>> data) {
            for (int i = 0; i < sensorCount; i++) {
                for (int j = 0; j < (elementsCount / sensorCount); j++) {
                    data[i].Add(fileReader.ReadDouble());
                }
            }
        }

        private void DrawChartsFromData(int sensorCount, List<List<double>> data) {

            int k;
            if (sensorCount == 4) {
                k = 16;
                mainForm.chart1.ChartAreas[8].AxisX.ScaleView.Zoom(-500, 500);
                mainForm.chart1.ChartAreas[8].AxisX.Maximum = (500 / 4);
            } else {
                k = 8;
            }

            for (int i = 0; i < sensorCount; i++) {
                mainForm.chart1.Series[i + k].Points.Clear();
                for (int j = 0; j < data[i].Count; j++) {
                    mainForm.chart1.Series[i + k].Points.AddY(data[i].ElementAt(j));
                }
            }
        }

        private void DrawChartsFromData(int sensorCount, List<List<int>> data) {

            for (int i = 0; i < sensorCount; i++) {
                mainForm.chart1.ChartAreas[i].AxisX.ScaleView.Zoom(-500, 500);
                mainForm.chart1.ChartAreas[i].AxisX.Maximum = (500 / sensorCount);

                mainForm.chart1.Series[i].Points.Clear();
                for (int j = 0; j < data[i].Count; j++) {
                    mainForm.chart1.Series[i].Points.AddY(data[i].ElementAt(j));
                }
            }
        }


    }
}
