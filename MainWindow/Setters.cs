using System;
using System.Collections;
using System.IO;
using System.Windows.Controls;
using MainWindow.SettersUserControls;

namespace MainWindow
{
    public class Setters
    {
        private string _daysBeforeRecordingAging = "30";
        private string _basketLimit = "100";
        private string _mail = "";
        private string _passwordGeneratorDefaultSymbolsAmount = "10";
        private string _isRemoveCapitalLetters = "false";
        private string _isRemoveSigns = "false";
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
        public string Mail
        {
            get
            {
                return _mail;
            }
        }
        public ArrayList PasswordGeneratorParams
        {
            get
            {
                return new ArrayList() { int.Parse(_passwordGeneratorDefaultSymbolsAmount), bool.Parse(_isRemoveCapitalLetters), bool.Parse(_isRemoveSigns) };
            }
        }

        private static readonly string settersParametersPath = $"{Environment.CurrentDirectory}\\setters_param.txt";
        public Setters()
        {
            if (File.Exists(settersParametersPath))
            {
                    string[] settersInfo = File.ReadAllLines(settersParametersPath);
                    _daysBeforeRecordingAging = settersInfo[0];
                    _basketLimit = settersInfo[1];
                    _mail = settersInfo[2];
                    _passwordGeneratorDefaultSymbolsAmount = settersInfo[3];
                    _isRemoveCapitalLetters = settersInfo[4];
                    _isRemoveSigns = settersInfo[5];
            }
            else
            {
                SaveSetters();
            }
        }
        public void SaveSetters()
        {
            File.WriteAllLines(settersParametersPath, new string[] { _daysBeforeRecordingAging, _basketLimit, _mail, _passwordGeneratorDefaultSymbolsAmount,
                    _isRemoveCapitalLetters, _isRemoveSigns } );
        }
        public static Setters ResetSetters()
        {
            File.Delete(settersParametersPath);
            return new Setters();
        }

        public Action editorSetterIsClosed;
        public UserControl OpenEditorSetter(string setter)
        {
            if (setter == "Время старения записи")
            {
                var uc = new daysBeforeRecordingAgingUC(_daysBeforeRecordingAging);
                uc.GetValue += ChangeDaysBeforeRecordingAging;
                return uc;
            }
            else if(setter == "Лимит корзины")
            {
                var uc = new BasketLimitUC(_basketLimit);
                uc.GetValue += ChangeBasketLimit;
                return uc;
            }
            else if(setter == "Почта для отправки данных")
            {
                var uc = new MailUC(_mail);
                uc.GetValue += ChangeMailButton;
                return uc;
            }
            else if(setter == "Количество символов")
            {
                var uc = new passwordGeneratorDefaultSymbolsAmountUC(_passwordGeneratorDefaultSymbolsAmount);
                uc.GetValue += ChangePasswordGeneratorDefaultSymbolsAmountButton;
                return uc;
            }
            else if(setter == "Убрать заглавные буквы")
            {
                var uc = new IsRemoveCapitalLettersUC();
                uc.GetValue += ChangeIsRemoveCapitalLettersBitton;
                return uc;
            }
            else if(setter == "Убрать знаки")
            {
                var uc = new IsRemoveSignsUC();
                uc.GetValue += ChangeIsRemoveSignsBitton;
                return uc;
            }
            throw new Exception();
        }
        private void ChangeDaysBeforeRecordingAging(uint value)
        {
            _daysBeforeRecordingAging = $"{value}";
            SaveSetters();
            CloseEditorSetter();
        }
        private void ChangeBasketLimit(uint value)
        {
            _basketLimit = $"{value}";
            SaveSetters();
            CloseEditorSetter();
        }
        private void ChangeMailButton(string value)
        {
            _mail = $"{value}";
            SaveSetters();
            CloseEditorSetter();
        }
        private void ChangePasswordGeneratorDefaultSymbolsAmountButton(uint value)
        {
            _passwordGeneratorDefaultSymbolsAmount = $"{value}";
            SaveSetters();
            CloseEditorSetter();
        }
        private void ChangeIsRemoveCapitalLettersBitton(bool value)
        {
            _isRemoveCapitalLetters = $"{value}";
            SaveSetters();
            CloseEditorSetter();
        }
        private void ChangeIsRemoveSignsBitton(bool value)
        {
            _isRemoveSigns = $"{value}";
            SaveSetters();
            CloseEditorSetter();
        }
        private void CloseEditorSetter()
        {
            editorSetterIsClosed?.Invoke();
        }
    }
}
