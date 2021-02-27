using System;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
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
            DynamicPartOfWindow.Child = NewRecordingEditor;
            //if (mainButtonCondition == ButtonConditions.Add)
            //{
            //    NewRecordingEditor.Visibility = Visibility.Visible;
            //    var animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
            //    NewRecordingEditor.BeginAnimation(OpacityProperty, animation);
            //    MainButton.Content = "Сохранить";
            //    mainButtonCondition = ButtonConditions.Save;
            //}
            //else if (mainButtonCondition == ButtonConditions.Save)
            //{
            //    var animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.5));
            //    NewRecordingEditor.BeginAnimation(OpacityProperty, animation);
            //    MainButton.Content = "Добавить";
            //    mainButtonCondition = ButtonConditions.Add;
            //}
        }

        private void ChangeDisplayer(object firstDisplayer, object secondDisplayer)
        {
        }
    }
}
