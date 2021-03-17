using System.Windows;

namespace PasswordGenerator
{
    /// <summary>
    /// Interaction logic for PasswordGenerator.xaml
    /// </summary>
    public partial class Displayer : Window
    {
        public Displayer()
        {
            InitializeComponent();
        }

        private void PasswordGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            int amountSymbols = (int)AmountSymbolsSlider.Value;
            bool isRemoveCapitalLetters = (bool)IsRemoveCapitalLettersCheckBox.IsChecked;
            bool isRemoveSigns = (bool)IsRemoveSignsCheckBox.IsChecked;
            PasswordField.Text = Generator.GeneratePassword(amountSymbols, !isRemoveCapitalLetters, !isRemoveSigns);
        }

        private void AmountSymbolsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AmountSymbolsField.Content = $"Количество символов: {AmountSymbolsSlider.Value}";
        }
    }
}
