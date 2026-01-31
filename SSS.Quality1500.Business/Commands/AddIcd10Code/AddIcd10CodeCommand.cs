namespace SSS.Quality1500.Business.Commands.AddIcd10Code;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

public record AddIcd10CodeCommand(string Code, string Description)
    : ICommand<Result<bool, string>>;
