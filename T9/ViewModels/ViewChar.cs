using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T9.ViewModels
{
    class ViewChar
    {
        public int currCharIndex { get; set; }
        public int dictSize { get; set; }
        public char charKey { get; set; }
        public DateTime timeStart { get; set; }

        public ViewChar(char charKey, int dictSize)
        {
            this.currCharIndex = 0;
            this.dictSize = dictSize;
            this.charKey = charKey;
            this.timeStart = DateTime.Now;
        }
    }
}
