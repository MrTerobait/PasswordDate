using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MainWindow
{
    /// <summary>
    /// Interaction logic for PasswordGenerator.xaml
    /// </summary>
    public partial class PasswordGenerator : Window
    {
        public PasswordGenerator(int amountSymbols, bool isRemoveCapitalLetters, bool isRemoveSigns)
        {
            InitializeComponent();
            AmountSymbolsSlider.Value = amountSymbols;
            IsRemoveCapitalLettersCheckBox.IsChecked = isRemoveCapitalLetters;
            IsRemoveSignsCheckBox.IsChecked = isRemoveSigns;
            PasswordField.TextChanged += PasswordField_TextChanged;
        }

        private void PasswordGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            int amountSymbols = (int)AmountSymbolsSlider.Value;
            bool isRemoveCapitalLetters = (bool)IsRemoveCapitalLettersCheckBox.IsChecked;
            bool isRemoveSigns = (bool)IsRemoveSignsCheckBox.IsChecked;
            PasswordField.Text = Tools.GeneratePassword(amountSymbols, !isRemoveCapitalLetters, !isRemoveSigns);
        }

        private void AmountSymbolsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AmountSymbolsField.Content = $"Количество символов: {AmountSymbolsSlider.Value}";
        }
        private static class Tools
        {
            enum SymbolType
            {
                Number = 48,
                Letter = 97,
                CapitalLetter = 65,
                Sign = 32
            }
            private static Random random = new Random();
            public static string GeneratePassword(int length, bool withCapitalLetters, bool withSigns)
            {
                string password = "";
                var allSymbols = new List<SymbolType>() { SymbolType.Number, SymbolType.Letter };
                if (withCapitalLetters)
                {
                    allSymbols.Add(SymbolType.CapitalLetter);
                }
                if (withSigns)
                {
                    allSymbols.Add(SymbolType.Sign);
                }
                for (int i = 0; i < length; i++)
                {
                    password += GenerateSymbol(allSymbols);
                }
                return password;
            }
            private static char GenerateSymbol(List<SymbolType> allTypesOfSymbols)
            {
                char symbol = ' ';

                SymbolType symbolType = allTypesOfSymbols[random.Next(allTypesOfSymbols.Count)];
                switch (symbolType)
                {
                    case SymbolType.Number:
                        symbol = (char)((int)SymbolType.Number + random.Next(10));
                        break;
                    case SymbolType.Letter:
                        symbol = (char)((int)SymbolType.Letter + random.Next(26));
                        break;
                    case SymbolType.CapitalLetter:
                        symbol = (char)((int)SymbolType.CapitalLetter + random.Next(26));
                        break;
                    case SymbolType.Sign:
                        var encodingSigns = new int[33]{ 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
                        58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94, 95, 96, 123, 124, 125, 126 };
                        symbol = (char)encodingSigns[random.Next(33)];
                        break;
                }
                return symbol;
            }
        }

        private void CopyPasswordLabel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clipboard.SetText(PasswordField.Text);
            CopyPasswordLabel.Content = "Скопировано";
            CopyPasswordLabel.Foreground = Brushes.LightGreen;
        }

        private void PasswordField_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CopyPasswordLabel.Content = "Скопировать";
            CopyPasswordLabel.Foreground = Brushes.DarkRed;
            CopyPasswordLabel.FontWeight = FontWeights.Bold;
        }
    }
}
