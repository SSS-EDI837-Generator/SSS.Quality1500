namespace SSS.Quality1500.Data.Services;

using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Globalization;
using System.Text;

/// <summary>
/// Writes a structured processing report to disk after each execution.
/// </summary>
public class ProcessingReportWriter : IProcessingReportWriter
{
    private const string Separator = "================================================================================";
    private const string LogsFolder = "logs";

    public Result<string, string> WriteReport(ClaimProcessingResult result)
    {
        try
        {
            string logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogsFolder);
            Directory.CreateDirectory(logsDir);

            string sourceFileName = Path.GetFileNameWithoutExtension(result.SourceFilePath);
            string timestamp = result.ProcessedAt.ToLocalTime().ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string fileName = $"processing_{sourceFileName}_{timestamp}.txt";
            string filePath = Path.Combine(logsDir, fileName);

            using StreamWriter writer = new(filePath, false, Encoding.UTF8);
            WriteHeader(writer, result);
            WriteSummary(writer, result);

            if (result.RecordsWithErrors > 0)
                WriteErrorDetails(writer, result);
            else
                WriteNoErrors(writer);

            WriteFooter(writer);

            return Result<string, string>.Ok(filePath);
        }
        catch (Exception ex)
        {
            return Result<string, string>.Fail($"Error al escribir reporte: {ex.Message}");
        }
    }

    private static void WriteHeader(StreamWriter writer, ClaimProcessingResult result)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("REPORTE DE PROCESAMIENTO - SSS.Quality1500");
        writer.WriteLine(Separator);
        writer.WriteLine($"Archivo:          {Path.GetFileName(result.SourceFilePath)}");
        writer.WriteLine($"Carpeta Imagenes: {result.ImagesFolderPath}");
        writer.WriteLine($"Fecha:            {result.ProcessedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
        writer.WriteLine(Separator);
        writer.WriteLine();
    }

    private static void WriteSummary(StreamWriter writer, ClaimProcessingResult result)
    {
        writer.WriteLine("RESUMEN");
        writer.WriteLine("-------");
        writer.WriteLine($"Total registros:        {result.TotalRecords}");
        writer.WriteLine($"Registros validos:      {result.ValidRecords}");
        writer.WriteLine($"Registros con errores:  {result.RecordsWithErrors}");
        writer.WriteLine($"Total errores de campo: {result.TotalFieldErrors}");
        writer.WriteLine($"Tasa de exito:          {result.SuccessRate.ToString("F2", CultureInfo.InvariantCulture)}%");
        writer.WriteLine();
    }

    private static void WriteErrorDetails(StreamWriter writer, ClaimProcessingResult result)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("DETALLE DE ERRORES");
        writer.WriteLine(Separator);
        writer.WriteLine();

        foreach (RecordValidationResult record in result.ErrorRecords)
        {
            string imageInfo = string.IsNullOrEmpty(record.ImageFileName)
                ? ""
                : $" | Imagen: {record.ImageFileName}";

            writer.WriteLine($"--- Registro #{record.RecordIndex + 1}{imageInfo} ---");

            foreach (FieldValidationError error in record.FieldErrors)
            {
                string valueStr = error.CurrentValue?.ToString() ?? "(vacio)";
                writer.WriteLine($"  [{error.ColumnName}] {error.DisplayName}: \"{valueStr}\" -> {error.ErrorMessage}");
            }

            writer.WriteLine();
        }
    }

    private static void WriteNoErrors(StreamWriter writer)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("SIN ERRORES - Todos los registros son validos.");
        writer.WriteLine(Separator);
        writer.WriteLine();
    }

    private static void WriteFooter(StreamWriter writer)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("FIN DEL REPORTE");
        writer.WriteLine(Separator);
    }
}
