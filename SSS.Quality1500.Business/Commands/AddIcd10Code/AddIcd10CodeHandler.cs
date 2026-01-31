namespace SSS.Quality1500.Business.Commands.AddIcd10Code;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Text.RegularExpressions;

public partial class AddIcd10CodeHandler(IIcd10Repository icd10Repository)
    : ICommandHandler<AddIcd10CodeCommand, Result<bool, string>>
{
    private readonly IIcd10Repository _icd10Repository = icd10Repository;

    public Task<Result<bool, string>> HandleAsync(
        AddIcd10CodeCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Task.FromResult(Result<bool, string>.Fail("El codigo ICD-10 es requerido."));

        if (string.IsNullOrWhiteSpace(command.Description))
            return Task.FromResult(Result<bool, string>.Fail("La descripcion es requerida."));

        string code = command.Code.Trim().ToUpperInvariant();

        if (!Icd10CodePattern().IsMatch(code))
            return Task.FromResult(Result<bool, string>.Fail(
                "Formato invalido. El codigo debe tener el patron: letra + 2 digitos + punto opcional + hasta 4 caracteres (ej: A01.0, T86.11)."));

        if (!_icd10Repository.AddCode(code, command.Description.Trim()))
            return Task.FromResult(Result<bool, string>.Fail($"El codigo {code} ya existe en el catalogo."));

        Result<int, string> saveResult = _icd10Repository.SaveChanges();

        if (saveResult.IsFailure)
            return Task.FromResult(Result<bool, string>.Fail(saveResult.GetErrorOrDefault()!));

        return Task.FromResult(Result<bool, string>.Ok(true));
    }

    [GeneratedRegex(@"^[A-Z]\d{2}(\.\d{1,4})?$")]
    private static partial Regex Icd10CodePattern();
}
