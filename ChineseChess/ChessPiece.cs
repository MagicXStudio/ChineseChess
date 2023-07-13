using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChineseChess
{
    public partial class ChessPiece : PictureBox
    {
        public ChessPiece()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public override Color BackColor { get => base.BackColor; set => base.BackColor = value; }

        public override string Text { get => base.Text; set => base.Text = value; }

        protected override void OnNotifyMessage(Message m)
        {
            base.OnNotifyMessage(m);
        }

        public override void Refresh()
        {
            base.Refresh();
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
    }
}
