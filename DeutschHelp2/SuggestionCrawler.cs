using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeutschHelp2
{
    class SuggestionCrawler
    {
        byte[] data;
        HttpWebResponse getResponse;
        public void GetCookie()
        {
            // Load Page via Get Request
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create("https://wort.ir");

            // Get Cookies
            getRequest.CookieContainer = new CookieContainer();
            getResponse = (HttpWebResponse)getRequest.GetResponse();

            // Get Token
            string getResponseString = new StreamReader(getResponse.GetResponseStream()).ReadToEnd();
            string tokenPattern = "<body data-vtn=\"(\\w+)\"";
            string getToken = Regex.Match(getResponseString, tokenPattern).Groups[1].Value;

            // Generate Body
            string word = "buc";
            string postData = $"search={word}&_token={getToken}";
            data = Encoding.ASCII.GetBytes(postData);
        }
        public List<Suggestion> GetData()
        {
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
            List<Suggestion> postResult = JsonSerializer.Deserialize<List<Suggestion>>(postResponseString);

            return postResult;
        }
    }
}
