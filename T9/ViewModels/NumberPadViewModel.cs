using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using T9.Utilities;
using T9.Models;
using System.Diagnostics;

namespace T9.ViewModels
{
    public class NumberPadViewModel : INotifyPropertyChanged {

        public ObservableCollection<string> PredictiveWords { get; set; }
        public StringBuilder CurrentInput {
            get {
                return _currentInput;
            }
            set {
                _currentInput = value;
                OnPropertyChanged("CurrentInput");
            }
        }      
        public bool PredictiveModeChecked {
            get {
                return _predictiveModeChecked;
            }
            set {
                _predictiveModeChecked = value;
                OnPropertyChanged ("PredictiveModeChecked");
            }
        }
        public string SelectedPrediction {
            get {
                return _selectedPrediction;
            }
            set {
                _selectedPrediction = value;
                OnPropertyChanged ("SelectedPrediction");
            }
        }
        // Commands activated via UI interaction
        public VMICommand CharButtonCommand { get; set; }

        private PredictiveDictionaryModel _predictiveDictionary;
        private readonly int _setCharTimeInterval = 400;
        private StringBuilder _currentInput;
        private string _selectedPrediction;
        private int _selectedPredictionIndex;
        private ViewChar _currentViewChar;
        private bool _predictiveModeChecked;
        private Stopwatch _charStopWatch;

        // On startup of viewModel
        public NumberPadViewModel() {
            CharButtonCommand = new VMICommand (OnCharacterPress);
            PredictiveWords = new ObservableCollection<string> ();
            _currentInput = new StringBuilder ();
            _predictiveDictionary = new PredictiveDictionaryModel ();
            _predictiveDictionary.LoadDictionary ("Utilities\\english-words.txt");
            _predictiveModeChecked = false;
            _charStopWatch = new Stopwatch ();
        }

        private void OnCharacterPress (object param) {
            var paramString = param as string;
            char key = paramString[0];

            switch (key) {
                case '*':
                    OnBackspace ();
                    if (PredictiveModeChecked) {
                        GetCurrentPredictions ();
                    } else {
                        
                        _currentViewChar = null;
                    }
                    break;
                case '1':
                    _currentViewChar = null;
                    break;
                case '#':
                    if (PredictiveModeChecked && SelectedPrediction != null) {
                        SetSelectedWord ();
                    }
                    OnCharacter (key);
                    break;
                case '0':
                    if (PredictiveModeChecked) {
                        CyclePredictionSelections ();
                    } else {
                        _currentViewChar = null;
                    }
                    
                    break;
                default:
                    if (PredictiveModeChecked) {
                        OnCharacterPredictive (key);
                    } else {
                        OnCharacter (key);
                    }
                    break;
            }
        }

        private void AppendCurrChar() {
            char finalChar = T9CharacterModel.GetCharacter (_currentViewChar.charKey, _currentViewChar.currCharIndex);
            _currentInput.Append(finalChar);
            OnPropertyChanged("CurrentInput");
        }

        private void AppendChar (char key) {
            _currentInput.Append (T9CharacterModel.GetCharacter (key, 0));
            OnPropertyChanged ("CurrentInput");
        }

        private void SetSelectedWord () {
            var tokens = GetInputTokens (_currentInput.ToString ());
            var currentWord = tokens[tokens.Length - 1];
            _currentInput.Remove (_currentInput.Length - currentWord.Length, currentWord.Length);
            _currentInput.Append (SelectedPrediction);
            PredictiveWords.Clear ();
            SelectedPrediction = null;
            _selectedPredictionIndex = 0;
            OnPropertyChanged ("SelectedPrediction");
            OnPropertyChanged ("PredictiveWords");
        }

