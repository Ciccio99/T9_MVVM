/*
 * Author: Alberto Scicali
 * Dictionary model for storing a given dictionary file and finding the word prediction for a given word/prefix
 */
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

        /*
         * Constructor which instatiates the dictionary
         */
        public PredictiveDictionaryModel () {
            _predictiveDictionary = new List<KeyValuePair<string, string>> ();
        }

        /*
         * Uses LINQ to find a list of word predictions given a prefix/word
         * @param word/prefix to find predictions for
         */
        public List<string> GetWordPredictions (string wordPrefix) {
            var predictionsList = (from word in _predictiveDictionary
                                   orderby word.Key.Length ascending
                                   where word.Key.StartsWith (wordPrefix)
                                   select word.Value).ToList ();
            return predictionsList;
        }

        /*
         * Given a file of words, loads the words into the dictionary for processing
         * @param name of dictionary file
         */
        public void LoadStringDictionary (string filename) {
            string line;
            //var filepath = Path.GetFullPath (filename);
            var filepath = filename;
            using (var fs = new FileStream (filepath, FileMode.Open, FileAccess.Read)) {
                using (StreamReader fileStream = new System.IO.StreamReader (fs)) {
                    while ((line = fileStream.ReadLine ()) != null) {
                        AddStringWord (Regex.Replace(line, @"\t\n\r", ""));
                    }
                }
            }
        }

        /*
         * Adds a new definition to the dictionary
         * @param Word to be added
         */
        private void AddStringWord (string word) {
            if (_predictiveDictionary == null)
                _predictiveDictionary = new List<KeyValuePair<string, string>> ();

            _predictiveDictionary?.Add (new KeyValuePair<string, string> (word, word));
        }
    }
}
