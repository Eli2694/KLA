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
            matchingIdAndName = new List<M_UniqueIds>();
            matchingIdAndDifferentName = new List<M_UniqueIds>();
            differentIdAndMatchingName = new List<M_UniqueIds>();
            differentIdAndDifferentName = new List<M_UniqueIds>();
        }
        public List<M_UniqueIds> matchingIdAndName { get; set; }
        public List<M_UniqueIds> matchingIdAndDifferentName { get; set; }
        public List<M_UniqueIds> differentIdAndMatchingName { get; set; }
        public List<M_UniqueIds> differentIdAndDifferentName { get; set; }
    }
}