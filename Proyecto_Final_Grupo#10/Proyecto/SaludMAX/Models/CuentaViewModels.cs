using System.ComponentModel.DataAnnotations;

namespace Programacion_Avanzada_Web_G10_SaludMax.Models;

public class IniciarSesionViewModel
{
    [Required, EmailAddress, Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = "";
    [Required, DataType(DataType.Password), Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = "";
    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}

public class RegistroViewModel
{
    [Required, StringLength(100), Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = "";
    [Required, EmailAddress, StringLength(160), Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = "";
    [Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = "";
    [Required, Compare(nameof(Contrasena)), DataType(DataType.Password), Display(Name = "Confirmar contraseña")]
    public string Confirmacion { get; set; } = "";
}

public class PerfilViewModel
{
    public required Usuario Usuario { get; set; }
    public required IReadOnlyList<Cita> Citas { get; set; }
}

public class ContactoViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = "";

    [Required, EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = "";

    [Required, StringLength(1000, MinimumLength = 10)]
    [Display(Name = "Mensaje")]
    public string Mensaje { get; set; } = "";
}


