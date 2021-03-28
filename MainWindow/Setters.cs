using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MainWindow.SettersUserControls;

namespace MainWindow
{
    public class Setters
    {
        private string _daysBeforeRecordingAging = "30";
        private string _basketLimit = "30";
        private string _mail = "";
        private string _passwordGeneratorDefaultSymbolsAmount = "10";
        private string _isRemoveCapitalLetters = "0";
        private string _isRemoveSigns = "0";
        public int DaysBeforeRecordingAging
        {
            get
            {
                return  int.Parse(_daysBeforeRecordingAging);
            }
        }
        public int BasketLimit
        {
            get
            {
                return int.Parse(_basketLimit);
            }
        }
        public ArrayList PasswordGeneratorParams
        {
            get
            {
                return new ArrayList() { int.Parse(_passwordGeneratorDefaultSymbolsAmount), bool.Parse(_isRemoveCapitalLetters), bool.Parse(_isRemoveSigns) };
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
                    _daysBeforeRecordingAging = settersInfo[0];
                    _basketLimit = settersInfo[1];
                    _mail = settersInfo[2];
                    _passwordGeneratorDefaultSymbolsAmount = settersInfo[3];
                    _isRemoveCapitalLetters = settersInfo[4];
                    _isRemoveSigns = settersInfo[5];
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
        }
        ~Setters()
        {
            File.WriteAllLines(settersParametersPath, new string[] { _daysBeforeRecordingAging, _basketLimit, _mail, _passwordGeneratorDefaultSymbolsAmount,
                    "false", "false" } );
        }

        private Border editorSetter;
        private string setterValue;
        public Border OpenEditorSetter(string setter)
        {
            editorSetter = new Border() { BorderThickness = new Thickness(0, 0, 0, 2), BorderBrush = Brushes.Black };
            editorSetter.Child = new daysBeforeRecordingAgingUC(_daysBeforeRecordingAging);
            if (setter == "Время старения записи")
            {
                return editorSetter;
            }
            throw new Exception();
        }

    }
}
