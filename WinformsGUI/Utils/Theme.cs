using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinformsGUI.Controls;

namespace WinformsGUI.Utils
{
    /// <summary>
    /// Design System — "Soft Indigo & Sage" v2
    /// Palet gelap modern, tipografi konsisten berbasis kelipatan genap,
    /// aksen indigo pastel dan sage mint, radius sudut 10px.
    /// </summary>
    public static class Theme
    {
        // ── Color Tokens ─────────────────────────────────────────────────────
        public static readonly Color BgDeep    = Color.FromArgb(30, 30, 46);   // #1E1E2E
        public static readonly Color BgSurface = Color.FromArgb(42, 42, 62);   // #2A2A3E
        public static readonly Color BgCard    = Color.FromArgb(49, 49, 73);   // #313149
        public static readonly Color BgInput   = Color.FromArgb(38, 38, 58);   // #26263A

        public static readonly Color AccentPrimary      = Color.FromArgb(124, 111, 247); // #7C6FF7
        public static readonly Color AccentPrimaryHover = Color.FromArgb(101, 88,  220); // #6558DC
        public static readonly Color AccentPrimaryPress = Color.FromArgb( 80, 68,  190);
        public static readonly Color AccentSecond       = Color.FromArgb( 93, 201, 168); // #5DC9A8
        public static readonly Color AccentSecondHover  = Color.FromArgb( 72, 175, 145);
        public static readonly Color AccentSuccess      = Color.FromArgb( 93, 201, 168); // Same as AccentSecond
        public static readonly Color AccentDanger       = Color.FromArgb(224, 123, 123); // #E07B7B
        public static readonly Color AccentDangerHover  = Color.FromArgb(200,  95,  95);

        public static readonly Color TextPrimary   = Color.FromArgb(232, 230, 240); // #E8E6F0
        public static readonly Color TextSecondary = Color.FromArgb(160, 156, 184); // #A09CB8
        public static readonly Color TextMuted     = Color.FromArgb(107, 104, 130); // #6B6882

        public static readonly Color BorderSoft   = Color.FromArgb(61, 59, 92);    // #3D3B5C
        public static readonly Color BorderActive = Color.FromArgb(124, 111, 247);

        // ── Typography (semua ukuran kelipatan genap) ─────────────────────────
        public static readonly Font FontTitle     = new Font("Segoe UI", 22F, FontStyle.Bold,    GraphicsUnit.Point);
        public static readonly Font FontPageTitle = new Font("Segoe UI", 18F, FontStyle.Bold,    GraphicsUnit.Point);
        public static readonly Font FontHeader    = new Font("Segoe UI", 16F, FontStyle.Bold,    GraphicsUnit.Point);
        public static readonly Font FontSubhead   = new Font("Segoe UI", 14F, FontStyle.Bold,    GraphicsUnit.Point);
        public static readonly Font FontBody      = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font FontSmall     = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font FontLabel     = new Font("Segoe UI", 10F, FontStyle.Bold,    GraphicsUnit.Point);

        // ── Spacing (px, kelipatan genap) ────────────────────────────────────
        public const int SpaceXS  =  6;
        public const int SpaceSM  = 12;
        public const int SpaceMD  = 20;
        public const int SpaceLG  = 28;
        public const int SpaceXL  = 40;
        public const int SpaceXXL = 56;

        // ── Radius ───────────────────────────────────────────────────────────
        public const int RadiusButton =  10;
        public const int RadiusCard   =  12;
        public const int RadiusCell   =   8; // aksi button dalam cell

        // ── Rounded Path Helper ───────────────────────────────────────────────
        public static GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            return RoundedButton.BuildPath(rect, radius);
        }

