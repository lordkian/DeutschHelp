using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DeutschHelp
{
    public partial class Form1 : Form
    {
        List<Word> words = new List<Word>();
        List<WordPackung> wordPackungen = new List<WordPackung>();
        SuggestionCrawler suggestionCrawler = new SuggestionCrawler();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            words.Clear();
            wordPackungen.Clear();
            var str = Regex.Replace(textBox1.Text, "\\s+", " ").Replace("\r", "").Replace(".", "").Replace(",", "");
            var strs = str.Split(' ').Where(d => d.Length > 0).Distinct().ToList();
            int i = 0;
            suggestionCrawler.GetCookie();
            foreach (var item in strs)
            {
                var slist = suggestionCrawler.GetSuggestions(item);
                var wlist = new List<Word>();
                foreach (var item2 in slist)
                {
                    var url = "https://wort.ir" + item2.full_slug;
                    HtmlWeb htmlWeb = new HtmlWeb();
                    var html = htmlWeb.Load(url);
                    try
                    {
                        wlist.Add(html.DocumentNode.GetEncapsulatedData<Word>());
                    }
                    catch (Exception) { }
                }
                if (wlist.Count > 0)
                    wordPackungen.Add(new WordPackung() { Text = item, Words = wlist });
                if (wlist.Count > 0)
                    words.Add(Word.Merge(wlist));
                i++;
                progressBar1.Value = 100 * i / strs.Count;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (wordPackungen.Count == 0)
            {
                MessageBox.Show("There is no word to show.");
                return;
            }
            var sfd = new SaveFileDialog()
            {
                Filter = "json object (*.json) | *.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "save all words"
            };
            var res = sfd.ShowDialog();
            if (res == DialogResult.Cancel)
                return;
            var sw = new StreamWriter(sfd.FileName);
            sw.WriteLine(JsonConvert.SerializeObject(new Serializable() { Text = textBox1.Text, WordPackungen = wordPackungen, Version = 1.2 }, Formatting.Indented));
            sw.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "json object (*.json) | *.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "load all words"
            };
            var res = ofd.ShowDialog();
            if (res == DialogResult.Cancel)
                return;
            var sr = new StreamReader(ofd.FileName);
            words.Clear();
            textBox1.Text = "";
            var str = sr.ReadToEnd();
            sr.Close();

            try
            {
                var s = JsonConvert.DeserializeObject<Serializable>(str);
                if (s.Version == 1.1)
                {
                    foreach (var item in s.Words)
                        wordPackungen.Add(new WordPackung() { Text = item.Text, Words = new List<Word>() { item } });
                    textBox1.Text = s.Text;
                }
                else if (s.Version == 1.2)
                {
                    textBox1.Text = s.Text;
                    wordPackungen.AddRange(s.WordPackungen);
                }
            }
            catch (Exception)
            {
                foreach (var item in JsonConvert.DeserializeObject<List<Word>>(str))
                    wordPackungen.Add(new WordPackung() { Text = item.Text, Words = new List<Word>() { item } });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (wordPackungen.Count == 0)
            {
                MessageBox.Show("There is no word to show.");
                return;
            }
            new Show(wordPackungen).Show();
        }

        public void ChangeProgressBarPercentage(int percentage)
        {
            progressBar1.Invoke(new Action(() => { progressBar1.Value = percentage; }));
        }
    }
}
