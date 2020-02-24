using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;

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
            foreach (var item in textBox1.Text.Replace('\n', ' ').Replace("\r", "").Split(' '))
            {
                var url = "https://wort.ir/woerterbuch/deutsch-persisch/" + item;
                HtmlWeb htmlWeb = new HtmlWeb();
                var html = htmlWeb.Load(url);
                var html2 = htmlWeb.Load(url + "-2");
                Word w1 = null, w2 = null;
                try
                {
                    w1 = html.DocumentNode.GetEncapsulatedData<Word>();
                }
                catch (Exception) { }
                try
                {
                    w2 = html2.DocumentNode.GetEncapsulatedData<Word>();
                }
                catch (Exception) { }
                words.Add(Word.Merge(w1, w2));
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ofd = new SaveFileDialog()
            {
                Filter = "json object (*.json)",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "save all words"
            };
            ofd.ShowDialog();
            var sw = new StreamWriter(ofd.FileName);
            sw.WriteLine(JsonConvert.SerializeObject(words, Formatting.Indented));
            sw.Close();
        }
    }
}
