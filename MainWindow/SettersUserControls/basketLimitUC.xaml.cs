using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainWindow.SettersUserControls
{
    /// <summary>
    /// Interaction logic for BasketLimitUC.xaml
    /// </summary>
    public partial class BasketLimitUC : UserControl
    {
        public event Action<uint> GetValue;
        public BasketLimitUC(string value)
        {
            InitializeComponent();
            ValueDisplayer.Text = value;
        }
        public void ValueDisplayer_TextChanged(object sender, TextChangedEventArgs e)
        {
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
                string str = "";
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
