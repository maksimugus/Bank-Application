using Contracts.Results;

namespace Models.Results;

public record Result<T>(ResultType Type, T Data)
    where T : class;