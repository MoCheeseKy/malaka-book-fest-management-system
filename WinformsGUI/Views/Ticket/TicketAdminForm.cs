#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Ticket
{
    public class TicketAdminForm : Form
    {
        // Header row
        private Label         lblTitle;
        private Label         lblDescription;
        private RoundedButton btnRefresh;
        private WinformsGUI.Controls.SearchBar txtSearch;

        // Grid + Scan card
        private DataGridView  dgvTickets;
        private Panel         pnlScanCard;
        private Label         lblScanTitle;
        private Label         lblScanHint;
        private Label         lblTicketIdLabel;
        private TextBox       txtTicketId;
        private RoundedButton btnScan;
        private Label         lblScanResult;

        private TicketService _ticketService;
        private System.Collections.Generic.List<TicketResponse> _allTickets = new();

        private const int Margin      = Theme.SpaceLG;
        private const int TitleHeight = 36;
        private const int DescriptionHeight  = 22;

        public TicketAdminForm()
        {
            _ticketService = new TicketService();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.lblTitle         = new Label();
            this.lblDescription   = new Label();
            this.btnRefresh       = new RoundedButton();
            this.txtSearch        = new WinformsGUI.Controls.SearchBar();
            this.dgvTickets       = new DataGridView();
            this.pnlScanCard      = new Panel();
            this.lblScanTitle     = new Label();
            this.lblScanHint      = new Label();
            this.lblTicketIdLabel = new Label();
            this.txtTicketId      = new TextBox();
            this.btnScan          = new RoundedButton();
            this.lblScanResult    = new Label();

            this.Text      = "Ticket Administration";
            this.BackColor = Theme.BgDeep;
            this.ForeColor = Theme.TextPrimary;
            this.Font      = Theme.FontBody;

            // ── Header row ─────────────────────────────────────────────────
            this.lblTitle.Text      = "Ticket Administration";
            this.lblTitle.Font      = Theme.FontPageTitle;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = true;

            this.lblDescription.Text      = "Kelola tiket pengunjung dan lakukan pemindaian untuk verifikasi kehadiran.";
            this.lblDescription.Font      = Theme.FontSmall;
            this.lblDescription.ForeColor = Theme.TextMuted;
            this.lblDescription.BackColor = Color.Transparent;
            this.lblDescription.AutoSize  = true;

            this.btnRefresh.Text         = "↻  Refresh";
            this.btnRefresh.Size         = new Size(130, 42);
            this.btnRefresh.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnRefresh);
            this.btnRefresh.Click       += BtnRefresh_Click;

            // ── Search Bar ──────────────────────────────────────────────────
            this.txtSearch.Size          = new Size(250, 42);
            this.txtSearch.PlaceholderText = "Cari tiket...";
            this.txtSearch.TextChangedEvent += TxtSearch_TextChanged;

            // ── Scan Card ─────────────────────────────────────────────────
            this.pnlScanCard.BackColor = Theme.BgSurface;

            this.lblScanTitle.Text      = "🎫  Scan Tiket";
            this.lblScanTitle.Font      = Theme.FontSubhead;
            this.lblScanTitle.ForeColor = Theme.AccentSecond;
            this.lblScanTitle.BackColor = Color.Transparent;
            this.lblScanTitle.AutoSize  = true;
            this.lblScanTitle.Location  = new Point(Margin, Margin);

            this.lblScanHint.Text      = "Masukkan Ticket ID (GUID) untuk\nverifikasi kehadiran pengunjung.";
            this.lblScanHint.Font      = Theme.FontSmall;
            this.lblScanHint.ForeColor = Theme.TextMuted;
            this.lblScanHint.BackColor = Color.Transparent;
            this.lblScanHint.AutoSize  = true;
            this.lblScanHint.Location  = new Point(Margin, Margin + 38);

            this.lblTicketIdLabel.Text      = "TICKET ID";
            this.lblTicketIdLabel.Font      = Theme.FontLabel;
            this.lblTicketIdLabel.ForeColor = Theme.TextMuted;
            this.lblTicketIdLabel.BackColor = Color.Transparent;
            this.lblTicketIdLabel.AutoSize  = true;
            this.lblTicketIdLabel.Location  = new Point(Margin, Margin + 38 + 54);

            this.txtTicketId.Location       = new Point(Margin, Margin + 38 + 54 + Theme.SpaceSM + 2);
            this.txtTicketId.Size           = new Size(230, 34);
            this.txtTicketId.BorderStyle    = BorderStyle.FixedSingle;
            this.txtTicketId.BackColor      = Theme.BgInput;
            this.txtTicketId.ForeColor      = Theme.TextPrimary;
            this.txtTicketId.Font           = Theme.FontSmall;
            this.txtTicketId.PlaceholderText= "xxxx-xxxx-xxxx-xxxx";

            int scanButtonPositionY = Margin + 38 + 54 + Theme.SpaceSM + 2 + 34 + Theme.SpaceMD;
            this.btnScan.Text         = "Scan & Verifikasi";
            this.btnScan.Size         = new Size(230, 48);
            this.btnScan.Location     = new Point(Margin, scanButtonPositionY);
            this.btnScan.Font         = Theme.FontSubhead;
            this.btnScan.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToAccentButton(this.btnScan);
            this.btnScan.Click       += BtnScan_Click;

            this.dgvTickets.Paint    += (s, e) => Theme.DrawEmptyState(dgvTickets, e, "Tidak ada data tiket yang ditemukan.");

            this.lblScanResult.Text      = "";
            this.lblScanResult.Font      = Theme.FontSmall;
            this.lblScanResult.ForeColor = Theme.TextMuted;
            this.lblScanResult.BackColor = Color.Transparent;
            this.lblScanResult.AutoSize  = false;
            this.lblScanResult.Size      = new Size(230, 40);
            this.lblScanResult.Location  = new Point(Margin, scanButtonPositionY + 56);
            this.lblScanResult.TextAlign = ContentAlignment.MiddleCenter;

            this.pnlScanCard.Controls.AddRange(new Control[] {
                lblScanTitle, lblScanHint, lblTicketIdLabel, txtTicketId, btnScan, lblScanResult
            });

            // ── DataGridView ────────────────────────────────────────────────
            Theme.ApplyToDataGridView(this.dgvTickets);

            this.Controls.AddRange(new Control[] {
                lblTitle, lblDescription, txtSearch, btnRefresh, pnlScanCard, dgvTickets
            });

            this.Resize += (s, e) => RepositionControls();
            RepositionControls();
        }

        private void RepositionControls()
        {
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // Header row
            int headerBlockH = TitleHeight + 8 + DescriptionHeight;
            int rowY = 102;

            this.lblTitle.Location       = new Point(Margin, rowY);
            this.lblDescription.Location = new Point(Margin, rowY + TitleHeight + 8);

            this.btnRefresh.Location = new Point(width - Margin - btnRefresh.Width,
                                                 rowY + (headerBlockH - btnRefresh.Height) / 2);
            this.txtSearch.Location  = new Point(btnRefresh.Left - Theme.SpaceLG - txtSearch.Width,
                                                 rowY + (headerBlockH - txtSearch.Height) / 2);

            // Scan card (right side)
            int cardWidth  = 278;
            int dgvY   = rowY + headerBlockH + 48;
            int dgvH   = Math.Max(0, height - dgvY - Margin);

            this.pnlScanCard.Location = new Point(width - Margin - cardWidth, dgvY);
            this.pnlScanCard.Size     = new Size(cardWidth, dgvH);

            // Grid (left side)
            this.dgvTickets.Location = new Point(Margin, dgvY);
            this.dgvTickets.Size     = new Size(width - cardWidth - Margin * 3, dgvH);
        }

        private async void LoadData()
        {
            try
            {
                _allTickets = await _ticketService.GetMyTicketsAsync();
                FilterData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tickets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterData()
        {
            if (_allTickets == null) return;
            var q = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q))
                dgvTickets.DataSource = _allTickets;
            else
            {
                var filtered = new System.Collections.Generic.List<TicketResponse>();
                foreach (var t in _allTickets)
                {
                    if ((t.TicketType != null && t.TicketType.ToLower().Contains(q)) || 
                        (t.Id.ToString().ToLower().Contains(q)))
                    {
                        filtered.Add(t);
                    }
                }
                dgvTickets.DataSource = filtered;
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e) => FilterData();

        private void BtnRefresh_Click(object? sender, EventArgs e) => LoadData();

        private async void BtnScan_Click(object? sender, EventArgs e)
        {
            if (Guid.TryParse(txtTicketId.Text.Trim(), out Guid ticketId))
            {
                btnScan.Enabled    = false;
                btnScan.Text       = "Memproses...";
                lblScanResult.Text = "";
                try
                {
                    var req    = new ScanTicketRequest { TicketId = ticketId };
                    var result = await _ticketService.ScanTicketAsync(req);
                    if (result == "Success")
                    {
                        lblScanResult.Text      = "✅ Tiket valid!";
                        lblScanResult.ForeColor = Theme.AccentSecond;
                        txtTicketId.Clear();
                        LoadData();
                    }
                    else
                    {
                        lblScanResult.Text      = $"❌ {result}";
                        lblScanResult.ForeColor = Theme.AccentDanger;
                    }
                }
                catch (Exception ex)
                {
                    lblScanResult.Text      = $"❌ {ex.Message}";
                    lblScanResult.ForeColor = Theme.AccentDanger;
                }
                finally
                {
                    btnScan.Enabled = true;
                    btnScan.Text    = "🔍  Scan & Verifikasi";
                }
            }
            else
            {
                lblScanResult.Text      = "⚠️ Format ID tidak valid";
                lblScanResult.ForeColor = Color.FromArgb(255, 200, 100);
            }
        }
    }
}
