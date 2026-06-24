#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Talkshow
{
    public class TalkshowEditForm : Form
    {
        private TalkshowResponse? _existing;
        private TalkshowService   _service;

        private Label         lblTitle;
        private Label         lblSubtitle;
        private Panel         pnlDivider;

        private Label         lblTsTitle;     private TextBox txtTsTitle;
        private Label         lblSpeakerName; private TextBox txtSpeakerName;
        private Label         lblSpeakerBio;  private TextBox txtSpeakerBio;
        private Label         lblVenue;       private TextBox txtVenue;
        private Label         lblCapacity;    private NumericUpDown numCapacity;
        private Label         lblStartTime;   private DateTimePicker dtpStartTime;
        private Label         lblEndTime;     private DateTimePicker dtpEndTime;
        private Label         lblStatus;      private ComboBox cmbStatus;

        private RoundedButton btnSave;
        private RoundedButton btnCancel;

        public TalkshowEditForm(TalkshowResponse? talkshow)
        {
            _existing = talkshow;
            _service  = new TalkshowService();
            InitializeComponent();
            PopulateData();
        }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblSubtitle    = new Label();
            this.pnlDivider     = new Panel();

            this.lblTsTitle     = new Label(); this.txtTsTitle     = new TextBox();
            this.lblSpeakerName = new Label(); this.txtSpeakerName = new TextBox();
            this.lblSpeakerBio  = new Label(); this.txtSpeakerBio  = new TextBox();
            this.lblVenue       = new Label(); this.txtVenue       = new TextBox();
            this.lblCapacity    = new Label(); this.numCapacity    = new NumericUpDown();
            this.lblStartTime   = new Label(); this.dtpStartTime   = new DateTimePicker();
            this.lblEndTime     = new Label(); this.dtpEndTime     = new DateTimePicker();
            this.lblStatus      = new Label(); this.cmbStatus      = new ComboBox();

            this.btnSave        = new RoundedButton();
            this.btnCancel      = new RoundedButton();

            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).BeginInit();

            this.Text            = _existing == null ? "Tambah Talkshow" : "Edit Talkshow";
            this.Size            = new Size(500, 780);
            this.BackColor       = Theme.BgSurface;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;

            int m = Theme.SpaceLG, w = 420, y = m;

            // Title
            this.lblTitle.Text      = "Form Talkshow";
            this.lblTitle.Font      = Theme.FontHeader;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = true;
            this.lblTitle.Location  = new Point(m, y); y += 38;

            this.lblSubtitle.Text      = "Isi detail acara talkshow dan pembicara";
            this.lblSubtitle.Font      = Theme.FontSmall;
            this.lblSubtitle.ForeColor = Theme.TextMuted;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.AutoSize  = true;
            this.lblSubtitle.Location  = new Point(m, y); y += 28;

            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(w, 1);
            this.pnlDivider.Location  = new Point(m, y); y += 1 + Theme.SpaceMD;

            // Fields
            MakeFieldLabel(lblTsTitle, "JUDUL ACARA", m, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtTsTitle, m, y, w, 38); y += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblSpeakerName, "NAMA PEMBICARA", m, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtSpeakerName, m, y, w, 38); y += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblSpeakerBio, "BIOGRAFI PEMBICARA", m, y); y += Theme.SpaceSM + 2;
            this.txtSpeakerBio.Location    = new Point(m, y);
            this.txtSpeakerBio.Size        = new Size(w, 76);
            this.txtSpeakerBio.Multiline   = true;
            this.txtSpeakerBio.BorderStyle = BorderStyle.FixedSingle;
            this.txtSpeakerBio.BackColor   = Theme.BgInput;
            this.txtSpeakerBio.ForeColor   = Theme.TextPrimary;
            this.txtSpeakerBio.Font        = Theme.FontBody;
            y += 76 + Theme.SpaceMD;

            MakeFieldLabel(lblVenue, "VENUE / RUANGAN", m, y); y += Theme.SpaceSM + 2;
            MakeTextBox(txtVenue, m, y, w, 38); y += 38 + Theme.SpaceMD;

            // Waktu Row (Start - End)
            int colW = (w - Theme.SpaceSM) / 2;
            MakeFieldLabel(lblStartTime, "WAKTU MULAI", m, y);
            MakeFieldLabel(lblEndTime, "WAKTU SELESAI", m + colW + Theme.SpaceSM, y);
            y += Theme.SpaceSM + 2;

            this.dtpStartTime.Location     = new Point(m, y);
            this.dtpStartTime.Size         = new Size(colW, 38);
            this.dtpStartTime.Format       = DateTimePickerFormat.Custom;
            this.dtpStartTime.CustomFormat = "dd MMM yyyy HH:mm";
            Theme.ApplyToDateTimePicker(this.dtpStartTime);

            this.dtpEndTime.Location     = new Point(m + colW + Theme.SpaceSM, y);
            this.dtpEndTime.Size         = new Size(colW, 38);
            this.dtpEndTime.Format       = DateTimePickerFormat.Custom;
            this.dtpEndTime.CustomFormat = "dd MMM yyyy HH:mm";
            Theme.ApplyToDateTimePicker(this.dtpEndTime);
            y += 38 + Theme.SpaceMD;

            // Cap + Status Row
            MakeFieldLabel(lblCapacity, "KAPASITAS PESERTA", m, y);
            MakeFieldLabel(lblStatus, "STATUS ACARA", m + colW + Theme.SpaceSM, y);
            y += Theme.SpaceSM + 2;

            this.numCapacity.Location = new Point(m, y);
            this.numCapacity.Size     = new Size(colW, 38);
            this.numCapacity.Maximum  = 10000M;
            Theme.ApplyToNumericUpDown(this.numCapacity);

            this.cmbStatus.Location     = new Point(m + colW + Theme.SpaceSM, y);
            this.cmbStatus.Size         = new Size(colW, 38);
            this.cmbStatus.DropDownStyle= ComboBoxStyle.DropDownList;
            this.cmbStatus.BackColor    = Theme.BgInput;
            this.cmbStatus.ForeColor    = Theme.TextPrimary;
            this.cmbStatus.Font         = Theme.FontBody;
            this.cmbStatus.FlatStyle    = FlatStyle.Flat;
            this.cmbStatus.Items.AddRange(new object[] { "0 - Scheduled", "1 - Ongoing", "2 - Completed" });
            this.cmbStatus.SelectedIndex= 0;
            y += 38 + Theme.SpaceLG;

            // Buttons
            this.btnSave.Text         = "Simpan";
            this.btnSave.Size         = new Size(colW, 48);
            this.btnSave.Location     = new Point(m, y);
            this.btnSave.Font         = Theme.FontSubhead;
            this.btnSave.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnSave);
            this.btnSave.Click       += BtnSave_Click;

            this.btnCancel.Text         = "Batal";
            this.btnCancel.Size         = new Size(colW, 48);
            this.btnCancel.Location     = new Point(m + colW + Theme.SpaceSM, y);
            this.btnCancel.Font         = Theme.FontSubhead;
            this.btnCancel.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnCancel);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, pnlDivider,
                lblTsTitle, txtTsTitle, lblSpeakerName, txtSpeakerName, lblSpeakerBio, txtSpeakerBio,
                lblVenue, txtVenue, lblStartTime, dtpStartTime, lblEndTime, dtpEndTime,
                lblCapacity, numCapacity, lblStatus, cmbStatus,
                btnSave, btnCancel
            });

            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).EndInit();
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

        private void PopulateData()
        {
            if (_existing == null) return;
            txtTsTitle.Text     = _existing.Title;
            txtSpeakerName.Text = _existing.SpeakerName;
            txtSpeakerBio.Text  = _existing.SpeakerBio;
            txtVenue.Text       = _existing.Venue;
            dtpStartTime.Value  = _existing.StartTime;
            dtpEndTime.Value    = _existing.EndTime;
            numCapacity.Value   = _existing.MaxCapacity;
            if (_existing.Status >= 0 && _existing.Status < cmbStatus.Items.Count)
                cmbStatus.SelectedIndex = _existing.Status;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            btnSave.Enabled = false; btnSave.Text = "Menyimpan...";
            try
            {
                if (_existing == null)
                {
                    await _service.CreateTalkshowAsync(new CreateTalkshowRequest
                    {
                        Title       = txtTsTitle.Text,
                        SpeakerName = txtSpeakerName.Text,
                        SpeakerBio  = txtSpeakerBio.Text,
                        Venue       = txtVenue.Text,
                        StartTime   = dtpStartTime.Value,
                        EndTime     = dtpEndTime.Value,
                        MaxCapacity = (int)numCapacity.Value
                    });
                }
                else
                {
                    await _service.UpdateTalkshowAsync(_existing.TalkshowId, new UpdateTalkshowRequest
                    {
                        Title       = txtTsTitle.Text,
                        SpeakerName = txtSpeakerName.Text,
                        SpeakerBio  = txtSpeakerBio.Text,
                        Venue       = txtVenue.Text,
                        StartTime   = dtpStartTime.Value,
                        EndTime     = dtpEndTime.Value,
                        MaxCapacity = (int)numCapacity.Value
                    });
                }

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
