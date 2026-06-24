using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinformsGUI.Controls;
using WinformsGUI.Models;
using WinformsGUI.Services;
using WinformsGUI.Utils;

namespace WinformsGUI.Views.Booth
{
    public class BoothDetailForm : Form
    {
        private BoothResponse _booth;
        private BookService   _bookService;

        // Header row
        private Label         lblBoothName;
        private Label         lblBoothMeta;
        private Panel         pnlDivider;

        // Books section
        private Label         lblBooksSection;
        private RoundedButton btnRefresh;
        private RoundedButton btnAddBook;
        private Panel         pnlDivider2;
        private DataGridView  dgvBooks;

        private bool _columnsInitialized;

        private const int M       = Theme.SpaceLG;
        private const int TitleH  = 36;
        private const int MetaH   = 22;
        private const int BtnH    = 44;
        private const int SecLblH = 22;

        private const int AksiPadX = 5;
        private const int AksiPadY = 6;
        private const int AksiBtnW = 82;
        private const int AksiGap  = 4;
        private const int AksiColW = AksiPadX + AksiBtnW + AksiGap + AksiBtnW + AksiGap + AksiBtnW + AksiPadX;

        public BoothDetailForm(BoothResponse booth)
        {
            _booth       = booth;
            _bookService = new BookService();
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.lblBoothName    = new Label();
            this.lblBoothMeta    = new Label();
            this.pnlDivider      = new Panel();
            this.lblBooksSection = new Label();
            this.btnRefresh      = new RoundedButton();
            this.btnAddBook      = new RoundedButton();
            this.pnlDivider2     = new Panel();
            this.dgvBooks        = new DataGridView();

            this.Text            = $"Detail Booth — {_booth.Name}";
            this.Size            = new Size(1000, 700);
            this.BackColor       = Theme.BgDeep;
            this.ForeColor       = Theme.TextPrimary;
            this.Font            = Theme.FontBody;
            this.StartPosition   = FormStartPosition.CenterScreen;

            // ── Booth Title ─────────────────────────────────────────────────
            this.lblBoothName.Text      = _booth.Name;
            this.lblBoothName.Font      = Theme.FontPageTitle;
            this.lblBoothName.ForeColor = Theme.TextPrimary;
            this.lblBoothName.BackColor = Color.Transparent;
            this.lblBoothName.AutoSize  = true;

            // ── Booth Meta ──────────────────────────────────────────────────
            this.lblBoothMeta.Text      = $"📍 {_booth.Location}   ·   {_booth.Description}";
            this.lblBoothMeta.Font      = Theme.FontSmall;
            this.lblBoothMeta.ForeColor = Theme.TextMuted;
            this.lblBoothMeta.BackColor = Color.Transparent;
            this.lblBoothMeta.AutoSize  = false;
            this.lblBoothMeta.Height    = MetaH;

            // ── Divider 1 ───────────────────────────────────────────────────
            this.pnlDivider.BackColor = Theme.BorderSoft;
            this.pnlDivider.Size      = new Size(10, 1);

            // ── Books Section Header ────────────────────────────────────────
            this.lblBooksSection.Text      = "DAFTAR BUKU";
            this.lblBooksSection.Font      = Theme.FontSmall;
            this.lblBooksSection.ForeColor = Theme.TextMuted;
            this.lblBooksSection.BackColor = Color.Transparent;
            this.lblBooksSection.AutoSize  = true;

            // ── Buttons ─────────────────────────────────────────────────────
            this.btnRefresh.Text         = "↻  Refresh";
            this.btnRefresh.Size         = new Size(130, 42);
            this.btnRefresh.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToSecondaryButton(this.btnRefresh);
            this.btnRefresh.Click       += (s, e) => LoadBooks();

            this.btnAddBook.Text         = "+  Tambah Buku";
            this.btnAddBook.Size         = new Size(160, BtnH);
            this.btnAddBook.Font         = Theme.FontSubhead;
            this.btnAddBook.CornerRadius = Theme.RadiusButton;
            Theme.ApplyToButton(this.btnAddBook);
            this.btnAddBook.Click       += BtnAddBook_Click;

            // ── Divider 2 ───────────────────────────────────────────────────
            this.pnlDivider2.BackColor = Theme.BorderSoft;
            this.pnlDivider2.Size      = new Size(10, 1);

            // ── DataGridView ────────────────────────────────────────────────
            Theme.ApplyToDataGridView(this.dgvBooks);
            this.dgvBooks.CellPainting   += DgvBooks_CellPainting;
            this.dgvBooks.CellMouseClick += DgvBooks_CellMouseClick;
            this.dgvBooks.CellMouseEnter += (s, e) => UpdateCursor(e.ColumnIndex);
            this.dgvBooks.CellMouseLeave += (s, e) => { this.dgvBooks.Cursor = Cursors.Default; };

            this.Controls.AddRange(new Control[] {
                lblBoothName, lblBoothMeta, pnlDivider,
                lblBooksSection, btnRefresh, btnAddBook, pnlDivider2,
                dgvBooks
            });

            this.Resize += (s, e) => RepositionControls();
            RepositionControls();
        }

        private void UpdateCursor(int colIndex)
        {
            if (colIndex >= 0 && dgvBooks.Columns.Count > colIndex &&
                dgvBooks.Columns[colIndex].Name == "colAksi")
                dgvBooks.Cursor = Cursors.Hand;
            else
                dgvBooks.Cursor = Cursors.Default;
        }

