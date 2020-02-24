using HtmlAgilityPack;

namespace DeutschHelp2
{
    [HasXPath]
    class Example
    {

        
        [XPath("/div[@class='row']/div[@class='fa-example-wrapper']/p[@class='fa text-right']")]
        public string DeuEx { get; set; }

        
        [XPath("/div[@class='row']/div[@class='de-example-wrapper']/p[@class='de text-left']")]
        public string Fa { get; set; }
        
    }
}
