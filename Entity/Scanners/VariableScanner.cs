using Entity.EntityInterfaces;
using Model;
using Model.XmlModels;
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

		public List<UniqueIds> ScanKtgemContent(Ktgem ktgemvar)
		{
			try
			{
				var variableList = new List<UniqueIds>();

				if (ktgemvar.DataVariables != null)
                {
                    variableList.AddRange(GetVariableList(ktgemvar.DataVariables, "DataVariable"));
                }
				if (ktgemvar.EquipmentConstants != null)
				{
					variableList.AddRange(GetVariableList(ktgemvar.EquipmentConstants, "EquipmentConstant"));
                }
				if (ktgemvar.DynamicVariables != null)
				{
                    variableList.AddRange(GetVariableList(ktgemvar.DynamicVariables, "DynamicVariable"));
                }
				if (ktgemvar.StatusVariables != null)
				{
                    variableList.AddRange(GetVariableList(ktgemvar.StatusVariables, "StatusVariable"));
                }

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

