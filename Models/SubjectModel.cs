using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SubjectModel : BaseModel
    {
        public string Name {  get; set; }

        public List<UnitModel> Unit { get; set; } = new();
    }
}
