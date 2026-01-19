namespace SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Common;


public interface IVdeListBatch {
    Result<bool, string> MyListBatch(BatchProcessingRequest request);
}
