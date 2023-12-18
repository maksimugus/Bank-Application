using Models.Operations;
using Models.Results;

namespace Contracts.Operations;

public interface IOperationService
{
    Task<Result<IEnumerable<Operation>>> GetAccountOperations(long accountId);
}