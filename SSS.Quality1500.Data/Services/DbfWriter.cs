namespace SSS.Quality1500.Data.Services;

using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Text;

/// <summary>
/// Implementation of IDbfWriter using direct byte manipulation.
/// DBF files have a well-defined binary format that allows in-place updates.
/// </summary>
public class DbfWriter : IDbfWriter
{
    private const int HeaderSize = 32;
    private const int FieldDescriptorSize = 32;
    private const byte HeaderTerminator = 0x0D;
    private const byte DeletedRecordFlag = 0x2A; // '*'
    private const byte ValidRecordFlag = 0x20;   // ' '

    static DbfWriter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public Result<bool, string> UpdateRecord(
        string filePath,
        int recordIndex,
        Dictionary<string, object?> fieldValues)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<bool, string>.Fail("La ruta del archivo DBF es requerida.");

        if (!File.Exists(filePath))
            return Result<bool, string>.Fail($"El archivo DBF no existe: {filePath}");

        if (recordIndex < 0)
            return Result<bool, string>.Fail("El índice del registro debe ser mayor o igual a cero.");

        if (fieldValues == null || fieldValues.Count == 0)
            return Result<bool, string>.Fail("Debe proporcionar al menos un campo para actualizar.");

        try
        {
            DbfHeader header = ReadHeader(filePath);

            if (recordIndex >= header.RecordCount)
                return Result<bool, string>.Fail($"Índice de registro {recordIndex} fuera de rango. Total de registros: {header.RecordCount}");

            using FileStream fs = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            foreach (KeyValuePair<string, object?> kvp in fieldValues)
            {
                DbfFieldDescriptor? field = header.Fields.FirstOrDefault(
                    f => f.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));

                if (field == null)
                    continue; // Skip unknown fields silently

                long recordOffset = header.HeaderLength + (recordIndex * header.RecordLength);
                long fieldOffset = recordOffset + 1 + field.Displacement; // +1 for deletion flag

                fs.Seek(fieldOffset, SeekOrigin.Begin);

                byte[] fieldData = ConvertToDbfBytes(kvp.Value, field);
                fs.Write(fieldData, 0, fieldData.Length);
            }

