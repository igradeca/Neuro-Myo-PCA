using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neuro_myo {
    public class Myo {

        public bool myoIsWorking = false;

        private MainForm mainForm;
        private IChannel myoChannel;
        private IHub myoHub;        

        public Myo(MainForm mainForm) {

            this.mainForm = mainForm;
        }

        public void InitializeMyo() {

            myoChannel = Channel.Create(ChannelDriver.Create
                (ChannelBridge.Create(), MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())) 
                );
            myoHub = Hub.Create(myoChannel);
            myoHub.MyoConnected += myoHub_MyoConnected;
            myoHub.MyoDisconnected += myoHub_MyoDisconnected;            
        }

        public void StartMyo() {

            myoChannel.StartListening();
            myoIsWorking = true;
        }

        public void StopMyo() {

            myoChannel.StopListening();
            myoChannel.Dispose();
            myoIsWorking = false;
            mainForm.StatusLabel.Text = "Myo has stopped listening.";
        }

        private void myoHub_MyoConnected(object sender, MyoEventArgs e) {

            mainForm.StatusLabel.Text = "Myo is connected and it is listening.";
            //MessageBox.Show("Myo is connected.\nPress OK to start listening.", "Myo Armband");            
            e.Myo.Vibrate(VibrationType.Medium);

            e.Myo.EmgDataAcquired += Myo_EmgDataAcquired;
            e.Myo.OrientationDataAcquired += Myo_OrientationDataAcquired;
            e.Myo.SetEmgStreaming(true);

            //myoIsWorking = true;            
        }

        private void myoHub_MyoDisconnected(object sender, MyoEventArgs e) {

            MessageBox.Show("Myo is disconnected!", "Myo Armband");
            e.Myo.Vibrate(VibrationType.Long);

            e.Myo.EmgDataAcquired -= Myo_EmgDataAcquired;
            e.Myo.OrientationDataAcquired -= Myo_OrientationDataAcquired;
            e.Myo.SetEmgStreaming(false);
        }

        private void Myo_EmgDataAcquired(object sender, EmgDataEventArgs e) {        
                
            for (int i = 0; i < mainForm.numberOfSensors; i++) {
                mainForm.myoEMG[i].Add(e.EmgData.GetDataForSensor(i));
            }            
        }

        private void Myo_OrientationDataAcquired(object sender, OrientationDataEventArgs e) {

            mainForm.myoOrientation[0].Add(e.Orientation.X);
            mainForm.myoOrientation[1].Add(e.Orientation.Y);
            mainForm.myoOrientation[2].Add(e.Orientation.Z);
            mainForm.myoOrientation[3].Add(e.Orientation.W);
        }

    }
}
