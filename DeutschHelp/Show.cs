using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeutschHelp
{
    public partial class Show : Form
    {
        WordPackung currentWordPackung = null;
        List<WordPackung> wordPackungen;
        List<Label> labels = new List<Label>();
        List<int> lineTops = new List<int>();
        public Show(List<WordPackung> wordPackungen)
        {
            this.wordPackungen = wordPackungen;
            InitializeComponent();
        }
        private void ShowWord()
        {
            foreach (var item in labels)
            {
                Controls.Remove(item);
                item.Dispose();
            }

            labels.Clear();
            label1.Text = currentWordPackung.Text;
            int top = button3.Top + button3.Height + 7;
            lineTops.Clear();

            foreach (var item in currentWordPackung.Words)
            {

                var center = new Label()
                {
                    Top = top,
                    Text = item.Text,
                    Left = label1.Left,
                    AutoSize = true,
                    Font = label2.Font
                };
                labels.Add(center);
                Controls.Add(center);
                top += 7 + center.Height;
                lineTops.Add(top - 3);
                foreach (var item2 in item.Defs)
                {
                    var right = new Label()
                    {
                        Top = top,
                        Left = 77 + button3.Left,
                        Text = item2.Deu,
                        AutoSize = true,
                        Font = label2.Font
                    };
                    var left = new Label()
                    {
                        Top = top,
                        Text = item2.Fa,
                        AutoSize = true,
                        Font = label2.Font
                    };
                    labels.Add(right);
                    Controls.Add(right);
                    labels.Add(left);
                    Controls.Add(left);
                    left.Left = button2.Left - 7 - left.Width;
                    top += 7 + right.Height;
                    lineTops.Add(top - 3);
                }
                top += 3;
            }

            Invalidate();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int i = wordPackungen.IndexOf(currentWordPackung) + 1;
            if (wordPackungen.Count == i)
                i = 0;
            currentWordPackung = wordPackungen[i];
            ShowWord();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = wordPackungen.IndexOf(currentWordPackung) - 1;
            if (-1 == i)
                i = wordPackungen.Count - 1;
            currentWordPackung = wordPackungen[i];
            ShowWord();
        }

        private void Show_Load(object sender, EventArgs e)
        {
            if (wordPackungen.Count == 0)
            {
                Close();
                return;
            }
            currentWordPackung = wordPackungen[0];
            Show_Resize(sender, e);
        }

        private void Show_Resize(object sender, EventArgs e)
        {
            button1.Top = (button5.Top + button5.Height - button3.Top) / 2 - 35;
            button2.Top = button1.Top;
            label1.Left = (button4.Left + button4.Width - button3.Left) / 2 - label1.Width / 2;
            ShowWord();
        }

        private void Show_Paint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Red, 1);
            foreach (var item in lineTops)
                e.Graphics.DrawLine(pen, button2.Left - 7, item, button1.Left + button1.Width + 7, item);
        }
    }
}
