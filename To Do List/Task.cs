using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_List
{
    public class Task
    {
        private string title;
        private string message;

        public Task(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }

        public string Title { get => title; set => title = value; }
        public string Message { get => message; set => message = value; }
    }
}
