using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Net;
using System.Net.Mail;

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
        private Setters setters;
        private RecordingsDataIO fileIOServices;
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
            setters = new Setters();
            fileIOServices = new RecordingsDataIO(recordingListPath, basketPath);
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
            MarkOldRecordingsInRecordingList();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            passwordGenerator?.Close();
            if (recordingList.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("Отправить пароли на почту?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (setters.Mail == "")
                {
                    MessageBox.Show("Почта не указана!\nВам нужно перезайти в программу\nи указать почту в настройках!");
                }
                else
                {
                    MailAddress From = new MailAddress("otpravitelparoley@mail.ru");
                    MailAddress To = new MailAddress(setters.Mail);
                    MailMessage msg = new MailMessage(From, To);
                    msg.Subject = "Список ваших паролей";
                    string attachmentFile = $"{Environment.CurrentDirectory}\\recordingList.txt";
                    fileIOServices.CreateRecordingListFileInTXT(attachmentFile);
                    msg.Attachments.Add(new Attachment(attachmentFile));
                    SmtpClient smtp = new SmtpClient("smpt.mail.ru", 25);
                    smtp.Credentials = new NetworkCredential("otpravitelparoley@mail.ru", "65+C1J0$h!");
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
            }
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
        private void MarkOldRecordingsInRecordingList()
        {
            RecordingsDisplayer.SelectedCells.Clear();
            foreach (var item in RecordingsDisplayer.Items)
            {
                var recording = item as Recording;
                var days = DateTime.Now - recording.CreationDate;
                if (days.Days >= setters.DaysBeforeRecordingAging)
                {
                    RecordingsDisplayer.CurrentCell = new DataGridCellInfo(item, RecordingsDisplayer.Columns[0]);
                    RecordingsDisplayer.SelectedCells.Add(RecordingsDisplayer.CurrentCell);
                }
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
                    CreationDateDisplayer.CellStyle = RecordingsDisplayer.FindResource("Default") as Style;
                    currentDisplayingList = CurrentDisplayingListTypes.Basket;
                    break;
                case CurrentDisplayingListTypes.Basket:
                    BasketButton.Content = "Корзина";
                    MainButtonDisplayer.Content = "Создать запись";
                    RecordingsDisplayer.ItemsSource = recordingList;
                    CreationDateDisplayer.CellStyle = RecordingsDisplayer.FindResource("CreationDate") as Style;
                    MarkOldRecordingsInRecordingList();
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
                    basket.Insert(0, currentRecording);
                    if (basket.Count > setters.BasketLimit)
                    {
                        basket.RemoveAt(basket.Count - 1);
                    }                    
                }
                else
                {
                    basket.RemoveAt(index);
                    basket.Insert(0, currentRecording);
                }
                RecordingButtonDisplayer.Children.RemoveAt(recordingNumber+1);
                recordingList.RemoveAt(recordingNumber);
                SaveRecordingsInFile();
                MarkOldRecordingsInRecordingList();
            }
            else if(currentRecording.Password != editorCondition.Recording.Password)
            {
                var index = IsRecordingInCurrentRecordingList(currentRecording.Name, CurrentDisplayingListTypes.Basket);
                if (index == -1)
                {
                    basket.Insert(0, currentRecording);
                    if (basket.Count > setters.BasketLimit)
                    {
                        basket.RemoveAt(basket.Count - 1);
                    }
                }
                else
                {
                    basket.RemoveAt(index);
                    basket.Insert(0, currentRecording);
                }
                recordingList.RemoveAt(recordingNumber);
                recordingList.Add(editorCondition.Recording);
                SaveRecordingsInFile();
                MarkOldRecordingsInRecordingList();
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
        private void DeleteOutOfBasketLimitRecording()
        {
            while (basket.Count > setters.BasketLimit)
            {
                basket.RemoveAt(basket.Count - 1);
            }
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
                Recording recording = new Recording(NameField.Text, PasswordField.Text, DateTime.Now);
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
                    _chosenRecording = new Recording(name, PasswordField.Text, DateTime.Now);
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
                    _chosenRecording = new Recording(name, PasswordField.Text, DateTime.Now);
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
        private ButtonConditions PasswordGeneratorButtonCondition = ButtonConditions.Opening;
        private void PasswordGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordGeneratorButtonCondition == ButtonConditions.Opening)
            {
                var defaultPasswordGeneratorValues = setters.PasswordGeneratorParams;
                int amountSymbols = (int)defaultPasswordGeneratorValues[0];
                bool isRemoveCapitalLetters = (bool)defaultPasswordGeneratorValues[1];
                bool isRemoveSigns = (bool)defaultPasswordGeneratorValues[2];
                passwordGenerator = new PasswordGenerator(amountSymbols, isRemoveCapitalLetters, isRemoveSigns);
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
            if (SettersButtonCondition == ButtonConditions.Opening)
            {
                if (IsRecordingEditorDisplayerOpen)
                {
                    CloseRecordingEditorDisplayer();
                }
                SetValuesInSettersDisplayer();
                MainWindowDisplayer.IsHitTestVisible = false;
                MainWindowDisplayer.Effect = new BlurEffect();
                SettersDisplayer.Visibility = Visibility.Visible;
                setters.editorSetterIsClosed += CloseSetter;
            }
            else if(SettersButtonCondition == ButtonConditions.Closing)
            {
                MainWindowDisplayer.IsHitTestVisible = true;
                MainWindowDisplayer.Effect = null;
                SettersDisplayer.Visibility = Visibility.Collapsed;
                setters.editorSetterIsClosed -= CloseSetter;
                MarkOldRecordingsInRecordingList();
                DeleteOutOfBasketLimitRecording();
                if (currentDisplayingList == CurrentDisplayingListTypes.Basket)
                {
                    if (basket.Count > 0)
                    {
                        SetRecordingButtons();
                    }
                    else
                    {
                        ToggleCurrentDisplayingList(null, null);
                    }
                }
            }
            SettersManagerDisplayerIsEmpty.Visibility = Visibility.Visible;
            SettersManagerDisplayerIsChosen.Visibility = Visibility.Collapsed;
            ChangeButtonCondition(ref SettersButtonCondition);
        }
        private void SetterButton_MouseOver(object sender, RoutedEventArgs e)
        {
            var setterControl = sender as Label;
            var setter =  DefineSenderFromSetters(setterControl);
            setter[0].Foreground = Brushes.Red;
            setter[1].Foreground = Brushes.Red;
        }
        private void SetterButton_MouseLeave(object sender, RoutedEventArgs e)
        {
            var setterControl = sender as Label;
            var setter = DefineSenderFromSetters(setterControl);
            setter[0].Foreground = Brushes.Black;
            setter[1].Foreground = Brushes.Black;
        }
        private void SetterButton_Click(object sender, RoutedEventArgs e)
        {
            var setterControl = sender as Label;
            var setter = DefineSenderFromSetters(setterControl);
            SettersManagerDisplayerIsChosen.Child = setters.OpenEditorSetter((string)setter[0].Content);
            SettersManagerDisplayerIsChosen.Visibility = Visibility.Visible;
            SettersManagerDisplayerIsEmpty.Visibility = Visibility.Collapsed;
            setters.editorSetterIsClosed += CloseSetter;

        }
        private void ResetSettersButton_Click(object sender, RoutedEventArgs e)
        {
            setters = Setters.ResetSetters();
            SetValuesInSettersDisplayer();
            SettersManagerDisplayerIsChosen.Visibility = Visibility.Collapsed;
            SettersManagerDisplayerIsEmpty.Visibility = Visibility.Visible;
        }
        private void SetValuesInSettersDisplayer()
        {
            var nounType = GetTypeNounToNumber(setters.DaysBeforeRecordingAging);
            if (nounType == "singular")
            {
                daysBeforeRecordingAgingDisplayer1.Content = $"{setters.DaysBeforeRecordingAging} день";
            }
            else if(nounType == "plural")
            {
                daysBeforeRecordingAgingDisplayer1.Content = $"{setters.DaysBeforeRecordingAging} дней";
            }
            else
            {
                daysBeforeRecordingAgingDisplayer1.Content = $"{setters.DaysBeforeRecordingAging} дня";
            }
            nounType = GetTypeNounToNumber(setters.BasketLimit);
            if (nounType == "singular")
            {
                basketLimitDisplayer1.Content = $"{setters.BasketLimit} запись";
            }
            else if (nounType == "plural")
            {
                basketLimitDisplayer1.Content = $"{setters.BasketLimit} записей";
            }
            else
            {
                basketLimitDisplayer1.Content = $"{setters.BasketLimit} записи";
            }
            if (setters.Mail != "")
            {
                mailDisplayer1.Content = "Указана";
            }
            else
            {
                mailDisplayer1.Content = "Не указана";
            }
            passwordGeneratorDefaultSymbolsAmountDisplayer1.Content = setters.PasswordGeneratorParams[0];
            if ((bool)setters.PasswordGeneratorParams[1] == false)
            {
                isRemoveCapitalLettersDisplayer1.Content = "нет";
            }
            else
            {
                isRemoveCapitalLettersDisplayer1.Content = "да";
            }
            if ((bool)setters.PasswordGeneratorParams[2] == false)
            {
                isRemoveSignsDisplayer1.Content = "нет";
            }
            else
            {
                isRemoveSignsDisplayer1.Content = "да";
            }
        }
        private string GetTypeNounToNumber(int number)
        {
            if (number > 9 && number < 20)
            {
                return "plural";
            }
            number = number % 10;
            if (number == 0 || number > 4)
            {
                return "plural";
            }
            else if(number == 1)
            {
                return "singular";
            }
            else
            {
                return "indirect declination";
            }
        }
        private void CloseSetter()
        {
            SettersManagerDisplayerIsChosen.Visibility = Visibility.Collapsed;
            SettersManagerDisplayerIsEmpty.Visibility = Visibility.Visible;
            SetValuesInSettersDisplayer();
        }

        private Label[] DefineSenderFromSetters(Label sender)
        {
            if (sender == daysBeforeRecordingAgingDisplayer0 || sender == daysBeforeRecordingAgingDisplayer1)
            {
                return new Label[] { daysBeforeRecordingAgingDisplayer0, daysBeforeRecordingAgingDisplayer1 };
            }
            else if (sender == basketLimitDisplayer0 || sender == basketLimitDisplayer1)
            {
                return new Label[] { basketLimitDisplayer0, basketLimitDisplayer1 };
            }
            else if (sender == mailDisplayer0 || sender == mailDisplayer1)
            {
                return new Label[] { mailDisplayer0, mailDisplayer1 };
            }
            else if (sender == passwordGeneratorDefaultSymbolsAmountDisplayer0 || sender == passwordGeneratorDefaultSymbolsAmountDisplayer1)
            {
                return new Label[] { passwordGeneratorDefaultSymbolsAmountDisplayer0, passwordGeneratorDefaultSymbolsAmountDisplayer1 };
            }
            else if (sender == isRemoveCapitalLettersDisplayer0 || sender == isRemoveCapitalLettersDisplayer1)
            {
                return new Label[] { isRemoveCapitalLettersDisplayer0, isRemoveCapitalLettersDisplayer1 };
            }
            else if(sender == isRemoveSignsDisplayer0 || sender == isRemoveSignsDisplayer1)
            {
                return new Label[] { isRemoveSignsDisplayer0, isRemoveSignsDisplayer1 };
            }
            throw new Exception();
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
