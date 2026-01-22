namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Entidad base que implementa principios de Domain-Driven Design
/// Principios SOLID aplicados:
/// - Single Responsibility: Solo maneja propiedades comunes de entidades
/// - Open/Closed: Abierto para extensión, cerrado para modificación
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuario que creó el registro
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Usuario que modificó por última vez el registro
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Indica si el registro está activo (Soft Delete pattern)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Actualiza los campos de auditoría para modificación
    /// Principio Single Responsibility: Centraliza lógica de auditoría
    /// </summary>
    /// <param name="updatedBy">Usuario que realiza la modificación</param>
    public virtual void MarkAsUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Marca el registro como eliminado (Soft Delete)
    /// Principio Open/Closed: Extensible sin modificar código existente
    /// </summary>
    /// <param name="deletedBy">Usuario que realiza la eliminación</param>
    public virtual void MarkAsDeleted(string deletedBy)
    {
        IsActive = false;
        MarkAsUpdated(deletedBy);
    }
}