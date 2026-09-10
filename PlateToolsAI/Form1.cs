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
using PlateToolsAI.Models;
using PlateToolsAI.Readers;


namespace PlateToolsAI
{
    public partial class Form1 : Form
    {
        private CutlistJob _currentJob;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize DataGridView columns
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dgvParts.Columns.Clear();
            dgvParts.Columns.Add("PieceMark", "Piece Mark");
            dgvParts.Columns.Add("Quantity", "Quantity");
            dgvParts.Columns.Add("Material", "Material");
            dgvParts.Columns.Add("Thickness", "Thickness");
            dgvParts.Columns.Add("Sequence", "Sequence");
            dgvParts.Columns.Add("Lot", "Lot");
            dgvParts.Columns.Add("Machine", "Machine");

            // Set column widths
            dgvParts.Columns["PieceMark"].Width = 100;
            dgvParts.Columns["Quantity"].Width = 80;
            dgvParts.Columns["Material"].Width = 80;
            dgvParts.Columns["Thickness"].Width = 80;
            dgvParts.Columns["Sequence"].Width = 80;
            dgvParts.Columns["Lot"].Width = 60;
            dgvParts.Columns["Machine"].Width = 100;
        }

        private void btnProcessJob_Click(object sender, EventArgs e)
        {
            var reader = new PdfCutlistReader();

            var employee =
                new AIEmployeeController(reader);

            _currentJob =
                employee.ProcessJob("34778.pdf");

            // Update Job Info
            lblJobNumberValue.Text = _currentJob.JobNumber;
            lblGroupValue.Text = "1"; // For now, default group

            // Populate Lots
            lstLots.Items.Clear();
            foreach (var lot in _currentJob.Lots)
            {
                lstLots.Items.Add($"Lot {lot}");
            }

            // Populate Sequences
            lstSequences.Items.Clear();
            foreach (var sequence in _currentJob.Sequences)
            {
                lstSequences.Items.Add($"Sequence {sequence}");
            }

            // Populate Parts Grid (show all parts initially)
            RefreshPartsGrid(_currentJob.Parts);
        }

        private void RefreshPartsGrid(List<CutlistPart> parts)
        {
            dgvParts.Rows.Clear();

            foreach (var part in parts)
            {
                dgvParts.Rows.Add(
                    part.PieceMark,
                    part.Quantity,
                    part.Material,
                    part.Thickness,
                    part.Sequence,
                    part.Lot,
                    part.Machine
                );
            }
        }

        private void lstLots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentJob == null || lstLots.SelectedIndex == -1)
                return;

            // Get selected lot
            string selectedLotText = lstLots.SelectedItem.ToString();
            string selectedLot = selectedLotText.Replace("Lot ", "");

            // Filter parts by selected lot
            var filteredParts = _currentJob.Parts
                .Where(p => p.Lot == selectedLot)
                .ToList();

            RefreshPartsGrid(filteredParts);
        }

        private void lstSequences_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentJob == null || lstSequences.SelectedIndex == -1)
                return;

            // Get selected sequence
            string selectedSequenceText = lstSequences.SelectedItem.ToString();
            string selectedSequence = selectedSequenceText.Replace("Sequence ", "");

            // Filter parts by selected sequence
            var filteredParts = _currentJob.Parts
                .Where(p => p.Sequence == selectedSequence)
                .ToList();

            RefreshPartsGrid(filteredParts);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            lstLots.Items.Clear();
            lstSequences.Items.Clear();
            dgvParts.Rows.Clear();
            lblJobNumberValue.Text = "(none)";
            lblGroupValue.Text = "(none)";
            _currentJob = null;
        }
    }
}
