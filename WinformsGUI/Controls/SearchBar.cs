using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinformsGUI.Utils;

namespace WinformsGUI.Controls
{
    public class SearchBar : UserControl
    {
        private TextBox _textBox;
        private Label   _lblIcon;
        private int     _radius = Theme.RadiusButton;

        public event EventHandler? TextChangedEvent;

        public SearchBar()
        {
            this.DoubleBuffered = true;
            this.BackColor      = Color.Transparent;
            this.ForeColor      = Theme.TextPrimary;
            this.Size           = new Size(250, 42);
            this.Padding        = new Padding(12, 0, 12, 0);

            _lblIcon = new Label();
            _lblIcon.Text      = "🔍";
            _lblIcon.Font      = Theme.FontSmall;
            _lblIcon.ForeColor = Theme.TextMuted;
            _lblIcon.BackColor = Color.Transparent;
            _lblIcon.AutoSize  = true;

            _textBox = new TextBox();
            _textBox.BorderStyle = BorderStyle.None;
            _textBox.BackColor   = Theme.BgInput;
            _textBox.ForeColor   = Theme.TextPrimary;
            _textBox.Font        = Theme.FontBody;
            _textBox.TextChanged += (s, e) => TextChangedEvent?.Invoke(this, EventArgs.Empty);

            this.Controls.Add(_textBox);
            this.Controls.Add(_lblIcon);

            this.Resize += SearchBar_Resize;
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string PlaceholderText
        {
            get => _textBox.PlaceholderText;
            set => _textBox.PlaceholderText = value;
        }

        public override string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PositionControls();
        }

        private void SearchBar_Resize(object? sender, EventArgs e)
        {
            PositionControls();
            this.Invalidate();
        }

        private void PositionControls()
        {
            _lblIcon.Location = new Point(12, (this.Height - _lblIcon.Height) / 2);
            _textBox.Location = new Point(_lblIcon.Right + 8, (this.Height - _textBox.Height) / 2);
            _textBox.Width    = this.Width - _textBox.Left - 12;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0, 0, this.Width, this.Height);
            using (var path = Theme.GetRoundedPath(rect, _radius))
            {
                using (var brush = new SolidBrush(Theme.BgInput))
                {
                    e.Graphics.FillPath(brush, path);
                }
                using (var pen = new Pen(Theme.BorderSoft, 1.5f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }
    }
}
