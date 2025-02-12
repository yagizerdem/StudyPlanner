using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NotesModel: BaseModel
    {
        public string Title { get; set; }   
        public string Body { get; set; }    
        public bool Starred {  get; set; }
    }
}
