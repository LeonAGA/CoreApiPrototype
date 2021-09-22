using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model.Dtos
{

    /// <summary>
    /// Clase de tipo objeto de transferencia de datos (DTO)
    /// para  la consulta y procesamiento de datos.
    /// Esta clase puede ser utilizada del API hacia las
    /// aplicaciones clientes y viceversa.
    /// </summary>
    public class UserDto
    {

        #region -- CONSTRUCTOR ----------------------------------------------->
        public UserDto()
        {
            Links = new List<LinkDto>();
        } 
        #endregion

        /// <summary>
        /// Identificador de sistema del usuario.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Identificador universalmente único del usuario.
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        /// Nombre o clave de usuario.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

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
        public string Email { get; set; }

        /// <summary>
        /// Colección de enlaces de tipo HATEOAS.
        /// </summary>
        public ICollection<LinkDto> Links { get; set; }
    }
}
