#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Talkshow
{
    public class TalkshowDetailForm : Form
    {
        private TalkshowResponse _talkshow;

        private Label         lblTitle;
        private Label         lblSpeaker;
        private Panel         pnlDivider;
        private Label         lblSection;
        private Panel         pnlInfoCard;
        private Label         lblVenueKey,   lblVenueVal;
        private Label         lblTimeKey,    lblTimeVal;
        private Label         lblStatusKey,  lblStatusVal;
        private Label         lblCapKey,     lblCapVal;
        private Label         lblBioSection;
        private Label         lblBio;
        private RoundedButton btnClose;

        public TalkshowDetailForm(TalkshowResponse talkshow)
        {
            _talkshow = talkshow;
            InitializeComponent();
            PopulateData();
        }

        private void InitializeComponent()
        {
            this.lblTitle      = new Label();
            this.lblSpeaker    = new Label();
            this.pnlDivider    = new Panel();
            this.lblSection    = new Label();
            this.pnlInfoCard   = new Panel();
            this.lblVenueKey   = new Label(); this.lblVenueVal   = new Label();
            this.lblTimeKey    = new Label(); this.lblTimeVal    = new Label();
            this.lblStatusKey  = new Label(); this.lblStatusVal  = new Label();
            this.lblCapKey     = new Label(); this.lblCapVal     = new Label();
            this.lblBioSection = new Label();
            this.lblBio        = new Label();
            this.btnClose      = new RoundedButton();

            this.Text            = "Detail Talkshow";
            this.Size            = new Size(560, 590);
            this.BackColor       = Theme.BgSurface;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;

            int margin = Theme.SpaceLG, cardWidth = 480, positionY = margin;

            // Title
            this.lblTitle.Text      = "(Title)";
            this.lblTitle.Font      = Theme.FontHeader;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Size      = new Size(cardWidth, 38);
            this.lblTitle.Location  = new Point(margin, positionY); positionY += 38 + 4;

            this.lblSpeaker.Text      = "🎤 (Speaker)";
            this.lblSpeaker.Font      = Theme.FontSmall;
            this.lblSpeaker.ForeColor = Theme.AccentPrimary;
            this.lblSpeaker.BackColor = Color.Transparent;
            this.lblSpeaker.AutoSize  = true;
            this.lblSpeaker.Location  = new Point(margin, positionY); positionY += 24 + Theme.SpaceSM;

            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(cardWidth, 1);
            this.pnlDivider.Location  = new Point(margin, positionY); positionY += 1 + Theme.SpaceMD;

            // Section
            this.lblSection.Text      = "INFORMASI ACARA";
            this.lblSection.Font      = Theme.FontLabel;
            this.lblSection.ForeColor = Theme.TextMuted;
            this.lblSection.BackColor = Color.Transparent;
            this.lblSection.AutoSize  = true;
            this.lblSection.Location  = new Point(margin, positionY); positionY += 22 + Theme.SpaceSM;

            // Info Card (4 rows × 58px)
            this.pnlInfoCard.BackColor = Theme.BgCard;
            this.pnlInfoCard.Location  = new Point(margin, positionY);
            this.pnlInfoCard.Size      = new Size(cardWidth, 232);
            AddMetaRow(pnlInfoCard, lblVenueKey,  lblVenueVal,  "Venue",      0,   cardWidth);
            AddMetaRow(pnlInfoCard, lblTimeKey,   lblTimeVal,   "Waktu",      58,  cardWidth);
            AddMetaRow(pnlInfoCard, lblStatusKey, lblStatusVal, "Status",     116, cardWidth);
            AddMetaRow(pnlInfoCard, lblCapKey,    lblCapVal,    "Kapasitas",  174, cardWidth);
            positionY += 232 + Theme.SpaceMD;

            // Bio
            this.lblBioSection.Text      = "BIOGRAFI PEMBICARA";
            this.lblBioSection.Font      = Theme.FontLabel;
            this.lblBioSection.ForeColor = Theme.TextMuted;
            this.lblBioSection.BackColor = Color.Transparent;
            this.lblBioSection.AutoSize  = true;
            this.lblBioSection.Location  = new Point(margin, positionY); positionY += 22 + Theme.SpaceSM;

            this.lblBio.Text      = "(bio)";
            this.lblBio.Font      = Theme.FontBody;
            this.lblBio.ForeColor = Theme.TextSecondary;
            this.lblBio.BackColor = Theme.BgCard;
            this.lblBio.AutoSize  = false;
            this.lblBio.Size      = new Size(cardWidth, 68);
            this.lblBio.Location  = new Point(margin, positionY);
            this.lblBio.Padding   = new Padding(Theme.SpaceMD, Theme.SpaceSM, Theme.SpaceMD, Theme.SpaceSM);
            positionY += 68 + Theme.SpaceLG;

            // Close
            this.btnClose.Text         = "Tutup";
            this.btnClose.Size         = new Size(cardWidth, 48);
            this.btnClose.Location     = new Point(margin, positionY);
            this.btnClose.Font         = Theme.FontSubhead;
            this.btnClose.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnClose);
            this.btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSpeaker, pnlDivider,
                lblSection, pnlInfoCard,
                lblBioSection, lblBio,
                btnClose
            });
        }

        private void AddMetaRow(Panel parent, Label k, Label v, string key, int positionY, int cardWidth)
        {
            k.Text      = key; k.Font = Theme.FontLabel; k.ForeColor = Theme.TextMuted;
            k.BackColor = Color.Transparent; k.AutoSize = false; k.Size = new Size(110, 26);
            k.Location  = new Point(Theme.SpaceMD, positionY + Theme.SpaceMD); k.TextAlign = ContentAlignment.MiddleLeft;

            v.Text      = "—"; v.Font = Theme.FontBody; v.ForeColor = Theme.TextPrimary;
            v.BackColor = Color.Transparent; v.AutoSize = false; v.Size = new Size(cardWidth - 110 - Theme.SpaceMD * 2, 26);
            v.Location  = new Point(Theme.SpaceMD + 120, positionY + Theme.SpaceMD); v.TextAlign = ContentAlignment.MiddleLeft;

            if (positionY > 0)
            {
                var div = new Panel { Location = new Point(Theme.SpaceMD, positionY + Theme.SpaceSM), Size = new Size(cardWidth - Theme.SpaceMD * 2, 1), BackColor = Theme.BorderSoft };
                parent.Controls.Add(div);
            }
            parent.Controls.Add(k); parent.Controls.Add(v);
        }

        private void PopulateData()
        {
            lblTitle.Text   = _talkshow.Title;
            lblSpeaker.Text = $"🎤  {_talkshow.SpeakerName}";
            lblVenueVal.Text  = _talkshow.Venue;
            lblTimeVal.Text   = $"{_talkshow.StartTime:dd MMM yyyy  HH:mm} – {_talkshow.EndTime:HH:mm}";
            lblStatusVal.Text = _talkshow.Status switch
            {
                0 => "📅 Scheduled",
                1 => "🔴 Ongoing",
                2 => "✅ Completed",
                _ => "Unknown"
            };
            lblCapVal.Text  = $"{_talkshow.RegisteredCount} / {_talkshow.MaxCapacity} peserta";
            lblBio.Text     = _talkshow.SpeakerBio;
        }
    }
}
