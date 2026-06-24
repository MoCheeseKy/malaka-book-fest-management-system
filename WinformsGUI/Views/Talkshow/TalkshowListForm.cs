using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Talkshow
{
    public class TalkshowListForm : Form
    {
        // Header row
        private Label         lblTitle;
        private Label         lblDescription;
        private RoundedButton btnRefresh;
        private RoundedButton btnAdd;
        private Panel         pnlDivider;

        // Grid
        private DataGridView dgvTalkshows;

        private TalkshowService _talkshowService;
        private bool            _columnsInitialized;
        private WinformsGUI.Controls.SearchBar txtSearch;
        private System.Collections.Generic.List<TalkshowResponse> _allTalkshows = new();

        private const int M       = Theme.SpaceLG;
        private const int TitleH  = 36;
        private const int DescH   = 22;
        private const int BtnH    = 44;

        // 2 action buttons: Edit + Detail
        private const int AksiPadX = 5;
        private const int AksiPadY = 6;
        private const int AksiBtnW = 82;
        private const int AksiGap  = 4;
        private const int AksiColW = AksiPadX + AksiBtnW + AksiGap + AksiBtnW + AksiPadX; // 178

        public TalkshowListForm()
        {
            _talkshowService = new TalkshowService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblDescription = new Label();
            this.btnRefresh     = new RoundedButton();
            this.btnAdd         = new RoundedButton();
            this.txtSearch      = new WinformsGUI.Controls.SearchBar();
            this.pnlDivider     = new Panel();
            this.dgvTalkshows   = new DataGridView();

            this.Text      = "Talkshow Management";
            this.BackColor = Theme.BgDeep;
            this.ForeColor = Theme.TextPrimary;
            this.Font      = Theme.FontBody;

            // ── Title ───────────────────────────────────────────────────────
            this.lblTitle.Text      = "Talkshow Management";
            this.lblTitle.Font      = Theme.FontPageTitle;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = true;

            // ── Description ─────────────────────────────────────────────────
            this.lblDescription.Text      = "Kelola acara talkshow, tambah jadwal, dan perbarui informasi pembicara.";
            this.lblDescription.Font      = Theme.FontSmall;
            this.lblDescription.ForeColor = Theme.TextMuted;
            this.lblDescription.BackColor = Color.Transparent;
            this.lblDescription.AutoSize  = true;

            // ── Buttons ─────────────────────────────────────────────────────
            this.btnRefresh.Text         = "↻  Refresh";
            this.btnRefresh.Size         = new Size(130, 42);
            this.btnRefresh.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnRefresh);
            this.btnRefresh.Click       += BtnRefresh_Click;

            this.btnAdd.Text         = "+  Tambah";
            this.btnAdd.Size         = new Size(148, BtnH);
            this.btnAdd.Font         = Theme.FontSubhead;
            this.btnAdd.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnAdd);
            this.btnAdd.Click       += BtnAdd_Click;

            // ── Search Bar ──────────────────────────────────────────────────
            this.txtSearch.Size          = new Size(250, 42);
            this.txtSearch.PlaceholderText = "Cari talkshow...";
            this.txtSearch.TextChangedEvent += TxtSearch_TextChanged;

            // ── Divider ─────────────────────────────────────────────────────
            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(10, 1);

            // ── DataGridView ────────────────────────────────────────────────
            Theme.ApplyToDataGridView(this.dgvTalkshows);
            this.dgvTalkshows.CellPainting   += DgvTs_CellPainting;
            this.dgvTalkshows.CellMouseClick += DgvTs_CellMouseClick;
            this.dgvTalkshows.CellMouseEnter += (s, e) => UpdateCursor(e.ColumnIndex);
            this.dgvTalkshows.CellMouseLeave += (s, e) => { this.dgvTalkshows.Cursor = Cursors.Default; };
            this.dgvTalkshows.CellFormatting += DgvTs_CellFormatting;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblDescription, txtSearch, btnRefresh, btnAdd, pnlDivider, dgvTalkshows
            });

            this.Resize += (s, e) => RepositionControls();
            RepositionControls();
        }

        private void UpdateCursor(int colIndex)
        {
            if (colIndex >= 0 && dgvTalkshows.Columns.Count > colIndex &&
                dgvTalkshows.Columns[colIndex].Name == "colAksi")
                dgvTalkshows.Cursor = Cursors.Hand;
            else
                dgvTalkshows.Cursor = Cursors.Default;
        }

        private void RepositionControls()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            int headerBlockH = TitleH + 6 + DescH;
            int rowY         = M;
            int btnY         = rowY + (headerBlockH - BtnH) / 2;

            this.lblTitle.Location       = new Point(M, rowY);
            this.lblDescription.Location = new Point(M, rowY + TitleH + 6);

            this.btnAdd.Location     = new Point(w - M - btnAdd.Width, btnY);
            this.btnRefresh.Location = new Point(btnAdd.Left - Theme.SpaceSM - btnRefresh.Width,
                                                 btnY + (BtnH - btnRefresh.Height) / 2);
            this.txtSearch.Location  = new Point(btnRefresh.Left - Theme.SpaceLG - txtSearch.Width, btnY + (BtnH - txtSearch.Height) / 2);

            int divY = rowY + headerBlockH + Theme.SpaceSM;
            this.pnlDivider.Location = new Point(M, divY);
            this.pnlDivider.Size     = new Size(w - M * 2, 1);

            int dgvY = divY + 1 + Theme.SpaceSM;
            this.dgvTalkshows.Location = new Point(M, dgvY);
            this.dgvTalkshows.Size     = new Size(w - M * 2, Math.Max(0, h - dgvY - M));
        }

        // ── Cell Painting ───────────────────────────────────────────────────
        private void DgvTs_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvTalkshows.Columns[e.ColumnIndex].Name != "colAksi") return;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.Background |
                                  DataGridViewPaintParts.SelectionBackground |
                                  DataGridViewPaintParts.Border);

            int btnH = e.CellBounds.Height - AksiPadY * 2;
            int x0   = e.CellBounds.X + AksiPadX;
            int y0   = e.CellBounds.Y + AksiPadY;

            Theme.DrawGridActionButton(e.Graphics,
                new RectangleF(x0, y0, AksiBtnW, btnH),
                "✎  Edit", Theme.AccentPrimary, Theme.TextPrimary);

            Theme.DrawGridActionButton(e.Graphics,
                new RectangleF(x0 + AksiBtnW + AksiGap, y0, AksiBtnW, btnH),
                "◎  Detail", Theme.BgCard, Theme.TextSecondary);

            e.Handled = true;
        }

        private void DgvTs_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dgvTalkshows.Columns[e.ColumnIndex].Name != "colAksi") return;

            int x = e.X;
            if      (x >= AksiPadX && x < AksiPadX + AksiBtnW)           BtnEdit_Action(e.RowIndex);
            else if (x >= AksiPadX + AksiBtnW + AksiGap)                  BtnDetail_Action(e.RowIndex);
        }

        // ── Status Formatting ───────────────────────────────────────────────
        private void DgvTs_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvTalkshows.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                e.Value = (int)e.Value switch
                {
                    0 => "📅 Scheduled",
                    1 => "🔴 Ongoing",
                    2 => "✅ Completed",
                    _ => "Unknown"
                };
                e.FormattingApplied = true;
            }
        }

        // ── Data Loading ────────────────────────────────────────────────────
        private async void LoadData()
        {
            try
            {
                _allTalkshows = await _talkshowService.GetAllTalkshowsAsync();
                FilterData();

                if (!_columnsInitialized)
                {
                    if (dgvTalkshows.Columns["TalkshowId"] != null) dgvTalkshows.Columns["TalkshowId"].Visible = false;
                    if (dgvTalkshows.Columns["SpeakerBio"] != null) dgvTalkshows.Columns["SpeakerBio"].Visible = false;
                    if (dgvTalkshows.Columns["MaxCapacity"] != null) dgvTalkshows.Columns["MaxCapacity"].Visible = false;

                    var colAksi = new DataGridViewTextBoxColumn
                    {
                        Name         = "colAksi",
                        HeaderText   = "Aksi",
                        ReadOnly     = true,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        Width        = AksiColW,
                        DefaultCellStyle = { SelectionBackColor = Theme.BgCard, SelectionForeColor = Theme.TextPrimary }
                    };
                    dgvTalkshows.Columns.Add(colAksi);
                    _columnsInitialized = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterData()
        {
            if (_allTalkshows == null) return;
            var q = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q))
                dgvTalkshows.DataSource = _allTalkshows;
            else
            {
                var filtered = new System.Collections.Generic.List<TalkshowResponse>();
                foreach (var ts in _allTalkshows)
                {
                    if ((ts.Title != null && ts.Title.ToLower().Contains(q)) || 
                        (ts.SpeakerName != null && ts.SpeakerName.ToLower().Contains(q)) ||
                        (ts.Venue != null && ts.Venue.ToLower().Contains(q)))
                    {
                        filtered.Add(ts);
                    }
                }
                dgvTalkshows.DataSource = filtered;
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e) => FilterData();

        // ── Actions ─────────────────────────────────────────────────────────
        private void BtnEdit_Action(int row)
        {
            if (dgvTalkshows.Rows[row].DataBoundItem is TalkshowResponse ts)
            {
                var frm = new TalkshowEditForm(ts);
                if (frm.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void BtnDetail_Action(int row)
        {
            if (dgvTalkshows.Rows[row].DataBoundItem is TalkshowResponse ts)
            {
                var frm = new TalkshowDetailForm(ts);
                frm.ShowDialog();
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e) => LoadData();

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var frm = new TalkshowEditForm(null);
            if (frm.ShowDialog() == DialogResult.OK) LoadData();
        }
    }
}
