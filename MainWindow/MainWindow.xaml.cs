using System;
using MainWindow.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Model;

namespace MainWindow
{
    enum ButtonCondition
    {
        Opened,
        Closure
    }
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
            ResetNewRecordingEditorFields();
            OpenNewRecordingEditor();
        }
        private void OpenNewRecordingEditor()
        {
            var animation = new DoubleAnimation(5, 80, TimeSpan.FromSeconds(0.5));
            NewRecordingEditor.BeginAnimation(HeightProperty, animation);
        }
        private void CloseNewRecordingEditor(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation(80, 5, TimeSpan.FromSeconds(0.5));
            NewRecordingEditor.BeginAnimation(HeightProperty, animation);
        }
        private void ResetNewRecordingEditorFields()
        {
            NameField.Text = "Название записи";
            PasswordField.Text = "Пароль";
        }
        private void CreateNewRecording(object sender, RoutedEventArgs e)
        {
            Recording recording = new Recording(NameField.Text, PasswordField.Text);
            for (int i = 0; i < recordingList.Count; i++)
            {
                if (recordingList[i].Name == recording.Name)
                {
                    MessageBox.Show("Запись с таким же названием уже существует");
                    return;
                }
            }
            recordingList.Add(recording);
            ResetNewRecordingEditorFields();
        }



        private PasswordGenerator passwordGenerator;
        private ButtonCondition PasswordGeneratorButtonCondition = ButtonCondition.Closure;

        private void PasswordGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordGeneratorButtonCondition == ButtonCondition.Closure)
            {
                passwordGenerator = new PasswordGenerator();
                passwordGenerator.Closed += PasswordGenerator_Closed;
                passwordGenerator.Show();
                PasswordGeneratorButton.Content = "Закрыть генератор пароля";
            }
            else if (PasswordGeneratorButtonCondition == ButtonCondition.Opened)
            {
                passwordGenerator.Closed -= PasswordGenerator_Closed;
                passwordGenerator.Close();
                PasswordGeneratorButton.Content = "Генератор пароля";
            }
            ChangeButtonCondition(ref PasswordGeneratorButtonCondition);
        }
        private void PasswordGenerator_Closed(object sender, EventArgs e)
        {
            PasswordGeneratorButton.Content = "Генератор пароля";
            ChangeButtonCondition(ref PasswordGeneratorButtonCondition);
        }

        private void ChangeButtonCondition(ref ButtonCondition button)
        {
            switch (button)
            {
                case ButtonCondition.Opened:
                    button = ButtonCondition.Closure;
                    return;
                case ButtonCondition.Closure:
                    button = ButtonCondition.Opened;
                    return;
            }
        }
    }
}
