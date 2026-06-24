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
        private Panel         pnlStatusChip;
        private Label         lblStatusText;
        private Panel         pnlDivider;

        // Books section
        private Label         lblBooksSection;
        private RoundedButton btnRefresh;
        private RoundedButton btnAddBook;
        private Panel         pnlDivider2;
        private DataGridView  dgvBooks;
        private WinformsGUI.Controls.SearchBar txtSearch;

        private System.Collections.Generic.List<BookResponse> _allBooks = new();

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
            this.pnlStatusChip   = new Panel();
            this.lblStatusText   = new Label();
            this.pnlDivider      = new Panel();
            this.lblBooksSection = new Label();
            this.btnRefresh      = new RoundedButton();
            this.btnAddBook      = new RoundedButton();
            this.txtSearch       = new WinformsGUI.Controls.SearchBar();
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

            // ── Status Chip ─────────────────────────────────────────────────
            this.pnlStatusChip.BackColor = _booth.IsActive ? Theme.AccentSuccess : Theme.AccentDanger;
            this.pnlStatusChip.Size      = new Size(80, 24);
            
            this.lblStatusText.Text      = _booth.IsActive ? "Active" : "Inactive";
            this.lblStatusText.Font      = Theme.FontSmall;
            this.lblStatusText.ForeColor = Color.White;
            this.lblStatusText.AutoSize  = false;
            this.lblStatusText.TextAlign = ContentAlignment.MiddleCenter;
            this.lblStatusText.Dock      = DockStyle.Fill;
            this.pnlStatusChip.Controls.Add(lblStatusText);

            // ── Booth Meta ──────────────────────────────────────────────────
            this.lblBoothMeta.Text      = $"📍 {_booth.Location}   ·   🏷️ {_booth.CategoryName}   ·   {_booth.Description}";
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
            this.lblBooksSection.Font      = Theme.FontSubhead;
            this.lblBooksSection.ForeColor = Color.White;
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

            // ── Search Bar ──────────────────────────────────────────────────
            this.txtSearch.Size          = new Size(250, 42);
            this.txtSearch.PlaceholderText = "Cari buku...";
            this.txtSearch.TextChangedEvent += TxtSearch_TextChanged;

            // ── Divider 2 ───────────────────────────────────────────────────
            this.pnlDivider2.BackColor = Theme.BorderSoft;
            this.pnlDivider2.Size      = new Size(10, 1);

            // ── DataGridView ────────────────────────────────────────────────
            Theme.ApplyToDataGridView(this.dgvBooks);
            this.dgvBooks.CellPainting   += DgvBooks_CellPainting;
            this.dgvBooks.CellMouseClick += DgvBooks_CellMouseClick;
            this.dgvBooks.Paint          += (s, e) => Theme.DrawEmptyState(dgvBooks, e, "Tidak ada data buku di booth ini.");
            this.dgvBooks.CellMouseEnter += (s, e) => UpdateCursor(e.ColumnIndex);
            this.dgvBooks.CellMouseLeave += (s, e) => { this.dgvBooks.Cursor = Cursors.Default; };

            this.Controls.AddRange(new Control[] {
                lblBoothName, pnlStatusChip, lblBoothMeta, pnlDivider,
                lblBooksSection, txtSearch, btnRefresh, btnAddBook, pnlDivider2,
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
            this.pnlStatusChip.Location = new Point(this.lblBoothName.Right + Theme.SpaceMD, M + (TitleH - this.pnlStatusChip.Height) / 2);
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
            this.txtSearch.Location       = new Point(btnRefresh.Left - Theme.SpaceLG - txtSearch.Width,
                                                      sec2Y + (secBtnH - txtSearch.Height) / 2);

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
                _allBooks = await _bookService.GetBooksByBoothAsync(_booth.Id);
                FilterBooks();

                if (!_columnsInitialized)
                {
                    if (dgvBooks.Columns["Id"]          != null) dgvBooks.Columns["Id"].Visible          = false;
                    if (dgvBooks.Columns["Publisher"]   != null) dgvBooks.Columns["Publisher"].Visible   = false; // just in case
                    if (dgvBooks.Columns["Isbn"]        != null) dgvBooks.Columns["Isbn"].Visible        = false;
                    if (dgvBooks.Columns["CoverUrl"]    != null) dgvBooks.Columns["CoverUrl"].Visible    = false;
                    if (dgvBooks.Columns["BoothId"]     != null) dgvBooks.Columns["BoothId"].Visible     = false;

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

        private void FilterBooks()
        {
            if (_allBooks == null) return;
            var q = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q))
                dgvBooks.DataSource = _allBooks;
            else
            {
                var filtered = new System.Collections.Generic.List<BookResponse>();
                foreach (var b in _allBooks)
                {
                    if ((b.Title != null && b.Title.ToLower().Contains(q)) || 
                        (b.Author != null && b.Author.ToLower().Contains(q)))
                    {
                        filtered.Add(b);
                    }
                }
                dgvBooks.DataSource = filtered;
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e) => FilterBooks();

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
