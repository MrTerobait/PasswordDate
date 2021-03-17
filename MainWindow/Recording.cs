using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWindow
{
    public class Recording
    {
        public string Name { get; }
        public string Password { get; private set; }
        public DateTime CreationdDate { get; private set; }
        public Recording(string name, string password)
        {
            Name = password;
            UpdatePassword(password);
        }
        public void UpdatePassword(string password)
        {
            Password = password;
            CreationdDate = DateTime.Now;
        }
    }
}
