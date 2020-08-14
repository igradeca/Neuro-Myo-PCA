using Accord;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuro_myo {
    public class Movement {

        public string name { get; set; }
        public double[] location { get; set; }
        public List<List<double>> rmsData { get; set; }

        public Movement(string name, int numberOfSensors) {

            this.name = name;
            location = new double[3];
            InstatiateRmsDataList(numberOfSensors);            
        }

        private void InstatiateRmsDataList(int numberOfSensors) {
            rmsData = new List<List<double>>();

            for (int i = 0; i < numberOfSensors; i++) {
                rmsData.Add(new List<double>());
            }
        }


    }
}
