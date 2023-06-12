using Entity.EntityInterfaces;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Scanners
{
    public class AlarmScanner : IScanner
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlarmScanner(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public Dictionary<string, M_UniqueIds> ScanCode(M_KlaXML ktgemvar)
        {
            Dictionary<string, M_UniqueIds> alaramsDictionary = new Dictionary<string, M_UniqueIds>();

            foreach (var alarm in ktgemvar.Alarms)
            {
                string ID_KEY = alarm.Id.ToString();
                alaramsDictionary.Add(ID_KEY, new M_UniqueIds { EntityType = "Alarm", ID = ID_KEY, Name = alarm.Name, Scope = "Alarms", Timestamp = DateTime.Now });
            }
            return alaramsDictionary;
        }

        public Dictionary<string, M_UniqueIds> RetriveFromDB()
        {
            throw new NotImplementedException();
        }
    }
}
