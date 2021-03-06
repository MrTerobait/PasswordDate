using System;
using System.Threading.Tasks;
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
            OpenNewRecordingEditor();
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
            recordingList.Add(recording);
        }
        private void OpenNewRecordingEditor()
        {
            NameInput.Text = "Название записи";
            PasswordInput.Text = "Пароль";
            var animation = new DoubleAnimation(5, 80, TimeSpan.FromSeconds(0.5));
            NewRecordingEditor.BeginAnimation(HeightProperty, animation);
        }
        private void CloseNewRecordingEditor(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(80, 5, TimeSpan.FromSeconds(0.5));
            NewRecordingEditor.BeginAnimation(HeightProperty, animation);
        }
        private void GeneratorPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordInput.Text = Tools.GeneratePassword(10, true, true);
        }
    }
}
