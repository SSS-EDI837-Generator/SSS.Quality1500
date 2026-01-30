namespace SSS.Quality1500.Domain.Models;

using SSS.Quality1500.Domain.Enums;

/// <summary>
/// Politica de validacion para una columna del DBF.
/// Define el tipo de validacion y opciones especificas.
/// </summary>
public sealed class ValidationPolicy
{
    /// <summary>
    /// Tipo de validacion a aplicar.
    /// </summary>
    public ValidationType Type { get; init; } = ValidationType.None;

    /// <summary>
    /// Opciones especificas del tipo de validacion.
    /// Ejemplo: { "AllowFuture": "False", "AllowEmpty": "True" }
    /// </summary>
    public Dictionary<string, string> Options { get; init; } = [];

    /// <summary>Politica sin validacion.</summary>
    public static ValidationPolicy None => new();

    /// <summary>Politica de validacion de fecha.</summary>
    public static ValidationPolicy ForDate(bool allowFuture = false, bool allowEmpty = true) =>
        new()
        {
            Type = ValidationType.Date,
            Options = new Dictionary<string, string>
            {
                ["AllowFuture"] = allowFuture.ToString(),
                ["AllowEmpty"] = allowEmpty.ToString()
            }
        };

    /// <summary>Politica de validacion de codigo ICD-10.</summary>
    public static ValidationPolicy ForIcd10(bool allowEmpty = true) =>
        new()
        {
            Type = ValidationType.Icd10,
            Options = new Dictionary<string, string>
            {
                ["AllowEmpty"] = allowEmpty.ToString()
            }
        };

    /// <summary>Politica de validacion de NPI.</summary>
    public static ValidationPolicy ForNpi(string sources = "ClientApi,Nppes") =>
        new()
        {
            Type = ValidationType.Npi,
            Options = new Dictionary<string, string>
            {
                ["Sources"] = sources
            }
        };

    /// <summary>Politica de validacion de miembro.</summary>
    public static ValidationPolicy ForMember() =>
        new() { Type = ValidationType.Member };

    /// <summary>Politica de campo requerido.</summary>
    public static ValidationPolicy ForRequired() =>
        new() { Type = ValidationType.Required };
}
