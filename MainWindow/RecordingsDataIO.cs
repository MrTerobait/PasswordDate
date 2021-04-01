using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
namespace MainWindow
{
    public class RecordingsDataIO
    {
        private readonly string _pathToRecordingList;
        private readonly string _pathToBasket;
        public RecordingsDataIO(string pathToRecordingList, string pathToBasket)
        {
            _pathToRecordingList = pathToRecordingList;
            _pathToBasket = pathToBasket;
        }
        public BindingList<Recording> LoadDataForRecordingList()
        {
            using (var reader = File.OpenText(_pathToRecordingList))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<BindingList<Recording>>(fileText);
            }
        }
        public BindingList<Recording> LoadDataForBasket()
        {
            using (var reader = File.OpenText(_pathToBasket))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<BindingList<Recording>>(fileText);
            }
        }
        public void SaveData(BindingList<Recording> recordingList, BindingList<Recording> basket)
        {
            using (StreamWriter writer = File.CreateText(_pathToRecordingList))
            {
                string output = JsonConvert.SerializeObject(recordingList);
                writer.Write(output);
            }
            using (StreamWriter writer = File.CreateText(_pathToBasket))
            {
                string output = JsonConvert.SerializeObject(basket);
                writer.Write(output);
            }
        }
        public void CreateRecordingListFileInTXT(string path)
        {
            var data = LoadDataForRecordingList();
            var content = "";
            for (int i = 0; i < data.Count; i++)
            {
                content += $"{i + 1}.";
                content += "     ";
                content += data[i].CreationDate;
                content += "     ";
                content += data[i].Name;
                content += "     ";
                content += data[i].Password;
                content += "\n";
                content += "\n";
            }
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(content);
            }
        }
        public bool IsFilesExist()
        {
            if (File.Exists(_pathToBasket) && File.Exists(_pathToRecordingList))
            {
                return true;
            }
            return false;
        }
        public void SetFiles()
        {
            using (var writer = File.CreateText(_pathToRecordingList))
            {
                writer.Write("[]");
            }
            using (var writer = File.CreateText(_pathToBasket))
            {
                writer.Write("[]");
            }
        }
    }
}
