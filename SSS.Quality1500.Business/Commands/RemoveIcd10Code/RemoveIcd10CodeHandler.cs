namespace SSS.Quality1500.Business.Commands.RemoveIcd10Code;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

public class RemoveIcd10CodeHandler(IIcd10Repository icd10Repository)
    : ICommandHandler<RemoveIcd10CodeCommand, Result<bool, string>>
{
    private readonly IIcd10Repository _icd10Repository = icd10Repository;

    public Task<Result<bool, string>> HandleAsync(
        RemoveIcd10CodeCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Task.FromResult(Result<bool, string>.Fail("El codigo ICD-10 es requerido."));

        if (!_icd10Repository.RemoveCode(command.Code.Trim()))
            return Task.FromResult(Result<bool, string>.Fail($"El codigo {command.Code} no existe en el catalogo."));

        Result<int, string> saveResult = _icd10Repository.SaveChanges();

        if (saveResult.IsFailure)
            return Task.FromResult(Result<bool, string>.Fail(saveResult.GetErrorOrDefault()!));

        return Task.FromResult(Result<bool, string>.Ok(true));
    }
}