        private void CyclePredictionSelections () {
            if (PredictiveWords.Count < 1) {
                SelectedPrediction = null;
                return;
            }

            _selectedPredictionIndex++;
            if (_selectedPredictionIndex >= PredictiveWords.Count) {
                _selectedPredictionIndex = 0;
                SelectedPrediction = PredictiveWords[_selectedPredictionIndex];
                return;
            }

            SelectedPrediction = PredictiveWords[_selectedPredictionIndex];
            OnPropertyChanged ("SelectedPrediction");
        }

        private void SwapLastWithCurrChar () {
            char finalChar = T9CharacterModel.GetCharacter (_currentViewChar.charKey, _currentViewChar.currCharIndex);

            if (_currentInput.Length > 0) {
                _currentInput.Remove (_currentInput.Length - 1, 1);
                _currentInput.Append (finalChar);
            } else {
                _currentInput.Append (finalChar);
            }
            
            OnPropertyChanged("CurrentInput");
        }

        private void OnCharacter(char key) {
            // If nothing has been pressed in a while or at start
            if (_currentViewChar == null) {
                _currentViewChar = new ViewChar (key, T9CharacterModel.T9CharDictionary[key].Length);
                AppendCurrChar ();
                //Timer stuff
                _charStopWatch.Reset ();
                _charStopWatch.Start ();
                return;
            }
            // If pressing same character 
            if (key == _currentViewChar.charKey) {
                _currentViewChar.currCharIndex++;
                _charStopWatch.Stop ();
                if (_charStopWatch.ElapsedMilliseconds < _setCharTimeInterval) {
                    if (_currentViewChar.currCharIndex >= _currentViewChar.dictSize) {
                        _currentViewChar = new ViewChar (key, T9CharacterModel.T9CharDictionary[key].Length);
                        AppendCurrChar ();
                        _charStopWatch.Reset ();
                        _charStopWatch.Start ();
                    } else {
                        SwapLastWithCurrChar ();
                        _charStopWatch.Reset ();
                        _charStopWatch.Start ();
                    }
                } else {
                    _currentViewChar = new ViewChar (key, T9CharacterModel.T9CharDictionary[key].Length);
                    AppendCurrChar ();
                    _charStopWatch.Reset ();
                    _charStopWatch.Start ();
                }
                return;
            }

            // if new char pressed compared to last char
            _currentViewChar = new ViewChar(key, T9CharacterModel.T9CharDictionary[key].Length);
            AppendCurrChar ();
            _charStopWatch.Reset ();
            _charStopWatch.Start ();
        }

        private void OnCharacterPredictive (char key) {
            if (_currentInput.Length < 2) {
                AppendChar (key);
                return;
            }

            AppendChar (key);
            GetCurrentPredictions ();
        }

        private void GetCurrentPredictions () {
            // Don't predict unless we have three letters minimum
            var tokens = GetInputTokens (_currentInput.ToString ());

            if (tokens.Length < 1 || tokens[tokens.Length - 1].Length < 3) {
                PredictiveWords.Clear ();
                SelectedPrediction = null;
                return;
            }

            PredictiveWords.Clear ();

            var predictedWords = _predictiveDictionary.GetWordPredictions (_predictiveDictionary.EncodeString(tokens[tokens.Length - 1]));
            if (predictedWords.Count > 0) {
                foreach (var word in predictedWords.Take (4)) {
                    PredictiveWords.Add (word);
                }
                SelectedPrediction = PredictiveWords.First ();

            } else {
                SelectedPrediction = null;
            }
            _selectedPredictionIndex = 0;
            OnPropertyChanged ("SelectedPrediction");
            OnPropertyChanged ("PredictiveWords");
        }

        private string[] GetInputTokens (string input) {
            return input.ToString ().Split ((char[]) null, StringSplitOptions.RemoveEmptyEntries);
        }

        private void OnBackspace() {
            if (_currentInput?.Length > 0) {
                _currentInput.Remove(_currentInput.Length - 1, 1);
                OnPropertyChanged("CurrentInput");
            }
        }



        




        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
