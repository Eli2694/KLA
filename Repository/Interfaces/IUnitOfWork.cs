namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUniqueIdsRepository UniqueIds { get; }
        IUserRepository Users { get; }
        IAliasesRepository Aliases { get; }
        void Complete();
    }
}
