using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T9.Models
{
    class T9CharacterModel
    {
        public static Dictionary<char, char[]> T9CharDictionary = new Dictionary<char, char[]>() {
            { '2', new char[]{'a', 'b', 'c' }},
            { '3', new char[]{'d', 'e', 'f' }},
            { '4', new char[]{'g', 'h', 'i' }},
            { '5', new char[]{'j', 'k', 'l' }},
            { '6', new char[]{'m', 'n', 'o' }},
            { '7', new char[]{'p', 'q', 'r', 's' }},
            { '8', new char[]{'t', 'u', 'v' }},
            { '9', new char[]{'w', 'x', 'y', 'z' }},
            { '#', new char[]{' '}}
        };

        // Retrives the character associated with the key at a given index
        public static char GetCharacter(char key, int index) {
            char finalChar;
            var dictLength = T9CharDictionary[key].Length;

            if (index >= dictLength)
                finalChar = T9CharDictionary[key][dictLength - 1];
            else if (index < 0)
                finalChar = T9CharDictionary[key][0];
            else
                finalChar = T9CharDictionary[key][index];

            return finalChar;
        }
    }
}
