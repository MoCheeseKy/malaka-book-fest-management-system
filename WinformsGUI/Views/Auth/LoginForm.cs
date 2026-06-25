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
    public class LoginForm : Form
    {
        private AuthService _authService;

        private Panel         pnlCard;
        private Label         lblAppName;
        private Label         lblTagline;
        private Label         lblEmail;
        private TextBox       txtEmail;
        private Label         lblPassword;
        private TextBox       txtPassword;
        private RoundedButton btnLogin;
        private LinkLabel     lnkRegister;

        public LoginForm()
        {
            _authService = new AuthService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pnlCard     = new Panel();
            this.lblAppName  = new Label();
            this.lblTagline  = new Label();
            this.lblEmail    = new Label();
            this.txtEmail    = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin    = new RoundedButton();
            this.lnkRegister = new LinkLabel();

            this.Text            = "Login — Malaka Book Fest";
            this.Size            = new Size(460, 560);
            this.BackColor       = Theme.BgDeep;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;

            int margin = Theme.SpaceLG;
            int cardWidth = this.ClientSize.Width - margin * 2;
            int cardHeight = 430;
            int cardPositionY = (this.ClientSize.Height - cardHeight) / 2;

            // Card Panel
            this.pnlCard.Location = new Point(margin, cardPositionY);
            this.pnlCard.Size     = new Size(cardWidth, cardHeight);
            this.pnlCard.BackColor= Theme.BgCard;
            this.Controls.Add(this.pnlCard);

            int contentX = Theme.SpaceLG, contentWidth = cardWidth - Theme.SpaceLG * 2, positionY = Theme.SpaceXL;

            // App Name
            this.lblAppName.Text      = "📚 Malaka Book Fest";
            this.lblAppName.Font      = Theme.FontTitle;
            this.lblAppName.ForeColor = Theme.TextPrimary;
            this.lblAppName.BackColor = Color.Transparent;
            this.lblAppName.AutoSize  = false;
            this.lblAppName.Size      = new Size(contentWidth, 46);
            this.lblAppName.Location  = new Point(contentX, positionY);
            this.lblAppName.TextAlign = ContentAlignment.MiddleCenter;
            positionY += 46 + 4;

            // Tagline
            this.lblTagline.Text      = "Silakan masuk untuk melanjutkan";
            this.lblTagline.Font      = Theme.FontBody;
            this.lblTagline.ForeColor = Theme.TextMuted;
            this.lblTagline.BackColor = Color.Transparent;
            this.lblTagline.AutoSize  = false;
            this.lblTagline.Size      = new Size(contentWidth, 24);
            this.lblTagline.Location  = new Point(contentX, positionY);
            this.lblTagline.TextAlign = ContentAlignment.MiddleCenter;
            positionY += 24 + Theme.SpaceXL;

            // Email
            this.lblEmail.Text      = "EMAIL";
            this.lblEmail.Font      = Theme.FontLabel;
            this.lblEmail.ForeColor = Theme.TextMuted;
            this.lblEmail.BackColor = Color.Transparent;
            this.lblEmail.AutoSize  = true;
            this.lblEmail.Location  = new Point(contentX, positionY); positionY += Theme.SpaceSM + 2;

            this.txtEmail.Location    = new Point(contentX, positionY);
            this.txtEmail.Size        = new Size(contentWidth, 38);
            this.txtEmail.BorderStyle = BorderStyle.FixedSingle;
            this.txtEmail.BackColor   = Theme.BgInput;
            this.txtEmail.ForeColor   = Theme.TextPrimary;
            this.txtEmail.Font        = Theme.FontBody;
            positionY += 38 + Theme.SpaceMD;

            // Password
            this.lblPassword.Text      = "PASSWORD";
            this.lblPassword.Font      = Theme.FontLabel;
            this.lblPassword.ForeColor = Theme.TextMuted;
            this.lblPassword.BackColor = Color.Transparent;
            this.lblPassword.AutoSize  = true;
            this.lblPassword.Location  = new Point(contentX, positionY); positionY += Theme.SpaceSM + 2;

            this.txtPassword.Location    = new Point(contentX, positionY);
            this.txtPassword.Size        = new Size(contentWidth, 38);
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            this.txtPassword.BackColor   = Theme.BgInput;
            this.txtPassword.ForeColor   = Theme.TextPrimary;
            this.txtPassword.Font        = Theme.FontBody;
            this.txtPassword.PasswordChar= '•';
            positionY += 38 + Theme.SpaceXL;

            // Login Button
            this.btnLogin.Text         = "Masuk";
            this.btnLogin.Size         = new Size(contentWidth, 48);
            this.btnLogin.Location     = new Point(contentX, positionY);
            this.btnLogin.Font         = Theme.FontSubhead;
            this.btnLogin.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnLogin);
            this.btnLogin.Click       += BtnLogin_Click;
            positionY += 48 + Theme.SpaceMD;

            // Register Link
            this.lnkRegister.Text      = "Belum punya akun? Daftar sekarang";
            this.lnkRegister.Font      = Theme.FontSmall;
            this.lnkRegister.Location  = new Point(contentX, positionY);
            this.lnkRegister.Size      = new Size(contentWidth, 24);
            this.lnkRegister.TextAlign = ContentAlignment.MiddleCenter;
            Theme.ApplyToLinkLabel(this.lnkRegister);
            this.lnkRegister.LinkClicked += LnkRegister_LinkClicked;

            this.pnlCard.Controls.AddRange(new Control[] {
                lblAppName, lblTagline, lblEmail, txtEmail, lblPassword, txtPassword, btnLogin, lnkRegister
            });
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            btnLogin.Enabled = false; btnLogin.Text = "Memproses...";
            try
            {
                var req = new LoginRequest { Email = txtEmail.Text, Password = txtPassword.Text };
                var res = await _authService.LoginAsync(req);
                Config.JwtToken = res.Token;

                var mainForm = new Views.Main.MainForm();
                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true; btnLogin.Text = "Masuk";
            }
        }

        private void LnkRegister_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            var registerForm = new RegisterForm();
            this.Hide();
            registerForm.ShowDialog();
            this.Close();
        }
    }
}
