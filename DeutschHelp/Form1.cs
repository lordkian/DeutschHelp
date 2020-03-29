using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeutschHelp
{
    public partial class Form1 : Form
    {
        private readonly object wordPackungenLock = new object();
        private readonly object percentLock = new object();
        List<WordPackung> wordPackungen = new List<WordPackung>();
        CancellationTokenSource tokenSource2;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (Regex.Replace(textBox1.Text.Replace(".", " ").Replace(",", " ").Replace("\r", " "), "\\s+", "").Length == 0)
            {
                MessageBox.Show("There is no word to show.");
                return;
            }

            wordPackungen.Clear();

            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button1.Text = "Stop";
            button1.Click -= button1_Click;
            button1.Click += button1_Click2;

            tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;
            Task.Run(() => Map(textBox1.Text))
                .ContinueWith(t => Process(t.Result, ct))
                .ContinueWith(t => Reduce(t.Result));
        }
        private List<string> Map(string text)
        {
            return Regex.Replace(
                text.Replace(".", " ").Replace(",", " ").Replace("\r", " ")
                , "\\s+", " ")
                .Split().ToList();
        }
        private List<string> Reduce(List<string> words)
        {
            wordPackungen.Sort((s1,s2) => { return words.IndexOf(s1.Text).CompareTo(words.IndexOf(s2.Text)); });
            Finish();
            return words;
        }
        private List<string> Process(List<string> words, CancellationToken cancellationToken)
        {
            int stat = 0;
            ChangeProgressBarPercentage(0);
            var suggestionCrawler = new SuggestionCrawler();
            suggestionCrawler.GetCookie();
            if (cancellationToken.IsCancellationRequested)
            {
                Finish();
                cancellationToken.ThrowIfCancellationRequested();
            }
            words.ForEach((word) =>
            {
                Task.Factory.StartNew(() =>
                {
                    var wp = ProcessString(word, cancellationToken, suggestionCrawler);
                    lock (wordPackungenLock)
                    {
                        if (wp != null)
                            wordPackungen.Add(wp);
                    }
                    lock (percentLock)
                    {
                        stat++;
                        ChangeProgressBarPercentage(100 * stat / words.Count);
                    }
                }, cancellationToken: cancellationToken, creationOptions: TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning, scheduler: TaskScheduler.Default);
            });
            return words;
        }
        private WordPackung ProcessString(string text, CancellationToken cancellationToken, SuggestionCrawler suggestionCrawler)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Finish();
                cancellationToken.ThrowIfCancellationRequested();
            }
            var slist = suggestionCrawler.GetSuggestions(text);
            if (slist == null)
                return null;
            var wlist = new List<Word>();
            foreach (var item2 in slist)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Finish();
                    cancellationToken.ThrowIfCancellationRequested();
                }
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
                return new WordPackung() { Text = text, Words = wlist };
            else
                return null;
        }
        private void button1_Click2(object sender, EventArgs e)
        {
            tokenSource2.Cancel();
        }
        public void Finish()
        {
            if (InvokeRequired)
                Invoke(new Action(Finish));
            else
            {
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button1.Text = "Download";
                button1.Click -= button1_Click2;
                button1.Click += button1_Click;
                progressBar1.Value = 0;
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
            wordPackungen.Clear();
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
