using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neuro_myo {
    public partial class RenameMovementForm : Form {

        private SetPCAForm pcaForm;
        private string name;

        public RenameMovementForm(SetPCAForm pcaForm, string name) {
            InitializeComponent();
            this.pcaForm = pcaForm;
            this.name = name;            
        }

        private void RenameMovementForm_Load(object sender, EventArgs e) {
            nameTextBox.Text = name;
        }

        private void OKButton_Click(object sender, EventArgs e) {
            pcaForm.ChangeName(nameTextBox.Text);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Close();
        }

        
    }
}
