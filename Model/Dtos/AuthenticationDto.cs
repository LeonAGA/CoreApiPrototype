using System.ComponentModel.DataAnnotations;

namespace WebApiPrototype2.Model.Dtos
{
    /// <summary>
    /// Clase de tipo objeto de transferencia de datos (DTO)
    /// para la autenticación de usuarios en el sistema.
    /// </summary>
    public class AuthenticationDto
    {
        /// <summary>
        /// Nombre del usuario registrado en el sistema.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        public string Password { get; set; }

    }
}