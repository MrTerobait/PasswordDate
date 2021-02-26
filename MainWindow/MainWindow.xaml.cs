using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Model;

namespace MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ButtonConditions
    {
        Add,
        Save
    }
    public partial class DataWindow : Window
    {
        private BindingList<Recording> recordingList = new BindingList<Recording>();
        private BindingList<Recording> deletedRecorgingHistory = new BindingList<Recording>();
        private ButtonConditions mainButtonCondition = ButtonConditions.Add;
        public DataWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RecordingDisplayer.ItemsSource = recordingList;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainButtonCondition == ButtonConditions.Add)
            {
                NewRecordingEditor.Visibility = Visibility.Visible;
                recordingList.Add(new Recording("",""));
                MainButton.Content = "Сохранить";
                mainButtonCondition = ButtonConditions.Save;
            }
            else if (mainButtonCondition == ButtonConditions.Save)
            {
                NewRecordingEditor.Visibility = Visibility.Collapsed;
                MainButton.Content = "Добавить";
                mainButtonCondition = ButtonConditions.Add;
            }
        }

    }
}
