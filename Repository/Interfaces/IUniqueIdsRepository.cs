using Model;

namespace Repository.Interfaces
{
    public interface IUniqueIdsRepository : IRepository<UniqueIds>
    {
        IEnumerable<UniqueIds> GetSpecificScope(string scope);
        IEnumerable<UniqueIds> GetUniqueIdsWithAliases();
    }
}
