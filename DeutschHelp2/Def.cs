using System.Collections.Generic;
using HtmlAgilityPack;

namespace DeutschHelp2
{
    [HasXPath]
    class Def
    {
        [XPath("/div[1]/div[@class='panel-title definition']/a/span[@class='de']")]
        public string Deu { get; set; }


        [XPath("/div[1]/div[@class='panel-title definition']/a/span[@class='fa']")]
        public string Fa { get; set; }


        /*
        [XPath("/div[2]/div[@class='panel-body']/div")]
        public IEnumerable<Example> Examples { get; set; }
        */

    }
}
