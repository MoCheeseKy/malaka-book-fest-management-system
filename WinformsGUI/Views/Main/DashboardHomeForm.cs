using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Main
{
    public class DashboardHomeForm : Form
    {
        private Label lblGreeting;
        private Label lblSubtitle;
        private Label lblIcon;

        public DashboardHomeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Theme.BgDeep;
            this.ForeColor = Theme.TextPrimary;

            lblIcon = new Label();
            lblIcon.Text = "👋";
            lblIcon.Font = new Font("Segoe UI", 48F);
            lblIcon.AutoSize = true;
            lblIcon.BackColor = Color.Transparent;

            lblGreeting = new Label();
            lblGreeting.Text = "Selamat Datang di Malaka Book Fest!";
            lblGreeting.Font = Theme.FontTitle;
            lblGreeting.ForeColor = Theme.AccentPrimary;
            lblGreeting.AutoSize = true;
            lblGreeting.BackColor = Color.Transparent;

            lblSubtitle = new Label();
            lblSubtitle.Text = "Pilih salah satu menu navigasi di sebelah kiri untuk memulai pengelolaan acara.";
            lblSubtitle.Font = Theme.FontBody;
            lblSubtitle.ForeColor = Theme.TextMuted;
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = Color.Transparent;

            this.Controls.Add(lblIcon);
            this.Controls.Add(lblGreeting);
            this.Controls.Add(lblSubtitle);

            this.Resize += DashboardHomeForm_Resize;
            this.Load   += DashboardHomeForm_Load;
        }

        private void DashboardHomeForm_Load(object? sender, EventArgs e)
        {
            PositionControls();
        }

        private void DashboardHomeForm_Resize(object? sender, EventArgs e)
        {
            PositionControls();
        }

        private void PositionControls()
        {
            lblIcon.Location = new Point((this.ClientSize.Width - lblIcon.Width) / 2, (this.ClientSize.Height / 2) - lblIcon.Height - 30);
            lblGreeting.Location = new Point((this.ClientSize.Width - lblGreeting.Width) / 2, lblIcon.Bottom + 10);
            lblSubtitle.Location = new Point((this.ClientSize.Width - lblSubtitle.Width) / 2, lblGreeting.Bottom + 10);
        }
    }
}
