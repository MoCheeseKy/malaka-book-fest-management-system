using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinformsGUI.Controls
{
    public class RoundedButton : Button
    {
        private bool _isHovered;
        private bool _isPressed;

        /// <summary>Radius sudut dalam pixel. Default: 10.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = 10;

        /// <summary>Warna background saat mouse hover.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverBackColor { get; set; } = Color.Empty;

        /// <summary>Warna background saat mouse ditekan.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color PressedBackColor { get; set; } = Color.Empty;

        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.DodgerBlue;
            this.ForeColor = Color.White;
            this.Cursor    = Cursors.Hand;

            // Supaya tidak kelihatan focus rect standar
            this.SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            _isPressed = true;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _isPressed = false;
            this.Invalidate();
        }

        // Helper path static supaya bisa di-reuse kalau perlu (misal di DataGridView cell)
        public static GraphicsPath BuildPath(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Jangan panggil base.OnPaint(pevent) karena kita override semuanya
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0, 0, this.Width, this.Height);

            // Determine current colors
            Color bg = this.BackColor;
            if (_isPressed && PressedBackColor != Color.Empty) bg = PressedBackColor;
            else if (_isHovered && HoverBackColor != Color.Empty) bg = HoverBackColor;

            using (var path = BuildPath(rect, CornerRadius))
            {
                // Isi background
                using (var brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                // Jika tombol di-disable
                if (!this.Enabled)
                {
                    using (var disabledBrush = new SolidBrush(Color.FromArgb(100, Color.Gray)))
                    {
                        g.FillPath(disabledBrush, path);
                    }
                }
            }

            // Gambar text di tengah-tengah dengan properti Font & ForeColor
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Warna teks: kalau disable ubah jadi abu-abu
            Color textColor = this.Enabled ? this.ForeColor : Color.Gray;

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(this.Text, this.Font, brush, rect, format);
            }
        }
    }
}
