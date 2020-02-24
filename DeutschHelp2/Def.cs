using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using HtmlAgilityPack;

namespace DeutschHelp2
{
    [HasXPath]
    [DataContract]
    [DebuggerDisplay("{Deu} : {Fa}")]
    public class Def
    {
        [XPath("/div[1]/div[@class='panel-title definition']/a/span[@class='de']")]
        [DataMember]
        public string Deu { get; set; }


        [XPath("/div[1]/div[@class='panel-title definition']/a/span[@class='fa']")]
        [DataMember]
        public string Fa { get; set; }


        /*
        [XPath("/div[2]/div[@class='panel-body']/div")]
        public IEnumerable<Example> Examples { get; set; }
        */

    }
}
