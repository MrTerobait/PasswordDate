using System;

namespace Model
{
    class Recording
    {
        public string Name { get; }
        public DateTime CreationDate { get; private set; }
        public string Password { get; private set; }
        public Recording(string name, string password)
        {
            Name = name;
            UpdatePassword(password);
        }
        public void UpdatePassword(string password)
        {
            Password = password;
            CreationDate = DateTime.Now;
        }
    }
}
