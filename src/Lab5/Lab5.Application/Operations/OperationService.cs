using Abstractions.Repositories;
using Contracts.Operations;
using Contracts.Results;
using Models.Operations;
using Models.Results;

#pragma warning disable CA2007

namespace Application.Operations;

public class OperationService : IOperationService
{
    private readonly IOperationRepository _operationRepository;

    public OperationService(IOperationRepository operationRepository)
    {
        ArgumentNullException.ThrowIfNull(operationRepository);
        _operationRepository = operationRepository;
    }

    public async Task<Result<IEnumerable<Operation>>> GetAccountOperations(long accountId)
    {
        IEnumerable<Operation> operations = await _operationRepository.GetAccountOperations(accountId);
        return new Result<IEnumerable<Operation>>(ResultType.Success, operations);
    }
}