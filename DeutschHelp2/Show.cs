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
        List<Word> Words;
        public Show(List<Word> words)
        {
            Words = words;
            InitializeComponent();
        }

        private void Show_Load(object sender, EventArgs e)
        {
            Show_Resize(sender, e);
        }

        private void Show_Resize(object sender, EventArgs e)
        {
            button1.Top = (button5.Top + button5.Height - button3.Top) / 2 - 35;
            button2.Top = button1.Top;
        }
    }
}
