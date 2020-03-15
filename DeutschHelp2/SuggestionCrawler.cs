using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace DeutschHelp2
{
    class SuggestionCrawler
    {
        string getToken;
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
            getToken = Regex.Match(getResponseString, tokenPattern).Groups[1].Value;

            // Generate Body


        }
        public List<Suggestion> GetSuggestions(string word)
        {
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
            var serializer = new JavaScriptSerializer();
            List<Suggestion> postResult = serializer.Deserialize<List<Suggestion>>(postResponseString);

            return postResult;
        }
    }
}
