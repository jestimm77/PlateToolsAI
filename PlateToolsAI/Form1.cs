using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PlateToolsAI.AIEmployee;
using PlateToolsAI.Readers;


namespace PlateToolsAI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnProcessJob_Click(object sender, EventArgs e)
        {
            var reader = new PdfCutlistReader();

            var employee =
                new AIEmployeeController(reader);

            var job =
                employee.ProcessJob("34778.pdf");

            txtJobNumber.Text = job.JobNumber;

            lstLots.Items.Clear();

            foreach (var lot in job.Lots)
            {
                lstLots.Items.Add(lot);
            }

          

            foreach (var sequence in job.Sequences)
            {
                lstSequences.Items.Add(sequence);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
