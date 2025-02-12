using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UnitModel:BaseModel
    {
        public string Name { get; set; }
        public List<ResourcesModel> Resources { get; set; } = new();

        public int SubjectId { get; set; }  
        public SubjectModel Subject { get; set; }   
    }
}
