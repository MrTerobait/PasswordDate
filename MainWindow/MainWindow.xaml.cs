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
    public partial class DataWindow : Window
    {
        private BindingList<Recording> recordingList = new BindingList<Recording>();
        private BindingList<Recording> deletedRecorgingHistory = new BindingList<Recording>();
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
            OpenEditor();
        }

        private void CreateNewRecording(object sender, RoutedEventArgs e)
        {
            Recording recording = new Recording(NameInput.Text, PasswordInput.Text);
            for (int i = 0; i < recordingList.Count; i++)
            {
                if (recordingList[i].Name == recording.Name)
                {
                    MessageBox.Show("Запись с таким же названием уже существует");
                    return;
                }
            }
            CloseEditor();
            recordingList.Add(recording);
        }
        private void OpenEditor()
        {
            var animation = new DoubleAnimation(5, 105, TimeSpan.FromSeconds(0.5));
            Editor.BeginAnimation(HeightProperty, animation);
        }
        private void CloseEditor()
        {
            var animation = new DoubleAnimation(105, 5, TimeSpan.FromSeconds(0.5));
            Editor.BeginAnimation(HeightProperty, animation);
        }

        private void MakeRandomTextInPasswordInput(object sender, RoutedEventArgs e)
        {
            PasswordInput.Text = Tools.GeneratePassword(10, true, true);
        }
    }
}
