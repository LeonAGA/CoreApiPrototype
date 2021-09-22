using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model.Dtos
{

    /// <summary>
    /// Clase de objeto de transferencia de datos que se usa como puente
    /// entre las aplicaciones y el API para las entidades Estado (State).
    /// Esta clase puede ser utilizada del API hacia las
    /// aplicaciones clientes y viceversa.
    /// </summary>
    public class StateDto
    {

        #region -- CONSTRUCTOR ----------------------------------------------->
        public StateDto()
        {
            Links = new List<LinkDto>();
        } 
        #endregion

        /// <summary>
        /// Identificador de sistema del estado.
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Identificador universalmente único del estado.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Nombre del estado.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Población total del estado.
        /// </summary>
        [Required]
        public int Population { get; set; }

        /// <summary>
        /// Identificador de sistema del país al que
        /// pertenece el estado.
        /// </summary>
        [Required]
        public int CountryId { get; set; }

        /// <summary>
        /// Referencia al objeto de tipo CountryDto
        /// al que pertenece el estado.
        /// </summary>
        public CountryDto Country { get; set; }

        /// <summary>
        /// Colección de enlaces de tipo HATEOAS.
        /// </summary>
        public ICollection<LinkDto> Links { get; set; }
    }
}
