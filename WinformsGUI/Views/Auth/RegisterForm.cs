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

            int margin = Theme.SpaceLG, cardWidth = this.ClientSize.Width - margin * 2, cardHeight = 500;
            int cardPositionY = (this.ClientSize.Height - cardHeight) / 2;

            this.pnlCard.Location = new Point(margin, cardPositionY);
            this.pnlCard.Size     = new Size(cardWidth, cardHeight);
            this.pnlCard.BackColor= Theme.BgCard;
            this.Controls.Add(this.pnlCard);

            int contentX = Theme.SpaceLG, contentWidth = cardWidth - Theme.SpaceLG * 2, positionY = Theme.SpaceXL;

            this.lblTitle.Text      = "Buat Akun Baru";
            this.lblTitle.Font      = Theme.FontTitle;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Size      = new Size(contentWidth, 46);
            this.lblTitle.Location  = new Point(contentX, positionY);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            positionY += 46 + 4;

            this.lblSubtitle.Text      = "Daftar untuk mengakses sistem";
            this.lblSubtitle.Font      = Theme.FontBody;
            this.lblSubtitle.ForeColor = Theme.TextMuted;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.AutoSize  = false;
            this.lblSubtitle.Size      = new Size(contentWidth, 24);
            this.lblSubtitle.Location  = new Point(contentX, positionY);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            positionY += 24 + Theme.SpaceXL;

            MakeFieldLabel(lblEmail, "EMAIL", contentX, positionY); positionY += Theme.SpaceSM + 2;
            MakeTextBox(txtEmail, contentX, positionY, contentWidth, 38); positionY += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblPassword, "PASSWORD", contentX, positionY); positionY += Theme.SpaceSM + 2;
            MakeTextBox(txtPassword, contentX, positionY, contentWidth, 38); txtPassword.PasswordChar = '•'; positionY += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblRole, "ROLE", contentX, positionY); positionY += Theme.SpaceSM + 2;
            this.cmbRole.Location     = new Point(contentX, positionY);
            this.cmbRole.Size         = new Size(contentWidth, 38);
            this.cmbRole.DropDownStyle= ComboBoxStyle.DropDownList;
            this.cmbRole.BackColor    = Theme.BgInput;
            this.cmbRole.ForeColor    = Theme.TextPrimary;
            this.cmbRole.Font         = Theme.FontBody;
            this.cmbRole.FlatStyle    = FlatStyle.Flat;
            this.cmbRole.Items.AddRange(new object[] { "Admin", "User", "Tenant" });
            this.cmbRole.SelectedIndex= 1;
            positionY += 38 + Theme.SpaceXL;

            this.btnRegister.Text         = "Daftar";
            this.btnRegister.Size         = new Size(contentWidth, 48);
            this.btnRegister.Location     = new Point(contentX, positionY);
            this.btnRegister.Font         = Theme.FontSubhead;
            this.btnRegister.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnRegister);
            this.btnRegister.Click       += BtnRegister_Click;
            positionY += 48 + Theme.SpaceMD;

            this.lnkLogin.Text      = "Sudah punya akun? Masuk di sini";
            this.lnkLogin.Font      = Theme.FontSmall;
            this.lnkLogin.Location  = new Point(contentX, positionY);
            this.lnkLogin.Size      = new Size(contentWidth, 24);
            this.lnkLogin.TextAlign = ContentAlignment.MiddleCenter;
            Theme.ApplyToLinkLabel(this.lnkLogin);
            this.lnkLogin.LinkClicked += LnkLogin_LinkClicked;

            this.pnlCard.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, lblEmail, txtEmail, lblPassword, txtPassword,
                lblRole, cmbRole, btnRegister, lnkLogin
            });
        }

        private void MakeFieldLabel(Label lbl, string text, int positionX, int positionY)
        {
            lbl.Text      = text;
            lbl.Font      = Theme.FontLabel;
            lbl.ForeColor = Theme.TextMuted;
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize  = true;
            lbl.Location  = new Point(positionX, positionY);
        }

        private void MakeTextBox(TextBox txt, int positionX, int positionY, int width, int height)
        {
            txt.Location    = new Point(positionX, positionY);
            txt.Size        = new Size(width, height);
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
