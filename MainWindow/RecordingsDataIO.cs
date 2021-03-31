using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var recordingListFile = File.Exists(_pathToRecordingList);
            if (!recordingListFile)
            {
                using (var writer = File.CreateText(_pathToRecordingList))
                {
                    writer.Write("[]");
                }
                return new BindingList<Recording>();
            }
            using (var reader = File.OpenText(_pathToRecordingList))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<BindingList<Recording>>(fileText);
            }
        }
        public BindingList<Recording> LoadDataForBasket()
        {
            var recordingListFile = File.Exists(_pathToBasket);
            if (!recordingListFile)
            {
                using (var writer = File.CreateText(_pathToRecordingList))
                {
                    writer.Write("[]");
                }
                return new BindingList<Recording>();
            }
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
    }
}
