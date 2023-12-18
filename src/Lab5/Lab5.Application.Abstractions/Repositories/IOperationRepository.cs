using Models.Operations;

namespace Abstractions.Repositories;

public interface IOperationRepository
{
    Task<IEnumerable<Operation>> GetAccountOperations(long accountId);
    Task AddOperation(long accountId, int balanceChange);
    Task UpdateOperation(Operation operation);
    Task DeleteOperationById(long id);
}