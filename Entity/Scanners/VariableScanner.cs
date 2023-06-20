using Entity.EntityInterfaces;
using Model;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_LOG;

namespace Entity.Scanners
{
	public class VariableScanner : BaseScanner, IScanner
	{
		public VariableScanner(LogManager log) : base(log)
		{
		}

        public VariableScanner() : base(null)
        {
        }

        public List<UniqueIds> ScanCode(KlaXML ktgemvar)
		{
			try
			{
				var variableList = new List<UniqueIds>();

				variableList.AddRange(GetVariableList(ktgemvar.DataVariables, "DataVariable"));
				variableList.AddRange(GetVariableList(ktgemvar.EquipmentConstants, "EquipmentConstant"));
				variableList.AddRange(GetVariableList(ktgemvar.DynamicVariables, "DynamicVariable"));
				variableList.AddRange(GetVariableList(ktgemvar.StatusVariables, "StatusVariable"));

				return variableList;
			}
			catch (Exception ex)
			{
				// Log the exception
				_log.LogException("error", ex, LogProviderType.Console);
				throw;
			}
		}

		private List<UniqueIds> GetVariableList<T>(IEnumerable<T> variables, string entityType)
		{
			return variables.Select(variable =>
				new UniqueIds
				{
					EntityType = entityType,
					ID = variable.GetType().GetProperty("Id").GetValue(variable).ToString(),
					Name = variable.GetType().GetProperty("ExternalName").GetValue(variable).ToString(),
					Scope = "variable",
					Timestamp = DateTime.Now
				}).ToList();
		}		
	}
}

