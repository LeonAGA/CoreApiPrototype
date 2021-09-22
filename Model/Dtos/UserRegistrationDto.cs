using System.ComponentModel.DataAnnotations;

namespace WebApiPrototype2.Model.Dtos
{
 
    /// <summary>
    /// Clase de tipo objeto de transferencia de datos (DTO)
    /// para el registro de nuevos usuarios.
    /// Esta clase solo se usa para enviar información de las 
    /// aplicaciones clientes hacia el API.
    /// </summary>
    public class UserRegistrationDto
    {
        /// <summary>
        /// Nombre o clave de usuario.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Primer nombre del usuario.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Segundo nombre del usuario.
        /// </summary>
        [MaxLength(50)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Dirección de correo electrónico del usuario.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

    }
}
