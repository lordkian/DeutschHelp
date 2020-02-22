using System.Collections.Generic;

namespace DeutschHelp2
{
    class Word
    {
        public string Text { get; set; }
        public List<Def> Defs { get; } = new List<Def>();
    }
}
