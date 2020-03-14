using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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


        public List<Suggestion> GetData()
        {
            // Load Page via Get Request
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create("https://wort.ir");

            // Get Cookies
            getRequest.CookieContainer = new CookieContainer();
            HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();

            // Get Token
            string getResponseString = new StreamReader(getResponse.GetResponseStream()).ReadToEnd();
            string tokenPattern = "<body data-vtn=\"(\\w+)\"";
            string getToken = Regex.Match(getResponseString, tokenPattern).Groups[1].Value;

            // Generate Body
            string word = "buc";
            string postData = $"search={word}&_token={getToken}";
            byte[] data = Encoding.ASCII.GetBytes(postData);

            // Generate Post Request
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create("https://wort.ir/woerterbuch/ac");

            // Add Cookies
            postRequest.CookieContainer = new CookieContainer();
            postRequest.CookieContainer.Add(getResponse.Cookies);

            // Add Headers
            postRequest.Headers["authority"] = "wort.ir";
            postRequest.Method = "POST";
            postRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            postRequest.ContentLength = data.Length;

            // Send Post Stream
            using (var stream = postRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);

            }

            // Get Response
            HttpWebResponse postResponse = (HttpWebResponse)postRequest.GetResponse();
            string postResponseString = new StreamReader(postResponse.GetResponseStream()).ReadToEnd();

            // Release Resources
            getResponse.Close();
            postResponse.Close();

            // Deserialize response
            List<Suggestion> postResult =JsonSerializer.Deserialize<List<Suggestion>>(postResponseString);

            return postResult;
        }


    }



    public class Suggestion
    {
        public string label { get; set; }
        public string type_string { get; set; }
        public string full_slug { get; set; }
    }

}
