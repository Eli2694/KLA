using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class M_ComparedLists
    {
        public M_ComparedLists()
        {
            matchingIdAndName = new List<UniqueIds>();
            matchingIdAndDifferentName = new List<UniqueIds>();
            differentIdAndMatchingName = new List<UniqueIds>();
            differentIdAndDifferentName = new List<UniqueIds>();
        }
        public List<UniqueIds> matchingIdAndName { get; set; }
        public List<UniqueIds> matchingIdAndDifferentName { get; set; }
        public List<UniqueIds> differentIdAndMatchingName { get; set; }
        public List<UniqueIds> differentIdAndDifferentName { get; set; }
    }
}