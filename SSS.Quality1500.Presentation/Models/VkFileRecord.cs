namespace SSS.Quality1500.Presentation.Models;

/// <summary>
/// ViewModel para representar un registro del archivo VK (DBF)
/// Usado exclusivamente en la UI para binding de información de batch
/// </summary>
public class VkFileRecord
    {
        public int RecordId { get; set; }
        public string BatchFrom { get; set; } = string.Empty;
        public string BatchTo { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool Documents { get; set; }

    }
