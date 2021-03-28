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
    /// Interaction logic for daysBeforeRecordingAgingUC.xaml
    /// </summary>
    public partial class daysBeforeRecordingAgingUC : UserControl
    {
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
                int value = int.Parse(ValueDisplayer.Text);
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
    }
}
