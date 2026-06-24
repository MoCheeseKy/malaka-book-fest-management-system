#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Auth
{
    public class RegisterForm : Form
    {
        private AuthService _authService;

        private Panel         pnlCard;
        private Label         lblTitle;
        private Label         lblSubtitle;
        private Label         lblEmail;     private TextBox txtEmail;
        private Label         lblPassword;  private TextBox txtPassword;
        private Label         lblRole;      private ComboBox cmbRole;
        private RoundedButton btnRegister;
        private LinkLabel     lnkLogin;

        public RegisterForm()
        {
            _authService = new AuthService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pnlCard     = new Panel();
            this.lblTitle    = new Label();
            this.lblSubtitle = new Label();
            this.lblEmail    = new Label(); this.txtEmail    = new TextBox();
            this.lblPassword = new Label(); this.txtPassword = new TextBox();
            this.lblRole     = new Label(); this.cmbRole     = new ComboBox();
            this.btnRegister = new RoundedButton();
            this.lnkLogin    = new LinkLabel();

            this.Text            = "Register — Malaka Book Fest";
            this.Size            = new Size(460, 620);
            this.BackColor       = Theme.BgDeep;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;

            int m = Theme.SpaceLG, w = this.ClientSize.Width - m * 2, cardH = 500;
            int cy = (this.ClientSize.Height - cardH) / 2;

            this.pnlCard.Location = new Point(m, cy);
            this.pnlCard.Size     = new Size(w, cardH);
            this.pnlCard.BackColor= Theme.BgCard;
            this.Controls.Add(this.pnlCard);

            int cx = Theme.SpaceLG, cw = w - Theme.SpaceLG * 2, y = Theme.SpaceXL;

            this.lblTitle.Text      = "Buat Akun Baru";
            this.lblTitle.Font      = Theme.FontTitle;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Size      = new Size(cw, 46);
            this.lblTitle.Location  = new Point(cx, y);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            y += 46 + 4;

            this.lblSubtitle.Text      = "Daftar untuk mengakses sistem";
            this.lblSubtitle.Font      = Theme.FontBody;
            this.lblSubtitle.ForeColor = Theme.TextMuted;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.AutoSize  = false;
            this.lblSubtitle.Size      = new Size(cw, 24);
            this.lblSubtitle.Location  = new Point(cx, y);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            y += 24 + Theme.SpaceXL;

            MakeFieldLabel(lblEmail, "EMAIL", cx, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtEmail, cx, y, cw, 38); y += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblPassword, "PASSWORD", cx, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtPassword, cx, y, cw, 38); txtPassword.PasswordChar = '•'; y += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblRole, "ROLE", cx, y); y += Theme.SpaceSM + 2;
            this.cmbRole.Location     = new Point(cx, y);
            this.cmbRole.Size         = new Size(cw, 38);
            this.cmbRole.DropDownStyle= ComboBoxStyle.DropDownList;
            this.cmbRole.BackColor    = Theme.BgInput;
            this.cmbRole.ForeColor    = Theme.TextPrimary;
            this.cmbRole.Font         = Theme.FontBody;
            this.cmbRole.FlatStyle    = FlatStyle.Flat;
            this.cmbRole.Items.AddRange(new object[] { "Admin", "User", "Tenant" });
            this.cmbRole.SelectedIndex= 1;
            y += 38 + Theme.SpaceXL;

            this.btnRegister.Text         = "Daftar";
            this.btnRegister.Size         = new Size(cw, 48);
            this.btnRegister.Location     = new Point(cx, y);
            this.btnRegister.Font         = Theme.FontSubhead;
            this.btnRegister.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnRegister);
            this.btnRegister.Click       += BtnRegister_Click;
            y += 48 + Theme.SpaceMD;

            this.lnkLogin.Text      = "Sudah punya akun? Masuk di sini";
            this.lnkLogin.Font      = Theme.FontSmall;
            this.lnkLogin.Location  = new Point(cx, y);
            this.lnkLogin.Size      = new Size(cw, 24);
            this.lnkLogin.TextAlign = ContentAlignment.MiddleCenter;
            Theme.ApplyToLinkLabel(this.lnkLogin);
            this.lnkLogin.LinkClicked += LnkLogin_LinkClicked;

            this.pnlCard.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, lblEmail, txtEmail, lblPassword, txtPassword,
                lblRole, cmbRole, btnRegister, lnkLogin
            });
        }

        private void MakeFieldLabel(Label lbl, string text, int x, int y)
        {
            lbl.Text      = text;
            lbl.Font      = Theme.FontLabel;
            lbl.ForeColor = Theme.TextMuted;
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize  = true;
            lbl.Location  = new Point(x, y);
        }

        private void MakeTextBox(TextBox txt, int x, int y, int w, int h)
        {
            txt.Location    = new Point(x, y);
            txt.Size        = new Size(w, h);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor   = Theme.BgInput;
            txt.ForeColor   = Theme.TextPrimary;
            txt.Font        = Theme.FontBody;
        }

        private async void BtnRegister_Click(object? sender, EventArgs e)
        {
            btnRegister.Enabled = false; btnRegister.Text = "Memproses...";
            try
            {
                var req = new RegisterRequest { Email = txtEmail.Text, Password = txtPassword.Text, Username = cmbRole.Text };
                await _authService.RegisterAsync(req);
                MessageBox.Show("Registrasi berhasil. Silakan login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LnkLogin_LinkClicked(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Register Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRegister.Enabled = true; btnRegister.Text = "Daftar";
            }
        }

        private void LnkLogin_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs? e)
        {
            var loginForm = new LoginForm();
            this.Hide();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}
