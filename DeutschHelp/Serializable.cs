using System.Collections.Generic;

namespace DeutschHelp
{
    public class Serializable
    {
        public List<WordPackung> WordPackungen { get; set; }
        public List<Word> Words { get; set; }
        public string Text { get; set; }
        public double Version { get; set; }
    }
}
