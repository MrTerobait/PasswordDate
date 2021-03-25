using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MainWindow
{
    public class Setters
    {
        private int _daysBeforeRecordingAging = 30;
        private int _basketLimit = 30;
        private int _passwordGeneratorDefaultSymbolsAmount = 10;
        private bool _isRemoveCapitalLetters = false;
        private bool _isRemoveSigns = false;
        public int DaysBeforeRecordingAging
        {
            get
            {
                return  _daysBeforeRecordingAging;
            }
        }
        public int BasketLimit
        {
            get
            {
                return _basketLimit;
            }
        }
        public ArrayList PasswordGeneratorParams
        {
            get
            {
                return new ArrayList() { _passwordGeneratorDefaultSymbolsAmount, _isRemoveCapitalLetters, _isRemoveSigns };
            }
        }
        private readonly string settersParametersPath = $"{Environment.CurrentDirectory}\\setters_param.txt";
        public Setters()
        {
            if (File.Exists(settersParametersPath))
            {
                try
                {
                    string[] settersInfo = File.ReadAllLines(settersParametersPath);
                    _daysBeforeRecordingAging = int.Parse(settersInfo[0]);
                    _basketLimit = int.Parse(settersInfo[1]);
                    _passwordGeneratorDefaultSymbolsAmount = int.Parse(settersInfo[2]);
                    _isRemoveCapitalLetters = bool.Parse(settersInfo[3]);
                    _isRemoveSigns = bool.Parse(settersInfo[4]);
                }
                catch (Exception)
                {
                    MessageBox.Show("файл с параметрами настроек не исправен");
                }
            }
            else
            {
                File.CreateText(settersParametersPath).Dispose();
            }
            Change();
        }
        ~Setters()
        {
            File.WriteAllLines(settersParametersPath, new string[] { $"{_daysBeforeRecordingAging}", $"{_basketLimit}", $"{_passwordGeneratorDefaultSymbolsAmount}",
                    $"{_isRemoveCapitalLetters}", $"{_isRemoveSigns}"});
        }
        Random random = new Random();
        public void Change()
        {
            _daysBeforeRecordingAging = 1;
            _basketLimit = 3;
            _passwordGeneratorDefaultSymbolsAmount = random.Next(0, 50);
            if (random.Next(0, 2) == 0)
            {
                _isRemoveCapitalLetters = true;
                _isRemoveSigns = true;
            }
            else
            {
                _isRemoveCapitalLetters = false;
                _isRemoveSigns = false;
            }
        }
        public static class Editor
        {
            public static Grid body = new Grid();
            static Editor()
            {

            }
        }
    }
}
