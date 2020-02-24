using System.Collections.Generic;
using System.Diagnostics;
using HtmlAgilityPack;

namespace DeutschHelp2
{
    [HasXPath]
    [DebuggerDisplay("Word={Text}")]
    class Word
    {
        [XPath("//*[@id='content-wrapper']/div/h1/text()")]
        public string Text { get; set; }

        [XPath("//*[@id='accordion']/div")]
        public List<Def> Defs { get; set; } = new List<Def>();
    }
}
