namespace SSS.Quality1500.Business.Models;

/// <summary>
/// Modelo para representar un registro del archivo VK (DBF)
/// Usado en la UI para mostrar información de batch
/// </summary>
public class VkFileRecord
    {
        public int RecordId { get; set; }
        public string BatchFrom { get; set; } = string.Empty;
        public string BatchTo { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool Documents { get; set; }

    }
