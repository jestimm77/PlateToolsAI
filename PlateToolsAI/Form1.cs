using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
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
        private static readonly string[] MachineOptions =
        {
            string.Empty,
            "Shear",
            "FPB",
            "2500A",
            "2500B",
            "Burn Table",
            "ESAB-1",
            "ESAB-2",
            "ESAB-3",
            "T-Order",
            "Stock"
        };

        private CutlistJob _currentJob;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize DataGridView columns
            InitializeDataGridView();
            UpdateAIEmployeeState();
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

            var machineColumn = new DataGridViewComboBoxColumn
            {
                Name = "Machine",
                HeaderText = "Machine",
                DataSource = MachineOptions.ToList(),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };

            dgvParts.Columns.Add(machineColumn);

            // Set column widths
            dgvParts.Columns["PieceMark"].Width = 100;
            dgvParts.Columns["Quantity"].Width = 80;
            dgvParts.Columns["Material"].Width = 80;
            dgvParts.Columns["Thickness"].Width = 80;
            dgvParts.Columns["Sequence"].Width = 80;
            dgvParts.Columns["Lot"].Width = 60;
            dgvParts.Columns["Machine"].Width = 100;

            dgvParts.ReadOnly = false;

            foreach (DataGridViewColumn column in dgvParts.Columns)
            {
                column.ReadOnly = column.Name != "Machine";
            }

            dgvParts.CurrentCellDirtyStateChanged += dgvParts_CurrentCellDirtyStateChanged;
            dgvParts.CellValueChanged += dgvParts_CellValueChanged;
            dgvParts.DataError += dgvParts_DataError;
        }

        private void btnProcessJob_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Select Cut List PDF",
                Filter = "PDF Files (*.pdf)|*.pdf",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var reader = new PdfCutlistReader();

            var employee =
                new AIEmployeeController(reader);

            _currentJob =
                employee.ProcessJob(dialog.FileName);

            if (chkAIEmployee.Checked)
            {
                ApplyMachineAssignments();
            }

            // Update Job Info
            lblJobNumberValue.Text = _currentJob.JobNumber;
            lblGroupValue.Text = "(none)";

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

            if (_currentJob.MachineAssignments.Count == 0 && chkAIEmployee.Checked)
            {
                chkAIEmployee.Checked = false;
            }
            else
            {
                UpdateMachineAssignmentStatus();
            }

            // Populate Parts Grid (show all parts initially)
            RefreshPartsGrid(_currentJob.Parts);

            if (_currentJob.MachineAssignments.Count == 0)
            {
                MessageBox.Show(
                    this,
                    $"No Piece Mark to Machine assignments were found in {Path.GetFileName(dialog.FileName)}.",
                    "No Assignments Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void RefreshPartsGrid(List<CutlistPart> parts)
        {
            dgvParts.Rows.Clear();

            foreach (var part in parts)
            {
                var rowIndex = dgvParts.Rows.Add(
                    part.PieceMark,
                    part.Quantity > 0 ? (object)part.Quantity : string.Empty,
                    part.Material,
                    part.Thickness,
                    part.Sequence,
                    part.Lot,
                    part.Machine
                );

                dgvParts.Rows[rowIndex].Tag = part;
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

        private void chkAIEmployee_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAIEmployeeState();

            if (_currentJob == null)
            {
                return;
            }

            if (chkAIEmployee.Checked)
            {
                ApplyMachineAssignments();
            }

            RefreshPartsGrid(_currentJob.Parts);
        }

        private void UpdateAIEmployeeState()
        {
            chkAIEmployee.Text = chkAIEmployee.Checked
                ? "AI Employee ON"
                : "AI Employee OFF";

            if (dgvParts.Columns.Contains("Machine"))
            {
                dgvParts.Columns["Machine"].ReadOnly = chkAIEmployee.Checked;
            }

            UpdateMachineAssignmentStatus();
        }

        private void UpdateMachineAssignmentStatus()
        {
            var modeText = chkAIEmployee.Checked
                ? "AI auto-select enabled"
                : "Manual machine selection enabled";

            lblMachineAssignmentStatus.Text = _currentJob == null
                ? modeText
                : $"{_currentJob.MachineAssignments.Count} assignments loaded | {modeText}";
        }

        private void ApplyMachineAssignments()
        {
            if (_currentJob == null)
            {
                return;
            }

            foreach (var part in _currentJob.Parts)
            {
                if (_currentJob.MachineAssignments.TryGetValue(part.PieceMark, out var machine))
                {
                    part.Machine = machine;
                }
            }
        }

        private void dgvParts_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvParts.IsCurrentCellDirty)
            {
                dgvParts.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvParts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (chkAIEmployee.Checked || e.RowIndex < 0 || dgvParts.Columns[e.ColumnIndex].Name != "Machine")
            {
                return;
            }

            if (dgvParts.Rows[e.RowIndex].Tag is CutlistPart part)
            {
                part.Machine = Convert.ToString(dgvParts.Rows[e.RowIndex].Cells["Machine"].Value) ?? string.Empty;
            }
        }

        private void dgvParts_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            lstLots.Items.Clear();
            lstSequences.Items.Clear();
            dgvParts.Rows.Clear();
            lblJobNumberValue.Text = "(none)";
            lblGroupValue.Text = "(none)";
            _currentJob = null;
            UpdateMachineAssignmentStatus();
        }
    }
}
