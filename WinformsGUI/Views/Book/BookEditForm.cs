#pragma warning disable CS8618
using System;
using System.Drawing;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Book
{
    public class BookEditForm : Form
    {
        private Guid          _boothId;
        private BookResponse? _existingBook;
        private BookService   _bookService;

        private Label         lblTitle;
        private Label         lblSubtitle;
        private Panel         pnlDivider;
        private Label         lblBookTitle;
        private TextBox       txtTitle;
        private Label         lblAuthor;
        private TextBox       txtAuthor;
        private Label         lblIsbn;
        private TextBox       txtIsbn;
        private Label         lblCoverUrl;
        private TextBox       txtCoverUrl;
        private Label         lblPrice;
        private NumericUpDown numPrice;
        private Label         lblStock;
        private NumericUpDown numStock;
        private RoundedButton btnSave;
        private RoundedButton btnCancel;

        public BookEditForm(Guid boothId, BookResponse? book)
        {
            _boothId      = boothId;
            _existingBook = book;
            _bookService  = new BookService();
            InitializeComponent();

            if (_existingBook != null)
            {
                lblTitle.Text     = "Edit Buku";
                lblSubtitle.Text  = $"Memperbarui: {_existingBook.Title}";
                txtTitle.Text     = _existingBook.Title;
                txtAuthor.Text    = _existingBook.Author;
                txtIsbn.Text      = _existingBook.Isbn;
                txtCoverUrl.Text  = _existingBook.CoverUrl;
                numPrice.Value    = _existingBook.Price;
                numStock.Value    = _existingBook.Stock;
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle    = new Label();
            this.lblSubtitle = new Label();
            this.pnlDivider  = new Panel();
            this.lblBookTitle= new Label();
            this.txtTitle    = new TextBox();
            this.lblAuthor   = new Label();
            this.txtAuthor   = new TextBox();
            this.lblIsbn     = new Label();
            this.txtIsbn     = new TextBox();
            this.lblCoverUrl = new Label();
            this.txtCoverUrl = new TextBox();
            this.lblPrice    = new Label();
            this.numPrice    = new NumericUpDown();
            this.lblStock    = new Label();
            this.numStock    = new NumericUpDown();
            this.btnSave     = new RoundedButton();
            this.btnCancel   = new RoundedButton();

            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).BeginInit();

            this.Text            = _existingBook == null ? "Tambah Buku" : "Edit Buku";
            this.Size            = new Size(460, 590);
            this.BackColor       = Theme.BgSurface;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;

            int margin = Theme.SpaceLG, cardWidth = 380, positionY = margin;

            // Title
            this.lblTitle.Text      = "Tambah Buku Baru";
            this.lblTitle.Font      = Theme.FontHeader;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.AutoSize  = true;
            this.lblTitle.Location  = new Point(margin, positionY); positionY += 38;

            this.lblSubtitle.Text      = "Isi detail informasi buku";
            this.lblSubtitle.Font      = Theme.FontSmall;
            this.lblSubtitle.ForeColor = Theme.TextMuted;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.AutoSize  = true;
            this.lblSubtitle.Location  = new Point(margin, positionY); positionY += 28;

            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(cardWidth, 1);
            this.pnlDivider.Location  = new Point(margin, positionY); positionY += 1 + Theme.SpaceMD;

            // Fields
            MakeFieldLabel(lblBookTitle, "JUDUL BUKU", margin, positionY); positionY += Theme.SpaceSM + 2;
            MakeTextBox(txtTitle, margin, positionY, cardWidth, 38); positionY += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblAuthor, "PENULIS", margin, positionY); positionY += Theme.SpaceSM + 2;
            MakeTextBox(txtAuthor, margin, positionY, cardWidth, 38); positionY += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblIsbn, "ISBN", margin, positionY); positionY += Theme.SpaceSM + 2;
            MakeTextBox(txtIsbn, margin, positionY, cardWidth, 38); positionY += 38 + Theme.SpaceMD;

            MakeFieldLabel(lblCoverUrl, "COVER URL", margin, positionY); positionY += Theme.SpaceSM + 2;
            MakeTextBox(txtCoverUrl, margin, positionY, cardWidth, 38); positionY += 38 + Theme.SpaceMD;

            // Price + Stock row
            int columnWidth = (cardWidth - Theme.SpaceSM) / 2;
            MakeFieldLabel(lblPrice, "HARGA (Rp)", margin,           positionY);
            MakeFieldLabel(lblStock, "STOK",        margin + columnWidth + Theme.SpaceSM, positionY);
            positionY += Theme.SpaceSM + 2;

            this.numPrice.Location         = new Point(margin, positionY);
            this.numPrice.Size             = new Size(columnWidth, 38);
            this.numPrice.Maximum          = 10000000M;
            this.numPrice.Increment        = 1000M;
            this.numPrice.ThousandsSeparator = true;
            Theme.ApplyToNumericUpDown(this.numPrice);

            this.numStock.Location = new Point(margin + columnWidth + Theme.SpaceSM, positionY);
            this.numStock.Size     = new Size(columnWidth, 38);
            this.numStock.Maximum  = 100000M;
            Theme.ApplyToNumericUpDown(this.numStock);
            positionY += 38 + Theme.SpaceLG;

            // Buttons
            this.btnSave.Text         = "Simpan";
            this.btnSave.Size         = new Size(columnWidth, 48);
            this.btnSave.Location     = new Point(margin, positionY);
            this.btnSave.Font         = Theme.FontSubhead;
            this.btnSave.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnSave);
            this.btnSave.Click       += BtnSave_Click;

            this.btnCancel.Text         = "Batal";
            this.btnCancel.Size         = new Size(columnWidth, 48);
            this.btnCancel.Location     = new Point(margin + columnWidth + Theme.SpaceSM, positionY);
            this.btnCancel.Font         = Theme.FontSubhead;
            this.btnCancel.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnCancel);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, pnlDivider,
                lblBookTitle, txtTitle, lblAuthor, txtAuthor, lblIsbn, txtIsbn,
                lblCoverUrl, txtCoverUrl,
                lblPrice, numPrice, lblStock, numStock,
                btnSave, btnCancel
            });

            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).EndInit();
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

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            btnSave.Enabled = false; btnSave.Text = "Menyimpan...";
            try
            {
                string? rawIsbn = string.IsNullOrWhiteSpace(txtIsbn.Text) ? null : txtIsbn.Text;
                string? isbn = rawIsbn?.Replace(" ", "").Replace("-", "");
                string? coverUrl = string.IsNullOrWhiteSpace(txtCoverUrl.Text) ? null : txtCoverUrl.Text;

                if (_existingBook == null)
                    await _bookService.CreateBookAsync(_boothId, new CreateBookRequest { Title = txtTitle.Text, Author = txtAuthor.Text, Isbn = isbn, CoverUrl = coverUrl, Price = numPrice.Value, Stock = (int)numStock.Value });
                else
                    await _bookService.UpdateBookAsync(_boothId, _existingBook.Id, new UpdateBookRequest { Title = txtTitle.Text, Author = txtAuthor.Text, Isbn = isbn, CoverUrl = coverUrl, Price = numPrice.Value, Stock = (int)numStock.Value });
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
