using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Booth
{
    public class BoothListForm : Form
    {
        // Header row
        private Label         lblTitle;
        private Label         lblDescription;
        private RoundedButton btnRefresh;
        private RoundedButton btnAdd;

        // Grid
        private DataGridView dgvBooths;

        private BoothService _boothService;
        private bool         _columnsInitialized;
        private WinformsGUI.Controls.SearchBar txtSearch;
        private System.Collections.Generic.List<BoothResponse> _allBooths = new();

        // Layout constants
        private const int Margin       = Theme.SpaceLG;  // margin = 28
        private const int TitleHeight  = 36;             // tinggi baris title (FontPageTitle 18F)
        private const int DescriptionHeight   = 22;             // tinggi baris deskripsi (FontSmall 10F)
        private const int ButtonHeight    = 44;             // tinggi tombol

        // Action column geometry (3 buttons: Edit + Delete + Detail)
        private const int AksiPadX = 5;
        private const int AksiPadY = 6;
        private const int AksiBtnW = 82;
        private const int AksiGap  = 4;
        private const int AksiColW = AksiPadX + AksiBtnW + AksiGap + AksiBtnW + AksiGap + AksiBtnW + AksiPadX; // 264

        public BoothListForm()
        {
            _boothService = new BoothService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.lblTitle      = new Label();
            this.lblDescription= new Label();
            this.btnRefresh    = new RoundedButton();
            this.btnAdd        = new RoundedButton();
            this.txtSearch     = new WinformsGUI.Controls.SearchBar();
            this.dgvBooths     = new DataGridView();

            this.Text      = "Booth Management";
            this.BackColor = Theme.BgDeep;
            this.ForeColor = Theme.TextPrimary;
            this.Font      = Theme.FontBody;

            // ── Title ───────────────────────────────────────────────────────
            this.lblTitle.Text      = "Booth Management";
            this.lblTitle.Font      = Theme.FontPageTitle;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = true;

            // ── Description ─────────────────────────────────────────────────
            this.lblDescription.Text      = "Kelola daftar booth, perbarui informasi, atau hapus booth tidak aktif.";
            this.lblDescription.Font      = Theme.FontSmall;
            this.lblDescription.ForeColor = Theme.TextMuted;
            this.lblDescription.BackColor = Color.Transparent;
            this.lblDescription.AutoSize  = true;

            // ── Refresh Button ──────────────────────────────────────────────
            this.btnRefresh.Text          = "↻  Refresh";
            this.btnRefresh.Size          = new Size(130, 42);
            this.btnRefresh.CornerRadius  = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnRefresh);
            this.btnRefresh.Click        += BtnRefresh_Click;

            // ── Add Button ──────────────────────────────────────────────────
            this.btnAdd.Text          = "+  Tambah";
            this.btnAdd.Size          = new Size(148, ButtonHeight);
            this.btnAdd.Font          = Theme.FontSubhead;
            this.btnAdd.CornerRadius  = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnAdd);
            this.btnAdd.Click        += BtnAdd_Click;

            // ── Search Bar ──────────────────────────────────────────────────
            this.txtSearch.Size          = new Size(250, 42);
            this.txtSearch.PlaceholderText = "Cari booth...";
            this.txtSearch.TextChangedEvent += TxtSearch_TextChanged;

        // ── DataGridView ────────────────────────────────────────────────
            Theme.ApplyToDataGridView(this.dgvBooths);
            this.dgvBooths.CellPainting   += DgvBooths_CellPainting;
            this.dgvBooths.CellMouseClick += DgvBooths_CellMouseClick;
            this.dgvBooths.Paint          += (s, e) => Theme.DrawEmptyState(dgvBooths, e, "Tidak ada data booth yang ditemukan.");
            this.dgvBooths.CellMouseEnter += (s, e) => UpdateCursor(e.ColumnIndex);
            this.dgvBooths.CellMouseLeave += (s, e) => { this.dgvBooths.Cursor = Cursors.Default; };

            // ── Compose ─────────────────────────────────────────────────────
            this.Controls.AddRange(new Control[] {
                lblTitle, lblDescription, txtSearch, btnRefresh, btnAdd, dgvBooths
            });

            this.Resize += (s, e) => RepositionControls();
            RepositionControls();
        }

        private void UpdateCursor(int colIndex)
        {
            if (colIndex >= 0 && dgvBooths.Columns.Count > colIndex &&
                dgvBooths.Columns[colIndex].Name == "colAksi")
                dgvBooths.Cursor = Cursors.Hand;
            else
                dgvBooths.Cursor = Cursors.Default;
        }

        private void RepositionControls()
        {
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // Header row: title (left) | buttons (right), vertically centered
            int headerBlockH = TitleHeight + 8 + DescriptionHeight;
            int rowY         = 102;                              // top of header row (aligns with sidebar title)
            int buttonPositionY = rowY + (headerBlockH - ButtonHeight) / 2; // vertically centered

            this.lblTitle.Location       = new Point(Margin, rowY);
            this.lblDescription.Location = new Point(Margin, rowY + TitleHeight + 8);

            // Buttons right-aligned
            this.btnAdd.Location     = new Point(width - Margin - btnAdd.Width, buttonPositionY);
            this.btnRefresh.Location = new Point(btnAdd.Left - Theme.SpaceSM - btnRefresh.Width, buttonPositionY + (ButtonHeight - btnRefresh.Height) / 2);
            this.txtSearch.Location  = new Point(btnRefresh.Left - Theme.SpaceLG - txtSearch.Width, buttonPositionY + (ButtonHeight - txtSearch.Height) / 2);

            // DataGridView (fills remaining space with larger top gap)
            int dgvY = rowY + headerBlockH + 48; // 48px gap instead of divider
            this.dgvBooths.Location = new Point(Margin, dgvY);
            this.dgvBooths.Size     = new Size(width - Margin * 2, Math.Max(0, height - dgvY - Margin));
        }

        // ── Cell Painting: Action Column ────────────────────────────────────
        private void DgvBooths_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvBooths.Columns[e.ColumnIndex].Name != "colAksi") return;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.Background |
                                  DataGridViewPaintParts.SelectionBackground |
                                  DataGridViewPaintParts.Border);

            int buttonHeight = e.CellBounds.Height - AksiPadY * 2;
            int x0   = e.CellBounds.X + AksiPadX;
            int y0   = e.CellBounds.Y + AksiPadY;

            Theme.DrawGridActionButton(e.Graphics,
                new RectangleF(x0, y0, AksiBtnW, buttonHeight),
                "✎  Edit", Theme.AccentPrimary, Theme.TextPrimary);

            Theme.DrawGridActionButton(e.Graphics,
                new RectangleF(x0 + AksiBtnW + AksiGap, y0, AksiBtnW, buttonHeight),
                "✕  Hapus", Theme.AccentDanger, Theme.TextPrimary);

            Theme.DrawGridActionButton(e.Graphics,
                new RectangleF(x0 + (AksiBtnW + AksiGap) * 2, y0, AksiBtnW, buttonHeight),
                "◎  Detail", Theme.BgCard, Theme.TextSecondary);

            e.Handled = true;
        }

        // ── Cell Click: Dispatch Action ─────────────────────────────────────
        private void DgvBooths_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dgvBooths.Columns[e.ColumnIndex].Name != "colAksi") return;

            int positionX = e.X;
            if      (positionX >= AksiPadX && positionX < AksiPadX + AksiBtnW)
                BtnEdit_Action(e.RowIndex);
            else if (positionX >= AksiPadX + AksiBtnW + AksiGap &&
                     positionX < AksiPadX + (AksiBtnW + AksiGap) * 2)
                BtnDelete_Action(e.RowIndex);
            else if (positionX >= AksiPadX + (AksiBtnW + AksiGap) * 2)
                BtnDetail_Action(e.RowIndex);
        }

        // ── Data Loading ────────────────────────────────────────────────────
        private async void LoadData()
        {
            try
            {
                _allBooths = await _boothService.GetAllBoothsAsync();
                FilterData();

                if (!_columnsInitialized)
                {
                    if (dgvBooths.Columns["Id"]          != null) dgvBooths.Columns["Id"].Visible          = false;
                    if (dgvBooths.Columns["Description"] != null) dgvBooths.Columns["Description"].Visible = false;
                    if (dgvBooths.Columns["Category"]    != null) dgvBooths.Columns["Category"].Visible    = false;
                    if (dgvBooths.Columns["IsActive"]    != null) dgvBooths.Columns["IsActive"].Visible    = false;

                    if (dgvBooths.Columns["CategoryName"] != null)
                    {
                        dgvBooths.Columns["CategoryName"].HeaderText = "Category";
                        dgvBooths.Columns["CategoryName"].DisplayIndex = 2; // Adjust if needed
                    }

                    var colAksi = new DataGridViewTextBoxColumn
                    {
                        Name         = "colAksi",
                        HeaderText   = "Aksi",
                        ReadOnly     = true,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        Width        = AksiColW,
                        DefaultCellStyle = { SelectionBackColor = Theme.BgCard, SelectionForeColor = Theme.TextPrimary }
                    };
                    dgvBooths.Columns.Add(colAksi);
                    _columnsInitialized = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterData()
        {
            if (_allBooths == null) return;
            var q = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q))
                dgvBooths.DataSource = _allBooths;
            else
            {
                var filtered = new System.Collections.Generic.List<BoothResponse>();
                foreach (var b in _allBooths)
                {
                    if ((b.Name != null && b.Name.ToLower().Contains(q)) || 
                        (b.Location != null && b.Location.ToLower().Contains(q)) ||
                        (b.CategoryName != null && b.CategoryName.ToLower().Contains(q)))
                    {
                        filtered.Add(b);
                    }
                }
                dgvBooths.DataSource = filtered;
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e) => FilterData();

        // ── Actions ─────────────────────────────────────────────────────────
        private void BtnEdit_Action(int row)
        {
            if (dgvBooths.Rows[row].DataBoundItem is BoothResponse booth)
            {
                var frm = new BoothEditForm(booth);
                if (frm.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private async void BtnDelete_Action(int row)
        {
            if (dgvBooths.Rows[row].DataBoundItem is BoothResponse booth)
            {
                var confirm = MessageBox.Show(
                    $"Hapus booth \"{booth.Name}\"? Tindakan ini tidak dapat dibatalkan.",
                    "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        await _boothService.DeleteBoothAsync(booth.Id);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnDetail_Action(int row)
        {
            if (dgvBooths.Rows[row].DataBoundItem is BoothResponse booth)
            {
                var frm = new BoothDetailForm(booth);
                frm.ShowDialog();
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e) => LoadData();

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var frm = new BoothEditForm(null);
            if (frm.ShowDialog() == DialogResult.OK) LoadData();
        }
    }
}
