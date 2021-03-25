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
    public class FileIOServices
    {
        private readonly string _pathToRecordingList;
        private readonly string _pathToBasket;
        public FileIOServices(string pathToRecordingList, string pathToBasket)
        {
            _pathToRecordingList = pathToRecordingList;
            _pathToBasket = pathToBasket;
        }
        public BindingList<Recording> LoadDataForRecordingList()
        {
            var recordingListFile = File.Exists(_pathToRecordingList);
            if (!recordingListFile)
            {
                File.CreateText(_pathToRecordingList).Dispose();
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
                File.CreateText(_pathToBasket).Dispose();
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
    }
}
