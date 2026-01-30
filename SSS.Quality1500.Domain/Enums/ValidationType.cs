namespace SSS.Quality1500.Domain.Enums;

/// <summary>
/// Tipos de validacion disponibles para columnas del DBF.
/// </summary>
public enum ValidationType
{
    /// <summary>Sin validacion (solo mostrar).</summary>
    None,

    /// <summary>Validacion de formato de fecha y reglas temporales.</summary>
    Date,

    /// <summary>Validacion de codigo ICD-10 contra repositorio local.</summary>
    Icd10,

    /// <summary>Validacion de NPI contra API del cliente y/o NPPES.</summary>
    Npi,

    /// <summary>Validacion de ID de miembro contra API del cliente.</summary>
    Member,

    /// <summary>Campo requerido (no puede estar vacio).</summary>
    Required
}
