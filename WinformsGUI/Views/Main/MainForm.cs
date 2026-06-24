using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Main
{
    public class MainForm : Form
    {
        private Panel  pnlSidebar;
        private Panel  pnlContent;
        private PictureBox picLogo;
        private Label  lblAppName;
        private Label  lblAppSub;
        private Panel  pnlDividerTop;
        private Label  lblNavSection;
        private Button btnBooths;
        private Button btnTalkshows;
        private Button btnTickets;
        private Panel  pnlDividerBottom;
        private Button btnLogout;
        private Label  lblFooter;

        private Button? _activeBtn;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pnlSidebar       = new Panel();
            this.pnlContent       = new Panel();
            this.picLogo          = new PictureBox();
            this.lblAppName       = new Label();
            this.lblAppSub        = new Label();
            this.pnlDividerTop    = new Panel();
            this.lblNavSection    = new Label();
            this.btnBooths        = new Button();
            this.btnTalkshows     = new Button();
            this.btnTickets       = new Button();
            this.pnlDividerBottom = new Panel();
            this.btnLogout        = new Button();
            this.lblFooter        = new Label();

            // ── Form ────────────────────────────────────────────────────────
            this.Text          = "Malaka Book Fest — Dashboard";
            this.Size          = new Size(1200, 800);
            this.WindowState   = FormWindowState.Maximized;
            this.BackColor     = Theme.BgDeep;
            this.ForeColor     = Theme.TextPrimary;
            this.Font          = Theme.FontBody;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── Sidebar (280px) ─────────────────────────────────────────────
            this.pnlSidebar.Dock      = DockStyle.Left;
            this.pnlSidebar.Width     = 280;
            this.pnlSidebar.BackColor = Theme.BgSurface;

            // Logo
            this.picLogo.Image     = Image.FromFile(System.IO.Path.Combine(Application.StartupPath, "Assets", "logo.jpg"));
            this.picLogo.SizeMode  = PictureBoxSizeMode.Zoom;
            this.picLogo.Size      = new Size(64, 64);
            this.picLogo.Location  = new Point(Theme.SpaceLG, Theme.SpaceLG);
            this.picLogo.BackColor = Color.Transparent;

            // App Name
            this.lblAppName.Text      = "Malaka Book Fest";
            this.lblAppName.Font      = Theme.FontHeader;
            this.lblAppName.ForeColor = Theme.TextPrimary;
            this.lblAppName.BackColor = Color.Transparent;
            this.lblAppName.AutoSize  = true;
            this.lblAppName.Location  = new Point(Theme.SpaceLG, picLogo.Bottom + 10);

            // App Subtitle
            this.lblAppSub.Text      = "Management System";
            this.lblAppSub.Font      = Theme.FontSmall;
            this.lblAppSub.ForeColor = Theme.TextMuted;
            this.lblAppSub.BackColor = Color.Transparent;
            this.lblAppSub.AutoSize  = true;
            this.lblAppSub.Location  = new Point(Theme.SpaceLG, lblAppName.Bottom + 5);

            // Top Divider
            this.pnlDividerTop.Location  = new Point(Theme.SpaceLG, lblAppSub.Bottom + 20);
            this.pnlDividerTop.Size      = new Size(228, 1);
            this.pnlDividerTop.BackColor = Theme.BorderSoft;

            // Nav Section Label
            this.lblNavSection.Text      = "NAVIGASI";
            this.lblNavSection.Font      = Theme.FontSmall;
            this.lblNavSection.ForeColor = Theme.TextMuted;
            this.lblNavSection.BackColor = Color.Transparent;
            this.lblNavSection.AutoSize  = true;
            this.lblNavSection.Location  = new Point(Theme.SpaceLG, pnlDividerTop.Bottom + 15);

            // Navigation Buttons
            int startY = lblNavSection.Bottom + 15;
            SetupNavButton(btnBooths,    "🏪  Booths",    startY, BtnBooths_Click);
            SetupNavButton(btnTalkshows, "🎤  Talkshows", startY + 54, BtnTalkshows_Click);
            SetupNavButton(btnTickets,   "🎫  Tickets",   startY + 108, BtnTickets_Click);

            // Bottom Divider
            this.pnlDividerBottom.Size      = new Size(228, 1);
            this.pnlDividerBottom.BackColor = Theme.BorderSoft;
            this.pnlDividerBottom.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left;
            this.pnlDividerBottom.Location  = new Point(Theme.SpaceLG, 720);

            // Logout Button
            this.btnLogout.Text     = "🚪  Logout";
            this.btnLogout.Size     = new Size(228, 44);
            this.btnLogout.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnLogout.Location = new Point(Theme.SpaceLG, 732);
            this.btnLogout.FlatStyle = FlatStyle.Flat;
            this.btnLogout.BackColor = Color.Transparent;
            this.btnLogout.ForeColor = Theme.AccentDanger;
            this.btnLogout.FlatAppearance.BorderSize         = 0;
            this.btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 224, 123, 123);
            this.btnLogout.Font      = Theme.FontLabel;
            this.btnLogout.Cursor    = Cursors.Hand;
            this.btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            this.btnLogout.Padding   = new Padding(10, 0, 0, 0);
            this.btnLogout.Click    += BtnLogout_Click;

            // Footer
            this.lblFooter.Text      = "v1.0.0";
            this.lblFooter.Font      = Theme.FontSmall;
            this.lblFooter.ForeColor = Theme.TextMuted;
            this.lblFooter.BackColor = Color.Transparent;
            this.lblFooter.AutoSize  = true;
            this.lblFooter.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left;
            this.lblFooter.Location  = new Point(Theme.SpaceLG, 790);

            // Compose Sidebar
            this.pnlSidebar.Controls.AddRange(new Control[] {
                picLogo, lblAppName, lblAppSub, pnlDividerTop, lblNavSection,
                btnBooths, btnTalkshows, btnTickets,
                pnlDividerBottom, btnLogout, lblFooter
            });

            // ── Content Panel ───────────────────────────────────────────────
            this.pnlContent.Dock      = DockStyle.Fill;
            this.pnlContent.BackColor = Theme.BgDeep;

            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSidebar);

            this.Load += (s, e) => ShowFormInContent(new DashboardHomeForm());
        }

        private void SetupNavButton(Button btn, string text, int y, EventHandler click)
        {
            btn.Text      = text;
            btn.Location  = new Point(Theme.SpaceLG, y);
            btn.Size      = new Size(228, 46);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Theme.TextSecondary;
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, Theme.AccentPrimary);
            btn.Font      = Theme.FontSubhead;
            btn.Cursor    = Cursors.Hand;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding   = new Padding(10, 0, 0, 0);
            btn.Click    += click;
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeBtn != null)
            {
                _activeBtn.BackColor = Color.Transparent;
                _activeBtn.ForeColor = Theme.TextSecondary;
            }
            btn.BackColor = Theme.AccentPrimary;
            btn.ForeColor = Color.White;
            _activeBtn    = btn;
        }

        private void ShowFormInContent(Form childForm)
        {
            this.pnlContent.Controls.Clear();
            childForm.TopLevel        = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock            = DockStyle.Fill;
            this.pnlContent.Controls.Add(childForm);
            childForm.Show();
        }

        private void BtnBooths_Click(object? sender, EventArgs e)
        {
            SetActiveButton(btnBooths);
            ShowFormInContent(new Views.Booth.BoothListForm());
        }

        private void BtnTalkshows_Click(object? sender, EventArgs e)
        {
            SetActiveButton(btnTalkshows);
            ShowFormInContent(new Views.Talkshow.TalkshowListForm());
        }

        private void BtnTickets_Click(object? sender, EventArgs e)
        {
            SetActiveButton(btnTickets);
            ShowFormInContent(new Views.Ticket.TicketAdminForm());
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            Config.JwtToken = string.Empty;
            var loginForm = new Auth.LoginForm();
            this.Hide();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}
