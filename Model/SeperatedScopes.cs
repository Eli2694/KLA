using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SeperatedScopes
    {
        public SeperatedScopes() 
        {
            EventsList = new List<UniqueIds>();
            AlarmsList = new List<UniqueIds>();
            VariablesList = new List<UniqueIds>();
        }
        public List<UniqueIds> EventsList;
        public List<UniqueIds> AlarmsList;
        public List<UniqueIds> VariablesList; 
   
    }
}
