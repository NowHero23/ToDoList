using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListWpf.Data.Classes
{
    public class UserTask
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActual { get; set; }

        public DateTime NeedTime { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        

    }
}
