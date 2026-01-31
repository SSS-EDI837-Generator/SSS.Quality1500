namespace SSS.Quality1500.Business.Commands.RemoveIcd10Code;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

public record RemoveIcd10CodeCommand(string Code)
    : ICommand<Result<bool, string>>;
