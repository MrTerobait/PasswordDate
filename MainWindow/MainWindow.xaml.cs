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
    enum ButtonConditions
    {
        Opening,
        Closing
    }
    enum CurrentDisplayingListTypes
    {
        RecordingList,
        Basket
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        private readonly string recordingListPath = $"{Environment.CurrentDirectory}\\recordingList.json";
        private readonly string basketPath = $"{Environment.CurrentDirectory}\\basket.json";
        private FileIOServices fileIOServices;
        private BindingList<Recording> recordingList = new BindingList<Recording>();
        private BindingList<Recording> basket = new BindingList<Recording>();
        private RecordingEditor recordingEditor;
        private bool IsRecordingChosen = false;
        CurrentDisplayingListTypes currentDisplayingList = CurrentDisplayingListTypes.RecordingList;

        public DataWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileIOServices = new FileIOServices(recordingListPath, basketPath);
            try
            {
                basket = fileIOServices.LoadDataForBasket();
                recordingList = fileIOServices.LoadDataForRecordingList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
            RecordingsDisplayer.ItemsSource = recordingList;
            SetRecordingButtons();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            passwordGenerator?.Close();
            SaveRecordingsInFile();
        }
        private void SaveRecordingsInFile()
        {
            try
            {
                fileIOServices.SaveData(recordingList, basket);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            switch (currentDisplayingList)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    OpenNewRecordingEditor();
                    break;
                case CurrentDisplayingListTypes.Basket:
                    if (MessageBox.Show("Вы хотите очистить корзину?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        basket.Clear();
                        SaveRecordingsInFile();
                        ToggleCurrentDisplayingList(null, null);
                    }
                    break;
            }
        }

        private void ToggleCurrentDisplayingList(object sender, RoutedEventArgs e)
        {
            switch (currentDisplayingList)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    if (IsBasketEmpty()) { return; }
                    BasketButton.Content = "Закрыть корзину";
                    RecordingsDisplayer.ItemsSource = basket;
                    MainButtonDisplayer.Content = "Очистить корзину";
                    currentDisplayingList = CurrentDisplayingListTypes.Basket;
                    break;
                case CurrentDisplayingListTypes.Basket:
                    BasketButton.Content = "Корзина";
                    MainButtonDisplayer.Content = "Создать запись";
                    RecordingsDisplayer.ItemsSource = recordingList;
                    currentDisplayingList = CurrentDisplayingListTypes.RecordingList;
                    break;
            }
            if (IsRecordingEditorDisplayerOpen)
            {
                CloseRecordingEditorDisplayer();
            }
            IsRecordingChosen = false;
            SetRecordingButtons();
        }
        private bool IsBasketEmpty()
        {
            if (basket.Count == 0)
            {
                MessageBox.Show("На данный момент корзина пуста");
                return true;
            }
            return false;
        }

        private void SetRecordingButtons()
        {
            RecordingButtonDisplayer.Children.Clear();
            int buttonsAmount = 0;
            switch (currentDisplayingList)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    buttonsAmount = recordingList.Count;
                    break;
                case CurrentDisplayingListTypes.Basket:
                    buttonsAmount = basket.Count;
                    break;
            }
            RecordingButtonDisplayer.Children.Add(Indent);
            for (int i = 0; i < buttonsAmount; i++)
            {
                var recordingButton = AddRecordingButton();
                recordingButton.Click += RecordingButton_Click;
                RecordingButtonDisplayer.Children.Add(recordingButton);
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
            return recordingButton;
        }
        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedRecordingButton = sender as Button;
            var recordingNumber = RecordingButtonDisplayer.Children.IndexOf(clickedRecordingButton) - 1;
            if (IsRecordingChosen)
            {
                if (clickedRecordingButton.Foreground == Brushes.Red)
                {
                    CloseChosenRecording();
                    clickedRecordingButton.Foreground = Brushes.White;
                    IsRecordingChosen = false;
                }
                else
                {
                    ChangeChosenRecording(recordingNumber);
                    for (int i = 1; i < RecordingButtonDisplayer.Children.Count; i++)
                    {
                        var recordingButton = RecordingButtonDisplayer.Children[i] as Button;
                        if (recordingButton.Foreground == Brushes.Red)
                        {
                            recordingButton.Foreground = Brushes.White;
                            break;
                        }
                    }
                    clickedRecordingButton.Foreground = Brushes.Red;
                }
            }
            else
            {
                OpenChosenRecording(recordingNumber);
                clickedRecordingButton.Foreground = Brushes.Red;
                IsRecordingChosen = true;
            }                   
        }
        private void ClosureChosenRecordingEditorButton_Click()
        {
            int recordingNumber = 0;
            for (int i = 1; i < RecordingButtonDisplayer.Children.Count; i++)
            {
                var recordingButton = RecordingButtonDisplayer.Children[i] as Button;
                if (recordingButton.Foreground == Brushes.Red)
                {
                    recordingButton.Foreground = Brushes.White;
                    IsRecordingChosen = false;
                    recordingNumber = i - 1;
                    break;
                }
            }
            ChangeConditionRecordingLists(recordingNumber);
            CloseChosenRecording();
        }
        private void CloseChosenRecording()
        {
            recordingEditor = null;
            if (IsRecordingEditorDisplayerOpen) { CloseRecordingEditorDisplayer(); }
        }
        private void ChangeChosenRecording(int recordingNumber)
        {
            switch (currentDisplayingList)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    recordingEditor = new ChosenRecordingInRecordingListEditor(recordingList[recordingNumber]);
                    break;
                case CurrentDisplayingListTypes.Basket:
                    var recording = basket[recordingNumber];
                    recordingEditor = new ChosenRecordingInBasketEditor(ref recording);
                    break;
            }
            recordingEditor.IsEndWork += ClosureChosenRecordingEditorButton_Click;
            RecordingEditorDisplayer.Child = recordingEditor.body;
            if (!IsRecordingEditorDisplayerOpen) { OpenRecordingEditorDisplayer(); }
        }
        private void OpenChosenRecording(int recordingNumber)
        {
            switch (currentDisplayingList)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    recordingEditor = new ChosenRecordingInRecordingListEditor(recordingList[recordingNumber]);
                    break;
                case CurrentDisplayingListTypes.Basket:
                    var recording = basket[recordingNumber];
                    recordingEditor = new ChosenRecordingInBasketEditor(ref recording);
                    break;
            }
            recordingEditor.IsEndWork += ClosureChosenRecordingEditorButton_Click;
            RecordingEditorDisplayer.Child = recordingEditor.body;
            if (!IsRecordingEditorDisplayerOpen) { OpenRecordingEditorDisplayer(); }
        }
        private void ChangeConditionRecordingLists(int recordingNumber)
        {
            switch (currentDisplayingList)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    WriteOffChangesFromCRIRLEditor(recordingNumber);
                    break;
                case CurrentDisplayingListTypes.Basket:
                    WriteOffChangesFromCRIBEditor(recordingNumber);
                    break;
            }
        }
        private void WriteOffChangesFromCRIRLEditor(int recordingNumber)
        {
            var editorCondition = recordingEditor as ChosenRecordingInRecordingListEditor;
            var currentRecording = recordingList[recordingNumber];
            if (editorCondition.Recording == null)
            {
                var index = IsRecordingInCurrentRecordingList(currentRecording.Name, CurrentDisplayingListTypes.Basket);
                if (index == -1)
                {
                    basket.Add(currentRecording);
                }
                else
                {
                    basket[index] = currentRecording;
                }
                RecordingButtonDisplayer.Children.RemoveAt(recordingNumber+1);
                recordingList.RemoveAt(recordingNumber);
                SaveRecordingsInFile();
            }
            else if(currentRecording.Password != editorCondition.Recording.Password)
            {
                var index = IsRecordingInCurrentRecordingList(currentRecording.Name, CurrentDisplayingListTypes.Basket);
                if (index == -1)
                {
                    basket.Add(currentRecording);
                }
                else
                {
                    basket[index] = currentRecording;
                }
                recordingList.RemoveAt(recordingNumber);
                recordingList.Add(editorCondition.Recording);
                SaveRecordingsInFile();
            }
        }
        private void WriteOffChangesFromCRIBEditor(int recordingNumber)
        {
            var editorCondition = recordingEditor as ChosenRecordingInBasketEditor;
            if (editorCondition.ChosenRecording == null)
            {
                basket.RemoveAt(recordingNumber);
                RecordingButtonDisplayer.Children.RemoveAt(recordingNumber + 1);
                SaveRecordingsInFile();
            }
            else if(editorCondition.IsRestoreRecordingButton == true)
            {
                var index = IsRecordingInCurrentRecordingList(editorCondition.ChosenRecording.Name, CurrentDisplayingListTypes.RecordingList);
                if (index == -1)
                {
                    basket.RemoveAt(recordingNumber);
                    RecordingButtonDisplayer.Children.RemoveAt(recordingNumber + 1);
                    InsertRecordingInRecordingList(editorCondition.ChosenRecording);
                    SaveRecordingsInFile();
                }
                else
                {
                    if(MessageBox.Show("Запись с таким названием уже существует!\nЗаменить запись из корзины на текущую?\n(Текущая запись удалится на всегда!)"
                        , $"{editorCondition.ChosenRecording.Name}", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        basket.RemoveAt(recordingNumber);
                        RecordingButtonDisplayer.Children.RemoveAt(recordingNumber + 1);
                        recordingList.RemoveAt(index);
                        InsertRecordingInRecordingList(editorCondition.ChosenRecording);
                        SaveRecordingsInFile();
                    }
                }
            }
            if (basket.Count == 0)
            {
                ToggleCurrentDisplayingList(null,null);
            }
        }
        private void InsertRecordingInRecordingList(Recording recording)
        {
            for (int i = 0; i < recordingList.Count; i++)
            {
                if (DateTime.Compare(recording.CreationDate, recordingList[i].CreationDate) == -1 || DateTime.Compare(recording.CreationDate, recordingList[i].CreationDate) == 0)
                {
                    recordingList.Insert(i, recording);
                    return;
                }
            }
            recordingList.Add(recording);
        }

        private void OpenNewRecordingEditor()
        {
            recordingEditor = new NewRecordingEditor();
            recordingEditor.IsEndWork += CloseNewRecordingEditor;
            var re = (NewRecordingEditor)recordingEditor;
            re.RecordingIsReady += AddNewRecording;
            RecordingEditorDisplayer.Child = recordingEditor.body;
            if (!IsRecordingEditorDisplayerOpen) { OpenRecordingEditorDisplayer(); }
        }
        private void AddNewRecording(Recording newRecording)
        {
            if (IsRecordingInCurrentRecordingList(newRecording.Name, CurrentDisplayingListTypes.RecordingList) != -1)
            {
                MessageBox.Show("Запись с таким же названием уже существует");
                return;
            }
            recordingList.Add(newRecording);
            SaveRecordingsInFile();
            var button = AddRecordingButton();
            button.Click += RecordingButton_Click;
            RecordingButtonDisplayer.Children.Add(button);
        }
        private void CloseNewRecordingEditor()
        {
            recordingEditor = null;
            if (IsRecordingEditorDisplayerOpen) { CloseRecordingEditorDisplayer(); }
        }

        private int IsRecordingInCurrentRecordingList(string name, CurrentDisplayingListTypes listType)
        {
            switch (listType)
            {
                case CurrentDisplayingListTypes.RecordingList:
                    for (int i = 0; i < recordingList.Count; i++)
                    {
                        if (name == recordingList[i].Name)
                        {
                            return i;
                        }
                    }
                    break;
                case CurrentDisplayingListTypes.Basket:
                    for (int i = 0; i < basket.Count; i++)
                    {
                        if (name == basket[i].Name)
                        {
                            return i;
                        }
                    }
                    break;
            }
            return -1;
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            //if (BasketButtonCondition == ButtonConditions.Opening)
            //{
            //    if (IsBasketEmpty()) { return; }
            //    RecordingsDisplayer.ItemsSource = basket;
            //    BasketButton.Content = "Закрыть корзину";
            //    MainButtonDisplayer.Content = "Очистить корзину";
            //}
            //else if (BasketButtonCondition == ButtonConditions.Closing)
            //{
            //    RecordingsDisplayer.ItemsSource = recordingList;
            //    BasketButton.Content = "Корзина";
            //    MainButtonDisplayer.Content = "Создать запись";
            //}
            //ChangeButtonCondition(ref BasketButtonCondition);
            //if (IsRecordingEditorDisplayerOpen)
            //{
            //    CloseRecordingEditorDisplayer();
            //}
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
            private TextBox NameField;
            private TextBox PasswordField;
            public event Action<Recording> RecordingIsReady;

            public NewRecordingEditor()
            {
                NameField = AddTextBox("Название записи");
                PasswordField = AddTextBox("Пароль");
                AddButton("Сохранить", AddRecording_Click, 150);
                AddButton("Закрыть", CloseEditor_Click, 150);
            }
            private void AddRecording_Click(object sender, RoutedEventArgs e)
            {
                Recording recording = new Recording(NameField.Text, PasswordField.Text);
                RecordingIsReady.Invoke(recording);
                NameField.Text = "Название записи";
                PasswordField.Text = "Пароль";
            }
            private void CloseEditor_Click(object sender, RoutedEventArgs e)
            {
                Close();
            }
        }
        private class ChosenRecordingInRecordingListEditor : RecordingEditor
        {
            private Recording _chosenRecording;
            private TextBox PasswordField;
            public Recording Recording { get { return _chosenRecording; } }
            public ChosenRecordingInRecordingListEditor(Recording chosenRecording)
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
                    var name = _chosenRecording.Name;
                    _chosenRecording = null;
                    _chosenRecording = new Recording(name, PasswordField.Text);
                    Close();
                }
            }
            private void ChangeAndCopyRecording_Click(object sender, RoutedEventArgs e)
            {
                if (!AreChosenRecordingAndPasswordFieldPasswordSame())
                {
                    Clipboard.SetText(PasswordField.Text);
                    var name = _chosenRecording.Name;
                    _chosenRecording = null;
                    _chosenRecording = new Recording(name, PasswordField.Text);
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
        private class ChosenRecordingInBasketEditor : RecordingEditor
        {
            private Recording _chosenRecording;
            private bool isRestoreRecording = false;
            private TextBox PasswordField;
            public Recording ChosenRecording { get { return _chosenRecording; } }
            public bool IsRestoreRecordingButton { get { return isRestoreRecording; } }
            public ChosenRecordingInBasketEditor(ref Recording chosenRecording)
            {
                _chosenRecording = chosenRecording;
                PasswordField = AddTextBox(chosenRecording.Password);
                PasswordField.IsReadOnly = true;
                AddButton("Cкопировать", CopyRecording_Click, 150);
                AddButton("Востановить",RestoreDeletedRecording_Click, 150);
                AddButton("Удалить на всегда", DeleteDeletedRecording_Click, 300);
                AddButton("Закрыть", CloseEditor_Click, 150);
            }
            private void RestoreDeletedRecording_Click(object sender, RoutedEventArgs e)
            {
                isRestoreRecording = true;
                Close();
            }
            private void DeleteDeletedRecording_Click(object sender, RoutedEventArgs e)
            {
                _chosenRecording = null;
                Close();
            }
            private void CopyRecording_Click(object sender, RoutedEventArgs e)
            {
                Clipboard.SetText(PasswordField.Text);
            }
            private void CloseEditor_Click(object sender, RoutedEventArgs e)
            {
                Close();
            }
        }
        public abstract class RecordingEditor
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
        private ButtonConditions PasswordGeneratorButtonCondition = ButtonConditions.Opening;
        private void PasswordGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordGeneratorButtonCondition == ButtonConditions.Opening)
            {
                passwordGenerator = new PasswordGenerator();
                passwordGenerator.Closed += PasswordGenerator_Closed;
                passwordGenerator.Show();
                PasswordGeneratorButton.Content = "Закрыть генератор пароля";
            }
            else if (PasswordGeneratorButtonCondition == ButtonConditions.Closing)
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


        private ButtonConditions SettersButtonCondition = ButtonConditions.Opening;
        private void SettersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeButtonCondition(ref ButtonConditions button)
        {
            switch (button)
            {
                case ButtonConditions.Opening:
                    button = ButtonConditions.Closing;
                    return;
                case ButtonConditions.Closing:
                    button = ButtonConditions.Opening;
                    return;
            }
        }
    }
}
