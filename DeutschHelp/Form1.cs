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
        SuggestionCrawler suggestionCrawler = new SuggestionCrawler();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            words.Clear();
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
                    words.Add(Word.Merge(wlist));
                i++;
                progressBar1.Value = 100 * i / strs.Count;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "json object (*.json) | *.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "save all words"
            };
            sfd.ShowDialog();
            var sw = new StreamWriter(sfd.FileName);
            sw.WriteLine(JsonConvert.SerializeObject(new Serializable() { Text = textBox1.Text, Words = words, Version = 1.1 }, Formatting.Indented));
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
            ofd.ShowDialog();

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
                    words.AddRange(s.Words);
                    textBox1.Text = s.Text;
                }
            }
            catch (Exception)
            {
                words.AddRange(JsonConvert.DeserializeObject<List<Word>>(str));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Show(words).Show();
        }
    }
}
