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
        enum OpeningList
        {
            RecordingList,
            DeletedRecordingList
        }
        private RecordingEditor recordingEditor;
        private int chosenRecordingNumber = -1;
        public DataWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetRecordingButtons();
            RecordingsDisplayer.ItemsSource = recordingList;
            recordingList.ListChanged += ResetRecordingButtons;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            passwordGenerator?.Close();
        }
        private void SetRecordingButtons()
        {
            RecordingButtonDisplayer.Children.Clear();
            RecordingButtonDisplayer.Children.Add(Indent);
            for (int i = 0; i < recordingList.Count; i++)
            {
                var recordingButton = AddRecordingButton();
                RecordingButtonDisplayer.Children.Add(recordingButton);
            }
        }
        private void ResetRecordingButtons(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                RecordingButtonDisplayer.Children.Add(AddRecordingButton());
            }
            else if(e.ListChangedType == ListChangedType.ItemDeleted)
            {
                RecordingButtonDisplayer.Children.RemoveAt(e.NewIndex+1); //zero element is indent
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
                Height = 35
            };
            recordingButton.Click += OpenChosenRecordingEditor;
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
            if (IsRecordingEditorDisplayerOpen) { CloseRecordingEditorDisplayer(); }
            recordingEditor.IsEndWork -= CloseNewRecordingEditor;
            recordingEditor = null;
        }

        private void OpenChosenRecordingEditor(object sender, RoutedEventArgs e)
        {
            if(chosenRecordingNumber != -1)
            {               
                RecordingButtonDisplayer.Children[chosenRecordingNumber+1].SetValue(ForegroundProperty, Brushes.White);
            }
            var chosenButton = sender as Button;
            int recordingIndex = RecordingButtonDisplayer.Children.IndexOf(chosenButton)-1; //zero element is indent
            if(recordingIndex == chosenRecordingNumber)
            {
                CloseChosenRecordingEditor();
                IsRecordingEditorDisplayerOpen = false;
                return;
            }
            chosenRecordingNumber = recordingIndex;
            Recording chosenRecording = recordingList[recordingIndex];
            recordingEditor = new ChosenRecordingEditor(ref chosenRecording);
            recordingEditor.IsEndWork += CloseChosenRecordingEditor;
            chosenButton.Foreground = Brushes.Red;
            RecordingEditorDisplayer.Child = recordingEditor.body;
            if (!IsRecordingEditorDisplayerOpen) { OpenRecordingEditorDisplayer(); }
        }
        private void CloseChosenRecordingEditor()
        {
            ChosenRecordingEditor currentEditorState = (ChosenRecordingEditor)recordingEditor;
            if (currentEditorState.ChosenRecording == null)
            {
                DeleteRecordingInRecordingList();
            }
            else
            {
                ChangeRecordingInRecordingList(currentEditorState.ChosenRecording);
            }
            if (IsRecordingEditorDisplayerOpen) { CloseRecordingEditorDisplayer(); }
            chosenRecordingNumber = -1;
            recordingEditor.IsEndWork -= CloseChosenRecordingEditor;
            recordingEditor = null;
        }
        private void ChangeRecordingInRecordingList(Recording recording)
        {
            recordingList.RemoveAt(chosenRecordingNumber);
            recordingList.Add(recording);
        }
        private void DeleteRecordingInRecordingList()
        {
            deletedRecorgingList.Add(recordingList[chosenRecordingNumber]);
            recordingList.RemoveAt(chosenRecordingNumber);
        }
        
        private bool IsRecordingEditorDisplayerOpen = false;
        private void OpenRecordingEditorDisplayer()
        {
            MainWindowDisplayer.IsHitTestVisible = false;
            var animation = new DoubleAnimation(5, 80, TimeSpan.FromSeconds(0.5));
            animation.Completed += UnblockClick;
            RecordingEditorDisplayer.BeginAnimation(HeightProperty, animation);
            IsRecordingEditorDisplayerOpen = true;
        }
        private void CloseRecordingEditorDisplayer()
        {
            MainWindowDisplayer.IsHitTestVisible = false;
            var animation = new DoubleAnimation(80, 5, TimeSpan.FromSeconds(0.5));
            animation.Completed += UnblockClick;
            RecordingEditorDisplayer.BeginAnimation(HeightProperty, animation);
            IsRecordingEditorDisplayerOpen = false;
        }
        private void UnblockClick(object sender, EventArgs e)
        {
            MainWindowDisplayer.IsHitTestVisible = true;
        }

        private class NewRecordingEditor : RecordingEditor
        {
            private Recording _newRecording;
            private BindingList<Recording> _recordingList;
            private TextBox NameField;
            private TextBox PasswordField;
            private Recording NewRecording
            {
                get
                {
                    return _newRecording;
                }
            }
            public NewRecordingEditor(ref BindingList<Recording> recordingList)
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
            private Recording _chosenRecording;
            private TextBox PasswordField;
            public Recording ChosenRecording 
            {
                get
                {
                    return _chosenRecording;
                } 
            }
            public ChosenRecordingEditor(ref Recording chosenRecording)
            {
                _chosenRecording = chosenRecording;
                PasswordField = AddTextBox(chosenRecording.Password);
                AddButton("Скопировать", CopyRecording_Click, 150);
                AddButton("Изменить", ChangeRecording_Click, 150);
                AddButton("Изменить и скопировать", ChangeAndCopyRecording_Click, 300);
                AddButton("Удалить", DeleteRecording_Click, 150);
                AddButton("Закрыть", CloseEditor_Click, 150);
            }
            private void CopyRecording_Click(object sender, RoutedEventArgs e)
            {
                Clipboard.SetText(PasswordField.Text);
            }
            private void ChangeRecording_Click(object sender, RoutedEventArgs e)
            {
                if (!AreChosenRecordingAndPasswordFieldPasswordSame())
                {
                    _chosenRecording.UpdatePassword(PasswordField.Text);
                    Close();
                }
            }
            private void ChangeAndCopyRecording_Click(object sender, RoutedEventArgs e)
            {
                if (!AreChosenRecordingAndPasswordFieldPasswordSame())
                {
                    Clipboard.SetText(PasswordField.Text);
                    _chosenRecording.UpdatePassword(PasswordField.Text);
                    Close();
                }
            }
            private bool AreChosenRecordingAndPasswordFieldPasswordSame()
            {
                if (_chosenRecording.Password == PasswordField.Text)
                {
                    MessageBox.Show("Новый пароль должен отличаться от старого");
                    return true;
                }
                return false;
            }
            private void DeleteRecording_Click(object sender, RoutedEventArgs e)
            {
                _chosenRecording = null;
                Close();
            }
            private void CloseEditor_Click(object sender, RoutedEventArgs e)
            {
                Close();
            }
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
