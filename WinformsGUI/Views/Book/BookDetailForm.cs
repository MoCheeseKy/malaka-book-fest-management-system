using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Book
{
    public class BookDetailForm : Form
    {
        private BookResponse _book;

        private Label         lblBookTitle;
        private Label         lblBookAuthor;
        private Panel         pnlDivider;
        private Label         lblSectionMeta;
        private Panel         pnlInfoCard;
        private Label         lblPublisherKey;
        private Label         lblPublisherVal;
        private Label         lblPriceKey;
        private Label         lblPriceVal;
        private Label         lblStockKey;
        private Label         lblStockVal;
        private Label         lblCoverUrlKey;
        private Label         lblCoverUrlVal;
        private Label         lblBoothIdKey;
        private Label         lblBoothIdVal;
        private RoundedButton btnClose;

        public BookDetailForm(BookResponse book)
        {
            _book = book;
            InitializeComponent();
            PopulateData();
        }

        private void InitializeComponent()
        {
            this.lblBookTitle   = new Label();
            this.lblBookAuthor  = new Label();
            this.pnlDivider     = new Panel();
            this.lblSectionMeta = new Label();
            this.pnlInfoCard    = new Panel();
            this.lblPublisherKey= new Label();
            this.lblPublisherVal= new Label();
            this.lblPriceKey    = new Label();
            this.lblPriceVal    = new Label();
            this.lblStockKey    = new Label();
            this.lblStockVal    = new Label();
            this.lblCoverUrlKey = new Label();
            this.lblCoverUrlVal = new Label();
            this.lblBoothIdKey  = new Label();
            this.lblBoothIdVal  = new Label();
            this.btnClose       = new RoundedButton();

            this.Text            = "Detail Buku";
            this.Size            = new Size(520, 580);
            this.BackColor       = Theme.BgSurface;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;

            int m = Theme.SpaceLG;
            int w = 440;
            int y = m;

            // Title
            this.lblBookTitle.Text      = "(Title)";
            this.lblBookTitle.Font      = Theme.FontHeader;
            this.lblBookTitle.ForeColor = Theme.TextPrimary;
            this.lblBookTitle.BackColor = Color.Transparent;
            this.lblBookTitle.AutoSize  = false;
            this.lblBookTitle.Size      = new Size(w, 38);
            this.lblBookTitle.Location  = new Point(m, y);
            y += 38 + 4;

            // Author badge
            this.lblBookAuthor.Text      = "✍  (Author)";
            this.lblBookAuthor.Font      = Theme.FontSmall;
            this.lblBookAuthor.ForeColor = Theme.AccentPrimary;
            this.lblBookAuthor.BackColor = Color.Transparent;
            this.lblBookAuthor.AutoSize  = true;
            this.lblBookAuthor.Location  = new Point(m, y);
            y += 24 + Theme.SpaceSM;

            // Divider
            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(w, 1);
            this.pnlDivider.Location  = new Point(m, y);
            y += 1 + Theme.SpaceMD;

            // Section label
            this.lblSectionMeta.Text      = "INFORMASI BUKU";
            this.lblSectionMeta.Font      = Theme.FontLabel;
            this.lblSectionMeta.ForeColor = Theme.TextMuted;
            this.lblSectionMeta.BackColor = Color.Transparent;
            this.lblSectionMeta.AutoSize  = true;
            this.lblSectionMeta.Location  = new Point(m, y);
            y += 22 + Theme.SpaceSM;

            // Info Card
            this.pnlInfoCard.BackColor = Theme.BgCard;
            this.pnlInfoCard.Location  = new Point(m, y);
            this.pnlInfoCard.Size      = new Size(w, 290);
            AddMetaRow(pnlInfoCard, lblPublisherKey, lblPublisherVal, "ISBN",       0);
            AddMetaRow(pnlInfoCard, lblPriceKey,     lblPriceVal,     "Harga",      58);
            AddMetaRow(pnlInfoCard, lblStockKey,     lblStockVal,     "Stok",       116);
            AddMetaRow(pnlInfoCard, lblCoverUrlKey,  lblCoverUrlVal,  "Cover URL",  174);
            AddMetaRow(pnlInfoCard, lblBoothIdKey,   lblBoothIdVal,   "Booth ID",   232);
            y += 290 + Theme.SpaceLG;

            // Close Button
            this.btnClose.Text         = "Tutup";
            this.btnClose.Size         = new Size(w, 48);
            this.btnClose.Location     = new Point(m, y);
            this.btnClose.Font         = Theme.FontSubhead;
            this.btnClose.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnClose);
            this.btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblBookTitle, lblBookAuthor, pnlDivider,
                lblSectionMeta, pnlInfoCard, btnClose
            });
        }

        private void AddMetaRow(Panel parent, Label keyLbl, Label valLbl, string key, int y)
        {
            keyLbl.Text      = key;
            keyLbl.Font      = Theme.FontLabel;
            keyLbl.ForeColor = Theme.TextMuted;
            keyLbl.BackColor = Color.Transparent;
            keyLbl.AutoSize  = false;
            keyLbl.Size      = new Size(110, 26);
            keyLbl.Location  = new Point(Theme.SpaceMD, y + Theme.SpaceMD);
            keyLbl.TextAlign = ContentAlignment.MiddleLeft;

            valLbl.Text      = "—";
            valLbl.Font      = Theme.FontBody;
            valLbl.ForeColor = Theme.TextPrimary;
            valLbl.BackColor = Color.Transparent;
            valLbl.AutoSize  = false;
            valLbl.Size      = new Size(280, 26);
            valLbl.Location  = new Point(Theme.SpaceMD + 120, y + Theme.SpaceMD);
            valLbl.TextAlign = ContentAlignment.MiddleLeft;

            if (y > 0)
            {
                var div = new Panel { Location = new Point(Theme.SpaceMD, y + Theme.SpaceSM), Size = new Size(396, 1), BackColor = Theme.BorderSoft };
                parent.Controls.Add(div);
            }
            parent.Controls.Add(keyLbl);
            parent.Controls.Add(valLbl);
        }

        private void PopulateData()
        {
            lblBookTitle.Text   = _book.Title;
            lblBookAuthor.Text  = $"✍  {_book.Author}";
            lblPublisherVal.Text= string.IsNullOrEmpty(_book.Isbn) ? "-" : _book.Isbn;
            lblPriceVal.Text    = $"Rp {_book.Price:N0}";
            lblStockVal.Text    = _book.Stock.ToString();
            lblCoverUrlVal.Text = string.IsNullOrEmpty(_book.CoverUrl) ? "-" : _book.CoverUrl;
            lblBoothIdVal.Text  = _book.BoothId.ToString();
        }
    }
}
