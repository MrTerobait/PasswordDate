using System;
using System.Windows;
using System.Windows.Controls;

namespace MainWindow.SettersUserControls
{
    /// <summary>
    /// Interaction logic for daysBeforeRecordingAgingUC.xaml
    /// </summary>
    public partial class daysBeforeRecordingAgingUC : UserControl
    {
        public event Action<uint> GetValue;
        public daysBeforeRecordingAgingUC(string value)
        {
            InitializeComponent();
            ValueDisplayer.Text = value;
        }

        private void ValueDisplayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = "";
            try
            {
                if (ValueDisplayer.Text.Length < 9)
                {
                    uint value = uint.Parse(ValueDisplayer.Text);
                }
                else
                {
                    ValueDisplayer.Text = ValueDisplayer.Text.Substring(0, 9);
                }
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
