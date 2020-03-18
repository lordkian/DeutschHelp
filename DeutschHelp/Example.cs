using HtmlAgilityPack;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DeutschHelp2
{
    [HasXPath]
    [DataContract]
    [DebuggerDisplay("{DeuEx} : {Fa}")]
    public class Example
    {


        [XPath("/div[@class='row']/div[@class='fa-example-wrapper']/p[@class='fa text-right']")]
        [DataMember]
        public string DeuEx { get; set; }


        [XPath("/div[@class='row']/div[@class='de-example-wrapper']/p[@class='de text-left']")]
        [DataMember]
        public string Fa { get; set; }

    }
}
