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
    public partial class Form1 : Form
    {
        List<Word> words = new List<Word>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            words.Clear();
            foreach (var item in textBox1.Text.Replace('\n',' ').Replace("\r","").Split(' '))
                words.Add(new Word() { Text = item });
            foreach (var item in words)
            {
                var url = "https://wort.ir/woerterbuch/deutsch-persisch/" + item.Text;
            }
        }
    }
}
