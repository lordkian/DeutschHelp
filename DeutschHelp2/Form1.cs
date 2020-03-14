using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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

        private void button3_Click(object sender, EventArgs e)
        {
            words.Clear();
            var str = Regex.Replace(textBox1.Text, "\\s+", "").Replace("\r", "").Replace(".", "").Replace(",", "");
            var strs = str.Split(' ');
            int i = 0;
            foreach (var item in strs)
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
                i++;
                progressBar1.Value = 100 * i / strs.Length;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "json object (*.json) | *.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "save all words"
            };
            sfd.ShowDialog();
            var sw = new StreamWriter(sfd.FileName);
            sw.WriteLine(JsonConvert.SerializeObject(words, Formatting.Indented));
            sw.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "json object (*.json) | *.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "load all words"
            };
            ofd.ShowDialog();
            var sr = new StreamReader(ofd.FileName);
            words.AddRange(JsonConvert.DeserializeObject<List<Word>>(sr.ReadToEnd()));
            sr.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Show(words).Show();
        }
    }
}