        private void RepositionControls()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            // Section 1: Booth info
            this.lblBoothName.Location = new Point(M, M);
            this.lblBoothMeta.Location = new Point(M, M + TitleH + 6);
            this.lblBoothMeta.Width    = w - M * 2;

            int div1Y = M + TitleH + 6 + MetaH + Theme.SpaceSM;
            this.pnlDivider.Location = new Point(M, div1Y);
            this.pnlDivider.Size     = new Size(w - M * 2, 1);

            // Section 2: Books header row
            int sec2Y   = div1Y + 1 + Theme.SpaceMD;
            int secBtnH = Math.Max(SecLblH, BtnH);

            this.lblBooksSection.Location = new Point(M, sec2Y + (secBtnH - SecLblH) / 2);
            this.btnAddBook.Location      = new Point(w - M - btnAddBook.Width, sec2Y + (secBtnH - BtnH) / 2);
            this.btnRefresh.Location      = new Point(btnAddBook.Left - Theme.SpaceSM - btnRefresh.Width,
                                                      sec2Y + (secBtnH - btnRefresh.Height) / 2);

            int div2Y = sec2Y + secBtnH + Theme.SpaceSM;
            this.pnlDivider2.Location = new Point(M, div2Y);
            this.pnlDivider2.Size     = new Size(w - M * 2, 1);

            // DataGridView
            int dgvY = div2Y + 1 + Theme.SpaceSM;
            this.dgvBooks.Location = new Point(M, dgvY);
            this.dgvBooks.Size     = new Size(w - M * 2, Math.Max(0, h - dgvY - M));
        }

        // ── Cell Painting ───────────────────────────────────────────────────
        private void DgvBooks_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvBooks.Columns[e.ColumnIndex].Name != "colAksi") return;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.Background |
                                  DataGridViewPaintParts.SelectionBackground |
                                  DataGridViewPaintParts.Border);

            int btnH = e.CellBounds.Height - AksiPadY * 2;
            int x0   = e.CellBounds.X + AksiPadX;
            int y0   = e.CellBounds.Y + AksiPadY;

            Theme.DrawGridActionButton(e.Graphics, new RectangleF(x0, y0, AksiBtnW, btnH),
                "✎  Edit", Theme.AccentPrimary, Theme.TextPrimary);
            Theme.DrawGridActionButton(e.Graphics, new RectangleF(x0 + AksiBtnW + AksiGap, y0, AksiBtnW, btnH),
                "✕  Hapus", Theme.AccentDanger, Theme.TextPrimary);
            Theme.DrawGridActionButton(e.Graphics, new RectangleF(x0 + (AksiBtnW + AksiGap) * 2, y0, AksiBtnW, btnH),
                "◎  Detail", Theme.BgCard, Theme.TextSecondary);

            e.Handled = true;
        }

        private void DgvBooks_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dgvBooks.Columns[e.ColumnIndex].Name != "colAksi") return;

            int x = e.X;
            if      (x >= AksiPadX && x < AksiPadX + AksiBtnW)                           BtnEdit_Action(e.RowIndex);
            else if (x >= AksiPadX + AksiBtnW + AksiGap && x < AksiPadX + (AksiBtnW + AksiGap) * 2) BtnDelete_Action(e.RowIndex);
            else if (x >= AksiPadX + (AksiBtnW + AksiGap) * 2)                            BtnDetail_Action(e.RowIndex);
        }

        // ── Data Loading ────────────────────────────────────────────────────
        private async void LoadBooks()
        {
            try
            {
                var books = await _bookService.GetBooksByBoothAsync(_booth.Id);
                dgvBooks.DataSource = books;

                if (!_columnsInitialized)
                {
                    var colAksi = new DataGridViewTextBoxColumn
                    {
                        Name         = "colAksi",
                        HeaderText   = "Aksi",
                        ReadOnly     = true,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        Width        = AksiColW,
                        DefaultCellStyle = { SelectionBackColor = Theme.BgCard, SelectionForeColor = Theme.TextPrimary }
                    };
                    dgvBooks.Columns.Add(colAksi);
                    _columnsInitialized = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Actions ─────────────────────────────────────────────────────────
        private void BtnEdit_Action(int row)
        {
            if (dgvBooks.Rows[row].DataBoundItem is BookResponse book)
            {
                var frm = new Views.Book.BookEditForm(_booth.Id, book);
                if (frm.ShowDialog() == DialogResult.OK) LoadBooks();
            }
        }

        private async void BtnDelete_Action(int row)
        {
            if (dgvBooks.Rows[row].DataBoundItem is BookResponse book)
            {
                var confirm = MessageBox.Show($"Hapus buku \"{book.Title}\"?",
                    "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        await _bookService.DeleteBookAsync(_booth.Id, book.Id);
                        LoadBooks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnDetail_Action(int row)
        {
            if (dgvBooks.Rows[row].DataBoundItem is BookResponse book)
            {
                var frm = new Views.Book.BookDetailForm(book);
                frm.ShowDialog();
            }
        }

        private void BtnAddBook_Click(object? sender, EventArgs e)
        {
            var frm = new Views.Book.BookEditForm(_booth.Id, null);
            if (frm.ShowDialog() == DialogResult.OK) LoadBooks();
        }
    }
}