        public static GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            return GetRoundedPath(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), radius);
        }

        // ── DataGridView Action Button Painter ────────────────────────────────
        /// <summary>
        /// Menggambar tombol aksi rounded dalam sebuah cell DataGridView (digunakan di CellPainting).
        /// Panggil dengan e.Graphics setelah e.Paint() sudah dipanggil untuk background.
        /// </summary>
        public static void DrawGridActionButton(Graphics g, RectangleF rect, string text, Color bg, Color fg)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path  = GetRoundedPath(rect, RadiusCell))
            using (var brush = new SolidBrush(bg))
                g.FillPath(brush, path);

            TextRenderer.DrawText(
                g, text, FontSmall,
                new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height),
                fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        // ── Apply Methods ─────────────────────────────────────────────────────

        public static void ApplyToForm(Form form)
        {
            form.BackColor     = BgDeep;
            form.ForeColor     = TextPrimary;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Font          = FontBody;
            ApplyToControls(form.Controls);
        }

        public static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c is Button btn)        ApplyToButton(btn);
                else if (c is Label lbl)    ApplyToLabel(lbl);
                else if (c is TextBox txt)  ApplyToTextBox(txt);
                else if (c is NumericUpDown n)    ApplyToNumericUpDown(n);
                else if (c is DateTimePicker dtp) ApplyToDateTimePicker(dtp);
                else if (c is LinkLabel lnk)      ApplyToLinkLabel(lnk);
                else if (c is DataGridView dgv)   ApplyToDataGridView(dgv);
                else if (c is Panel)
                {
                    if (c.Controls.Count > 0) ApplyToControls(c.Controls);
                }
                else
                {
                    c.ForeColor = TextPrimary;
                    if (c.Controls.Count > 0) ApplyToControls(c.Controls);
                }
            }
        }

        /// <summary>Primary button — indigo filled, rounded.</summary>
        public static void ApplyToButton(Button button)
        {
            button.BackColor = AccentPrimary;
            button.ForeColor = TextPrimary;
            button.Font      = FontLabel;
            button.Cursor    = Cursors.Hand;

            if (button is RoundedButton rb)
            {
                rb.HoverBackColor   = AccentPrimaryHover;
                rb.PressedBackColor = AccentPrimaryPress;
            }
            else
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize         = 0;
                button.FlatAppearance.MouseOverBackColor = AccentPrimaryHover;
                button.FlatAppearance.MouseDownBackColor = AccentPrimaryPress;
            }
        }

        /// <summary>Secondary button — card-colored, subtle.</summary>
        public static void ApplyToSecondaryButton(Button button)
        {
            button.BackColor = BgCard;
            button.ForeColor = TextSecondary;
            button.Font      = FontLabel;
            button.Cursor    = Cursors.Hand;

            if (button is RoundedButton rb)
            {
                rb.HoverBackColor   = Color.FromArgb(62, 62, 90);
                rb.PressedBackColor = Color.FromArgb(45, 45, 68);
            }
            else
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize         = 1;
                button.FlatAppearance.BorderColor        = BorderSoft;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 90);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(45, 45, 68);
            }
        }

        /// <summary>Danger button — pastel red for destructive actions.</summary>
        public static void ApplyToDangerButton(Button button)
        {
            button.BackColor = AccentDanger;
            button.ForeColor = TextPrimary;
            button.Font      = FontLabel;
            button.Cursor    = Cursors.Hand;

            if (button is RoundedButton rb)
            {
                rb.HoverBackColor   = AccentDangerHover;
                rb.PressedBackColor = Color.FromArgb(175, 70, 70);
            }
            else
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize         = 0;
                button.FlatAppearance.MouseOverBackColor = AccentDangerHover;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(175, 70, 70);
            }
        }

        /// <summary>Accent button — sage/mint for scan/positive special actions.</summary>
        public static void ApplyToAccentButton(Button button)
        {
            button.BackColor = AccentSecond;
            button.ForeColor = Color.FromArgb(20, 38, 32);
            button.Font      = FontLabel;
            button.Cursor    = Cursors.Hand;

            if (button is RoundedButton rb)
            {
                rb.HoverBackColor   = AccentSecondHover;
                rb.PressedBackColor = Color.FromArgb(55, 150, 122);
            }
            else
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize         = 0;
                button.FlatAppearance.MouseOverBackColor = AccentSecondHover;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(55, 150, 122);
            }
        }

        public static void ApplyToTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor   = BgInput;
            txt.ForeColor   = TextPrimary;
            txt.Font        = FontBody;
        }

        public static void ApplyToNumericUpDown(NumericUpDown num)
        {
            num.BackColor   = BgInput;
            num.ForeColor   = TextPrimary;
            num.Font        = FontBody;
            num.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void ApplyToDateTimePicker(DateTimePicker dtp)
        {
            dtp.CalendarMonthBackground = BgSurface;
            dtp.CalendarForeColor       = TextPrimary;
            dtp.CalendarTitleBackColor  = BgCard;
            dtp.CalendarTitleForeColor  = TextPrimary;
            dtp.Font = FontBody;
        }

        public static void ApplyToLabel(Label lbl)
        {
            lbl.BackColor = Color.Transparent;
            lbl.ForeColor = TextPrimary;
            lbl.Font      = FontBody;
        }

        public static void ApplyToLinkLabel(LinkLabel lnk)
        {
            lnk.BackColor    = Color.Transparent;
            lnk.ForeColor    = AccentSecond;
            lnk.LinkColor    = AccentSecond;
            lnk.ActiveLinkColor = TextPrimary;
            lnk.VisitedLinkColor= AccentSecond;
            lnk.Font         = FontBody;
            lnk.LinkBehavior = LinkBehavior.HoverUnderline;
        }

        public static void ApplyToDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = BgSurface;
            dgv.BorderStyle     = BorderStyle.None;
            dgv.GridColor       = BorderSoft;
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Headers
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor         = BgCard;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor         = TextMuted;
            dgv.ColumnHeadersDefaultCellStyle.Font              = FontLabel;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor= BgCard;
            dgv.ColumnHeadersDefaultCellStyle.Alignment         = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding           = new Padding(SpaceSM, 0, 0, 0);
            dgv.ColumnHeadersHeight = 44;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Rows
            dgv.RowHeadersVisible = false;
            dgv.DefaultCellStyle.BackColor         = BgSurface;
            dgv.DefaultCellStyle.ForeColor         = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor= BgCard;
            dgv.DefaultCellStyle.SelectionForeColor= AccentPrimary;
            dgv.DefaultCellStyle.Font              = FontBody;
            dgv.DefaultCellStyle.Padding           = new Padding(SpaceSM, 0, SpaceSM, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor         = Color.FromArgb(45, 45, 67);
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor= BgCard;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor= AccentPrimary;
            dgv.RowTemplate.Height = 46;

            dgv.AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode        = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows   = false;
            dgv.AllowUserToDeleteRows= false;
            dgv.ReadOnly             = true;
        }

        /// <summary>Buat panel divider horizontal.</summary>
        public static Panel MakeDivider(int width)
        {
            return new Panel { Size = new Size(width, 1), BackColor = BorderSoft };
        }
    }
}
