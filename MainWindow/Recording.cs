using System;

namespace MainWindow
{
    public class Recording
    {
        public string Name { get; }
        public DateTime CreationDate { get; private set; }
        public string Password { get; private set; }
        public Recording(string name, string password, DateTime creationDate)
        {
            Name = name;
            Password = password;
            CreationDate = creationDate;
        }
        public void UpdatePassword(string password)
        {
            Password = password;
            CreationDate = DateTime.Now;
        }
    }
}
