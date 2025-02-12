using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ResourcesModel : BaseModel
    {
        public string Name { get; set; }   

        public int UnitId { get; set; } 
        public UnitModel Unit { get; set; } 
    }
}
