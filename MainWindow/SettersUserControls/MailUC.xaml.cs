using System;
using System.Windows;
using System.Windows.Controls;

namespace MainWindow.SettersUserControls
{
    /// <summary>
    /// Interaction logic for MailUC.xaml
    /// </summary>
    public partial class MailUC : UserControl
    {
        public event Action<string> GetValue;
        public MailUC(string value)
        {
            InitializeComponent();
            ValueDisplayer.Text = value;
        }

        private void ChangeValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsMailValid())
            {
                GetValue?.Invoke(ValueDisplayer.Text);
            }
            else
            {
                if(MessageBox.Show("Почта введена не корректно!\nЗакрыть поле ввода?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    GetValue.Invoke("");
                }
            }
        }
        private bool IsMailValid()
        {
            System.Text.RegularExpressions.Regex rEMail = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (ValueDisplayer.Text.Length > 0)
            {
                if (rEMail.IsMatch(ValueDisplayer.Text))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
