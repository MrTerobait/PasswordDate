using System;
using System.Windows;
using System.Windows.Controls;

namespace MainWindow.SettersUserControls
{
    /// <summary>
    /// Interaction logic for passwordGeneratorDefaultSymbolsAmountUC.xaml
    /// </summary>
    public partial class passwordGeneratorDefaultSymbolsAmountUC : UserControl
    {
        public event Action<uint> GetValue;
        public passwordGeneratorDefaultSymbolsAmountUC(string value)
        {
            InitializeComponent();
            ValueDisplayer.Text = value;
        }

        private void ValueDisplayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = "";
            try
            {
                uint value = uint.Parse(ValueDisplayer.Text);
            }
            catch
            {
                for (int i = 0; i < ValueDisplayer.Text.Length; i++)
                {
                    if (ValueDisplayer.Text[i] >= 48 && ValueDisplayer.Text[i] <= 57)
                    {
                        str += ValueDisplayer.Text[i];
                    }
                }
                ValueDisplayer.Text = str;
            }
            if (ValueDisplayer.Text != "" && uint.Parse(ValueDisplayer.Text) > 50)
            {
                ValueDisplayer.Text = "50";
            }
        }
        private void ChangeValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValueDisplayer.Text.Length == 0)
            {
                GetValue?.Invoke(0);
                return;
            }
            GetValue?.Invoke(uint.Parse(ValueDisplayer.Text));
        }
    }
}
