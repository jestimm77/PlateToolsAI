using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private bool _loadingGrid;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDataGridView();
            UpdateAIEmployeeState();
            UpdateTotalsSummary(new List<CutlistPart>());
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
            dgvParts.Columns.Add("Status", "Status");

            dgvParts.Columns["PieceMark"].Width = 100;
            dgvParts.Columns["Quantity"].Width = 80;
            dgvParts.Columns["Material"].Width = 110;
            dgvParts.Columns["Thickness"].Width = 80;
            dgvParts.Columns["Sequence"].Width = 80;
            dgvParts.Columns["Lot"].Width = 60;
            dgvParts.Columns["Machine"].Width = 100;
            dgvParts.Columns["Status"].Width = 180;

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
                Title = "Select Cut List",
                Filter = "Cut List Files (*.pdf;*.txt;*.csv)|*.pdf;*.txt;*.csv|PDF Files (*.pdf)|*.pdf|Text Files (*.txt;*.csv)|*.txt;*.csv",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                var employee = new AIEmployeeController(new PdfCutlistReader());
                _currentJob = employee.ProcessJob(dialog.FileName);

                if (chkAIEmployee.Checked)
                {
                    ApplyMachineAssignments();
                }

                lblJobNumberValue.Text = _currentJob.JobNumber;
                lblGroupValue.Text = "(none)";
                PopulateFilters();
                RefreshCurrentView();

                if (_currentJob.ImportWarnings.Count > 0)
                {
                    MessageBox.Show(
                        this,
                        string.Join(Environment.NewLine, _currentJob.ImportWarnings),
                        "Import Review",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Import Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void PopulateFilters()
        {
            lstLots.Items.Clear();
            foreach (var lot in _currentJob.Lots)
            {
                lstLots.Items.Add($"Lot {lot}");
            }

            lstSequences.Items.Clear();
            foreach (var sequence in _currentJob.Sequences)
            {
                lstSequences.Items.Add($"Sequence {sequence}");
            }

            lstLots.ClearSelected();
            lstSequences.ClearSelected();
        }

        private void RefreshPartsGrid(List<CutlistPart> parts)
        {
            _loadingGrid = true;
            try
            {
                dgvParts.Rows.Clear();

                foreach (var part in parts)
                {
                    var rowIndex = dgvParts.Rows.Add(
                        part.PieceMark,
                        part.Quantity,
                        part.Material,
                        part.Thickness,
                        part.Sequence,
                        part.Lot,
                        part.Machine,
                        GetAssignmentStatus(part)
                    );

                    dgvParts.Rows[rowIndex].Tag = part;
                }
            }
            finally
            {
                _loadingGrid = false;
            }
        }

        private void lstLots_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCurrentView();
        }

        private void lstSequences_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCurrentView();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_currentJob == null)
            {
                return;
            }

            if (chkAIEmployee.Checked)
            {
                ApplyMachineAssignments();
            }

            RefreshCurrentView();
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

            RefreshCurrentView();
        }

        private void UpdateAIEmployeeState()
        {
            UpdateMachineAssignmentStatus();
        }

        private void ApplyMachineAssignments()
        {
            if (_currentJob == null)
            {
                return;
            }

            foreach (var part in _currentJob.Parts)
            {
                if (string.IsNullOrWhiteSpace(part.SuggestedMachine))
                {
                    continue;
                }

                if (!part.HasManualMachineOverride || string.IsNullOrWhiteSpace(part.Machine))
                {
                    part.Machine = part.SuggestedMachine;
                }
            }
        }

        private void RefreshCurrentView()
        {
            var filteredParts = GetFilteredParts();
            RefreshPartsGrid(filteredParts);
            UpdateMachineAssignmentStatus();
            UpdateTotalsSummary(filteredParts);
        }

        private List<CutlistPart> GetFilteredParts()
        {
            if (_currentJob == null)
            {
                return new List<CutlistPart>();
            }

            IEnumerable<CutlistPart> parts = _currentJob.Parts;

            var selectedLots = lstLots.SelectedItems
                .Cast<object>()
                .Select(item => item?.ToString()?.Replace("Lot ", string.Empty))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            if (selectedLots.Count > 0)
            {
                parts = parts.Where(part =>
                    selectedLots.Any(lot => string.Equals(part.Lot, lot, StringComparison.OrdinalIgnoreCase)));
            }

            var selectedSequences = lstSequences.SelectedItems
                .Cast<object>()
                .Select(item => item?.ToString()?.Replace("Sequence ", string.Empty))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            if (selectedSequences.Count > 0)
            {
                parts = parts.Where(part =>
                    selectedSequences.Any(sequence => string.Equals(part.Sequence, sequence, StringComparison.OrdinalIgnoreCase)));
            }

            return parts.ToList();
        }

        private void UpdateMachineAssignmentStatus()
        {
            var modeText = chkAIEmployee.Checked
                ? "AI suggestions will fill recognized matches."
                : "Manual routing only.";

            if (_currentJob == null)
            {
                lblMachineAssignmentStatus.Text = modeText;
                return;
            }

            var suggestedQty = _currentJob.Parts
                .Where(part => !string.IsNullOrWhiteSpace(part.SuggestedMachine))
                .Sum(GetQuantity);

            var needsReviewQty = _currentJob.Parts
                .Where(part => string.IsNullOrWhiteSpace(part.Machine))
                .Sum(GetQuantity);

            lblMachineAssignmentStatus.Text = $"{suggestedQty} qty with AI suggestions | {needsReviewQty} qty still need review | {modeText}";
        }

        private void UpdateTotalsSummary(IReadOnlyCollection<CutlistPart> visibleParts)
        {
            var allParts = _currentJob?.Parts ?? new List<CutlistPart>();
            var summary = new StringBuilder();

            summary.AppendLine($"Project Qty: {allParts.Sum(GetQuantity)}");
            summary.AppendLine($"Visible Qty: {visibleParts.Sum(GetQuantity)}");
            summary.AppendLine($"Needs Review Qty: {allParts.Where(part => string.IsNullOrWhiteSpace(part.Machine)).Sum(GetQuantity)}");
            summary.AppendLine();
            summary.AppendLine("Project Machine Totals:");
            AppendMachineTotals(summary, allParts);

            if (_currentJob != null && visibleParts.Count != allParts.Count)
            {
                summary.AppendLine();
                summary.AppendLine("Filtered View Totals:");
                AppendMachineTotals(summary, visibleParts);
            }

            if (_currentJob != null && _currentJob.ImportWarnings.Count > 0)
            {
                summary.AppendLine();
                summary.AppendLine("Import Notes:");
                foreach (var warning in _currentJob.ImportWarnings)
                {
                    summary.AppendLine($"- {warning}");
                }
            }

            txtTotalsSummary.Text = summary.ToString().TrimEnd();
        }

        private static void AppendMachineTotals(StringBuilder summary, IEnumerable<CutlistPart> parts)
        {
            var machineTotals = MachineOptions
                .Where(option => !string.IsNullOrWhiteSpace(option))
                .ToDictionary(
                    option => option,
                    option => parts.Where(part => string.Equals(part.Machine, option, StringComparison.OrdinalIgnoreCase)).Sum(GetQuantity)
                );

            foreach (var entry in machineTotals)
            {
                summary.AppendLine($"{entry.Key}: {entry.Value}");
            }

            summary.AppendLine($"Unassigned: {parts.Where(part => string.IsNullOrWhiteSpace(part.Machine)).Sum(GetQuantity)}");
        }

        private static int GetQuantity(CutlistPart part)
        {
            return Math.Max(part?.Quantity ?? 0, 0);
        }

        private static string GetAssignmentStatus(CutlistPart part)
        {
            if (part == null)
            {
                return string.Empty;
            }

            if (part.HasManualMachineOverride)
            {
                return "Manual";
            }

            if (!string.IsNullOrWhiteSpace(part.Machine) &&
                string.Equals(part.Machine, part.SuggestedMachine, StringComparison.OrdinalIgnoreCase))
            {
                return "AI Match";
            }

            if (!string.IsNullOrWhiteSpace(part.SuggestedMachine))
            {
                return $"AI Suggestion: {part.SuggestedMachine}";
            }

            if (!string.IsNullOrWhiteSpace(part.ReviewNote))
            {
                return part.ReviewNote;
            }

            return string.IsNullOrWhiteSpace(part.Machine) ? "Unassigned" : "Manual";
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
            if (_loadingGrid || e.RowIndex < 0 || dgvParts.Columns[e.ColumnIndex].Name != "Machine")
            {
                return;
            }

            if (!(dgvParts.Rows[e.RowIndex].Tag is CutlistPart part))
            {
                return;
            }

            part.Machine = Convert.ToString(dgvParts.Rows[e.RowIndex].Cells["Machine"].Value) ?? string.Empty;
            part.HasManualMachineOverride = string.IsNullOrWhiteSpace(part.SuggestedMachine)
                ? !string.IsNullOrWhiteSpace(part.Machine)
                : !string.Equals(part.Machine, part.SuggestedMachine, StringComparison.OrdinalIgnoreCase);

            dgvParts.Rows[e.RowIndex].Cells["Status"].Value = GetAssignmentStatus(part);
            UpdateMachineAssignmentStatus();
            UpdateTotalsSummary(GetFilteredParts());
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
            txtTotalsSummary.Clear();
            lblJobNumberValue.Text = "(none)";
            lblGroupValue.Text = "(none)";
            _currentJob = null;
            UpdateAIEmployeeState();
            UpdateTotalsSummary(new List<CutlistPart>());
        }
    }
}
