using System.Collections.Generic;

namespace DeutschHelp2
{
    class Def
    {
        public string Deu { get; set; }
        public string Fa { get; set; }
        public List<Example> Examples { get; } = new List<Example>();
    }
}
