namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Common;
using System.Data;


public interface IBatchReport
{
    Task<Result<bool, string>> Generate(string vkFile, string v1Pagina, DataTable table);

}
