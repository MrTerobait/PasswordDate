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
    /// Interaction logic for IsRemoveCapitalLettersUC.xaml
    /// </summary>
    public partial class IsRemoveCapitalLettersUC : UserControl
    {
        public event Action<bool> GetValue;
        public IsRemoveCapitalLettersUC()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            GetValue?.Invoke(true);
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            GetValue?.Invoke(false);
        }
    }
}
