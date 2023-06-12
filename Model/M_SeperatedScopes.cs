using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public record M_SeperatedScopes
    {
        public M_SeperatedScopes() 
        {
            EventsDictionary = new Dictionary<string, M_UniqueIds>();
            AlarmsDictionary = new Dictionary<string, M_UniqueIds>();
            VariableDictionary = new Dictionary<string, M_UniqueIds>();
        }
        public Dictionary<string, M_UniqueIds> EventsDictionary;
        public Dictionary<string, M_UniqueIds> AlarmsDictionary;
        public Dictionary<string, M_UniqueIds> VariableDictionary; 
   
    }
}
