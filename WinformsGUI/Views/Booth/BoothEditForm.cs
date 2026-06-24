#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Booth
{
    public class BoothEditForm : Form
    {
        private BoothResponse? _existingBooth;
        private BoothService   _boothService;

        private Label         lblTitle;
        private Label         lblSubtitle;
        private Panel         pnlDivider;
        private Label         lblName;
        private TextBox       txtName;
        private Label         lblDescription;
        private TextBox       txtDescription;
        private Label         lblLocation;
        private TextBox       txtLocation;
        private RoundedButton btnSave;
        private RoundedButton btnCancel;

        public BoothEditForm(BoothResponse? booth)
        {
            _existingBooth = booth;
            _boothService  = new BoothService();
            InitializeComponent();

            if (_existingBooth != null)
            {
                lblTitle.Text       = "Edit Booth";
                lblSubtitle.Text    = $"Memperbarui data: {_existingBooth.Name}";
                txtName.Text        = _existingBooth.Name;
                txtDescription.Text = _existingBooth.Description;
                txtLocation.Text    = _existingBooth.Location;
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblSubtitle    = new Label();
            this.pnlDivider     = new Panel();
            this.lblName        = new Label();
            this.txtName        = new TextBox();
            this.lblDescription = new Label();
            this.txtDescription = new TextBox();
            this.lblLocation    = new Label();
            this.txtLocation    = new TextBox();
            this.btnSave        = new RoundedButton();
            this.btnCancel      = new RoundedButton();

            this.Text            = _existingBooth == null ? "Tambah Booth" : "Edit Booth";
            this.Size            = new Size(460, 510);
            this.BackColor       = Theme.BgSurface;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;

            int x = Theme.SpaceLG, w = 380;
            int y = Theme.SpaceLG;

            // Title
            this.lblTitle.Text      = "Tambah Booth Baru";
            this.lblTitle.Font      = Theme.FontHeader;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = true;
            this.lblTitle.Location  = new Point(x, y);
            y += 38;

            this.lblSubtitle.Text      = "Isi detail informasi booth";
            this.lblSubtitle.Font      = Theme.FontSmall;
            this.lblSubtitle.ForeColor = Theme.TextMuted;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.AutoSize  = true;
            this.lblSubtitle.Location  = new Point(x, y);
            y += 28;

            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(w, 1);
            this.pnlDivider.Location  = new Point(x, y);
            y += 1 + Theme.SpaceMD;

            // Name
            MakeFieldLabel(lblName, "NAMA BOOTH", x, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtName, x, y, w, 38); y += 38 + Theme.SpaceMD;

            // Description
            MakeFieldLabel(lblDescription, "DESKRIPSI", x, y); y += Theme.SpaceSM + 2;
            this.txtDescription.Location    = new Point(x, y);
            this.txtDescription.Size        = new Size(w, 76);
            this.txtDescription.Multiline   = true;
            this.txtDescription.BorderStyle = BorderStyle.FixedSingle;
            this.txtDescription.BackColor   = Theme.BgInput;
            this.txtDescription.ForeColor   = Theme.TextPrimary;
            this.txtDescription.Font        = Theme.FontBody;
            y += 76 + Theme.SpaceMD;

            // Location
            MakeFieldLabel(lblLocation, "LOKASI", x, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtLocation, x, y, w, 38); y += 38 + Theme.SpaceLG;

            // Buttons
            int btnW = (w - Theme.SpaceSM) / 2;
            this.btnSave.Text         = "Simpan";
            this.btnSave.Size         = new Size(btnW, 48);
            this.btnSave.Location     = new Point(x, y);
            this.btnSave.Font         = Theme.FontSubhead;
            this.btnSave.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnSave);
            this.btnSave.Click       += BtnSave_Click;

            this.btnCancel.Text         = "Batal";
            this.btnCancel.Size         = new Size(btnW, 48);
            this.btnCancel.Location     = new Point(x + btnW + Theme.SpaceSM, y);
            this.btnCancel.Font         = Theme.FontSubhead;
            this.btnCancel.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnCancel);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, pnlDivider,
                lblName, txtName, lblDescription, txtDescription, lblLocation, txtLocation,
                btnSave, btnCancel
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

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            btnSave.Enabled = false; btnSave.Text = "Menyimpan...";
            try
            {
                if (_existingBooth == null)
                    await _boothService.CreateBoothAsync(new CreateBoothRequest { Name = txtName.Text, Description = txtDescription.Text, Location = txtLocation.Text });
                else
                    await _boothService.UpdateBoothAsync(_existingBooth.Id, new UpdateBoothRequest { Name = txtName.Text, Description = txtDescription.Text, Location = txtLocation.Text });
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = true; btnSave.Text = "Simpan";
            }
        }
    }
}