            return Result<bool, string>.Ok(true);
        }
        catch (IOException ex)
        {
            return Result<bool, string>.Fail($"Error de E/S al actualizar registro: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<bool, string>.Fail($"Error al actualizar registro: {ex.Message}");
        }
    }

    public Result<int, string> UpdateRecords(
        string filePath,
        List<(int RecordIndex, Dictionary<string, object?> FieldValues)> updates)
    {
        if (updates == null || updates.Count == 0)
            return Result<int, string>.Fail("Debe proporcionar al menos una actualización.");

        int successCount = 0;
        List<string> errors = [];

        foreach ((int recordIndex, Dictionary<string, object?> fieldValues) in updates)
        {
            Result<bool, string> result = UpdateRecord(filePath, recordIndex, fieldValues);
            if (result.IsSuccess)
            {
                successCount++;
            }
            else
            {
                errors.Add($"Registro {recordIndex}: {result.GetErrorOrDefault()}");
            }
        }

        if (errors.Count > 0 && successCount == 0)
            return Result<int, string>.Fail(string.Join("; ", errors));

        return Result<int, string>.Ok(successCount);
    }

    public Result<bool, string> MarkRecordDeleted(string filePath, int recordIndex)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<bool, string>.Fail("La ruta del archivo DBF es requerida.");

        if (!File.Exists(filePath))
            return Result<bool, string>.Fail($"El archivo DBF no existe: {filePath}");

        if (recordIndex < 0)
            return Result<bool, string>.Fail("El índice del registro debe ser mayor o igual a cero.");

        try
        {
            DbfHeader header = ReadHeader(filePath);

            if (recordIndex >= header.RecordCount)
                return Result<bool, string>.Fail($"Índice de registro {recordIndex} fuera de rango.");

            using FileStream fs = new(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            long recordOffset = header.HeaderLength + (recordIndex * header.RecordLength);
            fs.Seek(recordOffset, SeekOrigin.Begin);
            fs.WriteByte(DeletedRecordFlag);

            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool, string>.Fail($"Error al marcar registro como eliminado: {ex.Message}");
        }
    }

    private static DbfHeader ReadHeader(string filePath)
    {
        using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using BinaryReader reader = new(fs, Encoding.ASCII);

        // Read main header (32 bytes)
        byte version = reader.ReadByte();
        byte[] lastUpdate = reader.ReadBytes(3);
        int recordCount = reader.ReadInt32();
        short headerLength = reader.ReadInt16();
        short recordLength = reader.ReadInt16();
        reader.ReadBytes(20); // Reserved bytes

        // Calculate number of field descriptors
        int fieldCount = (headerLength - HeaderSize - 1) / FieldDescriptorSize;

        // Read field descriptors
        List<DbfFieldDescriptor> fields = [];
        int displacement = 0;

        for (int i = 0; i < fieldCount; i++)
        {
            byte[] nameBytes = reader.ReadBytes(11);
            string name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');

            char fieldType = (char)reader.ReadByte();
            reader.ReadBytes(4); // Reserved
            byte length = reader.ReadByte();
            byte decimalCount = reader.ReadByte();
            reader.ReadBytes(14); // Reserved

            fields.Add(new DbfFieldDescriptor
            {
                Name = name,
                Type = fieldType,
                Length = length,
                DecimalCount = decimalCount,
                Displacement = displacement
            });

            displacement += length;
        }

        return new DbfHeader
        {
            Version = version,
            RecordCount = recordCount,
            HeaderLength = headerLength,
            RecordLength = recordLength,
            Fields = fields
        };
    }

    private static byte[] ConvertToDbfBytes(object? value, DbfFieldDescriptor field)
    {
        Encoding encoding = Encoding.GetEncoding(1252);
        byte[] result = new byte[field.Length];

        // Fill with spaces (default padding for DBF)
        for (int i = 0; i < result.Length; i++)
            result[i] = 0x20;

        if (value == null || value == DBNull.Value)
            return result;

        string stringValue = field.Type switch
        {
            'C' => ConvertCharacterField(value, field.Length),
            'N' => ConvertNumericField(value, field.Length, field.DecimalCount),
            'F' => ConvertNumericField(value, field.Length, field.DecimalCount),
            'D' => ConvertDateField(value),
            'L' => ConvertLogicalField(value),
            _ => value.ToString() ?? string.Empty
        };

        byte[] valueBytes = encoding.GetBytes(stringValue);
        int copyLength = Math.Min(valueBytes.Length, field.Length);

        // For numeric fields, right-align; for others, left-align
        if (field.Type is 'N' or 'F')
        {
            int startPos = field.Length - copyLength;
            Array.Copy(valueBytes, 0, result, startPos, copyLength);
        }
        else
        {
            Array.Copy(valueBytes, 0, result, 0, copyLength);
        }

        return result;
    }

    private static string ConvertCharacterField(object value, int maxLength)
    {
        string str = value.ToString() ?? string.Empty;
        return str.Length > maxLength ? str[..maxLength] : str;
    }

    private static string ConvertNumericField(object value, int length, int decimalCount)
    {
        if (value is decimal decVal)
            return decVal.ToString($"F{decimalCount}").PadLeft(length);

        if (value is double dblVal)
            return dblVal.ToString($"F{decimalCount}").PadLeft(length);

        if (value is int intVal)
            return intVal.ToString().PadLeft(length);

        if (value is long longVal)
            return longVal.ToString().PadLeft(length);

        if (decimal.TryParse(value.ToString(), out decimal parsed))
            return parsed.ToString($"F{decimalCount}").PadLeft(length);

        return value.ToString()?.PadLeft(length) ?? string.Empty.PadLeft(length);
    }

    private static string ConvertDateField(object value)
    {
        if (value is DateTime dt)
            return dt.ToString("yyyyMMdd");

        if (DateTime.TryParse(value.ToString(), out DateTime parsed))
            return parsed.ToString("yyyyMMdd");

        return "        "; // 8 spaces for empty date
    }

    private static string ConvertLogicalField(object value)
    {
        if (value is bool b)
            return b ? "T" : "F";

        string str = value.ToString()?.ToUpperInvariant() ?? string.Empty;
        return str is "T" or "TRUE" or "Y" or "YES" or "1" ? "T" : "F";
    }

    private sealed class DbfHeader
    {
        public byte Version { get; init; }
        public int RecordCount { get; init; }
        public short HeaderLength { get; init; }
        public short RecordLength { get; init; }
        public List<DbfFieldDescriptor> Fields { get; init; } = [];
    }

    private sealed class DbfFieldDescriptor
    {
        public string Name { get; init; } = string.Empty;
        public char Type { get; init; }
        public byte Length { get; init; }
        public byte DecimalCount { get; init; }
        public int Displacement { get; init; }
    }
}
