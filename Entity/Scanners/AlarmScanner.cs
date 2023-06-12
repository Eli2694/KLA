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
        public Dictionary<string, M_UniqueIds> RetriveFromDB()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, M_UniqueIds> ScanCode()
        {
            throw new NotImplementedException();
        }
    }
}
