﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using HtmlAgilityPack;

namespace DeutschHelp2
{
    [HasXPath]
    [DataContract]
    [DebuggerDisplay("Word={Text}")]
    class Word
    {
        [XPath("//*[@id='content-wrapper']/div/h1/text()")]
        [DataMember]
        public string Text { get; set; }

        [XPath("//*[@id='accordion']/div")]
        [DataMember]
        public List<Def> Defs { get; set; } = new List<Def>();
        public static Word Merge(Word w1, Word w2)
        {
            if (w2 == null)
                return w1;
            if (w1 == null)
                return w2;
            if (w1.Text != w2.Text)
                throw new Exception("Not the same word");
            var w = new Word() { Text = w1.Text };
            w.Defs.AddRange(w1.Defs);
            w.Defs.AddRange(w2.Defs);
            return w;
        }
    }
}
