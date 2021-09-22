using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model.Dtos
{
    /// <summary>
    /// Clase de tipo objeto de transferencia de datos (DTO)
    /// que contiene la información de un usuario y su token
    /// JWT para acceder a la información de la API.
    /// Esta clase únicamente se usa para enviar información a las
    /// aplicaciones clientes.
    /// </summary>
    public class UserTokenDto
    {
        /// <summary>
        /// Información del usuario.
        /// </summary>
        public UserDto User { get; set; }
        
        /// <summary>
        /// Token de acceso al sistema.
        /// </summary>
        public string Token { get; set; }
    }
}
