using System;
using MainWindow.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Model;
using System.Windows.Controls;
using System.Windows.Media;

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
        private BindingList<Recording> deletedRecorgingList = new BindingList<Recording>();
        private int chosenRecordingNumber = -1;
        private event Action recordingIsChosen;
        private RecordingEditor recordingEditor;
        public DataWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RecordingDisplayer.ItemsSource = recordingList;
            recordingList.ListChanged += ResetRecordingButtons;
        }
        private void SetRecordingButtons()
        {
            RecordingButtonDisplayer.Children.Clear();
            RecordingButtonDisplayer.Children.Add(Indent);
            if(chosenRecordingNumber == -1)
            for (int i = 0; i < recordingList.Count; i++)
            {
                var recordingButton = AddRecordingButton();
                RecordingButtonDisplayer.Children.Add(recordingButton);
            }
        }
        private void ResetRecordingButtons(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType == ListChangedType.ItemAdded)
            {
                AddRecordingButton();
            }
            else if(e.ListChangedType == ListChangedType.ItemDeleted)
            {
                RecordingButtonDisplayer.Children.RemoveAt(e.OldIndex);
            }
            else if(e.ListChangedType == ListChangedType.ItemChanged)
            {
                RecordingButtonDisplayer.Children.RemoveAt(e.NewIndex);
                RecordingButtonDisplayer.Children.Add(AddRecordingButton());
            }
        }
        private Button AddRecordingButton()
        {
            var recordingButton = new Button
            {
                Content = "->",
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Style = Application.Current.FindResource("MaterialDesignToolButton") as Style,
                Foreground = Brushes.White,
                Height = 31
            };
            return recordingButton;
        }      

        private void OpenNewRecordingEditor(object sender, RoutedEventArgs e)
        {
            recordingEditor = new NewRecordingEditor(ref recordingList);
            recordingEditor.IsEndWork += CloseNewRecordingEditor;
            RecordingEditorDisplayer.Child = recordingEditor.body;
            if (!IsRecordingEditorDisplayerOpen) { OpenRecordingEditorDisplayer(); }
        }
        private void CloseNewRecordingEditor()
        {
            recordingEditor.IsEndWork -= CloseNewRecordingEditor;
            recordingEditor = null;
            if (IsRecordingEditorDisplayerOpen) { CloseRecordingEditorDisplayer(); }
        }
        private bool IsRecordingEditorDisplayerOpen = false;
        private void OpenRecordingEditorDisplayer()
        {
            var animation = new DoubleAnimation(5, 80, TimeSpan.FromSeconds(0.5));
            RecordingEditorDisplayer.BeginAnimation(HeightProperty, animation);
            IsRecordingEditorDisplayerOpen = true;
        }
        private void OpenChosenRecordingEditor(object sender, RoutedEventArgs e)
        {

        }
        private void CloseChosenRecordingEditor()
        {

        }
        private void CloseRecordingEditorDisplayer()
        {
            var animation = new DoubleAnimation(80, 5, TimeSpan.FromSeconds(0.5));
            RecordingEditorDisplayer.BeginAnimation(HeightProperty, animation);
            IsRecordingEditorDisplayerOpen = false;
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
        private class NewRecordingEditor : RecordingEditor
        {
            private BindingList<Recording> _recordingList;
            private TextBox NameField;
            private TextBox PasswordField;
            public NewRecordingEditor(ref BindingList<Recording> recordingList) : base()
            {
                _recordingList = recordingList;
                NameField = AddTextBox("Название записи");
                PasswordField = AddTextBox("Пароль");
                AddButton("Сохранить", AddRecording_Click, 150);
                AddButton("Закрыть", CloseEditor_Click, 150);
            }
            private void AddRecording_Click(object sender, RoutedEventArgs e)
            {
                Recording recording = new Recording(NameField.Text, PasswordField.Text);
                for (int i = 0; i < _recordingList.Count; i++)
                {
                    if (_recordingList[i].Name == recording.Name)
                    {
                        MessageBox.Show("Запись с таким же названием уже существует");
                        return;
                    }
                }
                _recordingList.Add(recording);
                NameField.Text = "Название записи";
                PasswordField.Text = "Пароль";
            }
            private void CloseEditor_Click(object sender, RoutedEventArgs e)
            {
                Close();
            }
        }
        private class ChosenRecordingEditor : RecordingEditor
        {

        }
        private abstract class RecordingEditor
        {
            public Grid body = new Grid();
            private int countControls;
            public event Action IsEndWork;

            protected void Close()
            {
                IsEndWork.Invoke();
            }

            protected void AddButton(string content, RoutedEventHandler buttonHandler, double width)
            {
                var button = new Button()
                {
                    Margin = new Thickness(10, 20, 10, 20),
                    Content = content,
                };
                button.Click += buttonHandler;
                Grid.SetColumn(button, countControls++);
                body.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width) });
                body.Children.Add(button);
            }
            protected TextBox AddTextBox(string text)
            {
                var textBox = new TextBox()
                {
                    Text = text,
                    Margin = new Thickness(0, 4, 0, 0),
                    MaxLength = 50,
                    TextAlignment = TextAlignment.Center,
                    Foreground = Brushes.DarkCyan,
                    FontSize = 14,
                    Style = Application.Current.FindResource("MaterialDesignComboBoxEditableTextBox") as Style
                };
                var container = new Border()
                {
                    Margin = new Thickness(10, 21, 10, 20),
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2),
                    Height = 31,
                    Child = textBox
                };
                Grid.SetColumn(container, countControls++);
                body.ColumnDefinitions.Add(new ColumnDefinition());
                body.Children.Add(container);
                return textBox;
            }
        }
    }
}
