using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class M_SeperatedScopes
    {
        public M_SeperatedScopes() 
        {
            EventsList = new List<M_UniqueIds>();
            AlarmsList = new List<M_UniqueIds>();
            VariablesList = new List<M_UniqueIds>();
        }
        public List<M_UniqueIds> EventsList;
        public List<M_UniqueIds> AlarmsList;
        public List<M_UniqueIds> VariablesList; 
   
    }
}
