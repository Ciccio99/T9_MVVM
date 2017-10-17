using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace T9.Models {
    class PredictiveDictionaryModel {
        private List<KeyValuePair<string, string>> _predictiveDictionary;

        public PredictiveDictionaryModel () {
            _predictiveDictionary = new List<KeyValuePair<string, string>> ();
        }

        public string EncodeString (string text) {
            string result = text.ToLower ();

            result = Regex.Replace (result, "[2-9]", string.Empty);

            result = Regex.Replace (result, "[abc]", "2");
            result = Regex.Replace (result, "[def]", "3");
            result = Regex.Replace (result, "[ghi]", "4");
            result = Regex.Replace (result, "[jkl]", "5");
            result = Regex.Replace (result, "[mno]", "6");
            result = Regex.Replace (result, "[pqrs]", "7");
            result = Regex.Replace (result, "[tuv]", "8");
            result = Regex.Replace (result, "[wxyz]", "9");

            return result;
        }

        public List<string> GetWordPredictions (string wordPrefix) {
            var predictionsList = (from word in _predictiveDictionary
                                   orderby word.Key.Length ascending
                                   where word.Key.StartsWith (wordPrefix)
                                   select word.Value).ToList ();
            return predictionsList;
        }

        public void LoadDictionary (string filename) {
            string line;
            var filepath = Path.GetFullPath (filename);
            using (var fs = new FileStream (filepath, FileMode.Open, FileAccess.Read)) {
                using (StreamReader fileStream = new System.IO.StreamReader (fs)) {
                    while ((line = fileStream.ReadLine ()) != null) {
                        AddWord (line);
                    }
                }
            }
        }

        public void AddWord (string word) {
            if (_predictiveDictionary == null)
                _predictiveDictionary = new List<KeyValuePair<string, string>> ();

            _predictiveDictionary?.Add(new KeyValuePair<string, string> (EncodeString (word), word));
        }
    }
}
