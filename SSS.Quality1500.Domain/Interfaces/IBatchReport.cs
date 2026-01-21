namespace SSS.Quality1500.Domain.Interfaces;

using System.Data;
using SSS.Quality1500.Domain.Models;

public interface IBatchReport
{
    Task<Result<bool, string>> Generate(string vkFile, string v1Pagina, DataTable table);

}
