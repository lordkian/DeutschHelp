using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeutschHelp2
{
    public partial class Show : Form
    {
        Word CurrentWord = null;
        List<Word> Words;
        List<Label> labels = new List<Label>();
        List<int> lineTops = new List<int>();
        public Show(List<Word> words)
        {
            Words = words.Distinct().ToList();
            if (Words.Contains(null))
                Words.Remove(null);
            InitializeComponent();
        }
        private void Show_Load(object sender, EventArgs e)
        {
            if (Words.Count == 0)
            {
                Close();
                return;
            }
            CurrentWord = Words[0];
            Show_Resize(sender, e);
        }
        private void Show_Resize(object sender, EventArgs e)
        {
            button1.Top = (button5.Top + button5.Height - button3.Top) / 2 - 35;
            button2.Top = button1.Top;
            ShowWord();
            label1.Left = (button4.Left + button4.Width - button3.Left) / 2 - label1.Width / 2;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int i = Words.IndexOf(CurrentWord) + 1;
            if (Words.Count == i)
                i = 0;
            CurrentWord = Words[i];
            ShowWord();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int i = Words.IndexOf(CurrentWord) - 1;
            if (-1 == i)
                i = Words.Count - 1;
            CurrentWord = Words[i];
            ShowWord();
        }
        private void ShowWord()
        {
            foreach (var item in labels)
            {
                Controls.Remove(item);
                item.Dispose();
            }

            labels.Clear();
            label1.Text = CurrentWord.Text;
            int top = button3.Top + button3.Height + 7;
            lineTops.Clear();
            
            foreach (var item in CurrentWord.Defs)
            {
                var right = new Label()
                {
                    Top = top,
                    Left = 77 + button3.Left,
                    Text = item.Deu,
                    AutoSize = true,
                    Font = label2.Font
                };
                var left = new Label()
                {
                    Top = top,
                    Text = item.Fa,
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
            Invalidate();
        }
        private void Show_Paint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Red, 1);
            foreach (var item in lineTops)
            {
                e.Graphics.DrawLine(pen, button2.Left - 7, item, button1.Left + button1.Width + 7, item);
            } 
        }
    }
}
