using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model.Dtos
{

    /// <summary>
    /// Clase de objeto de transferencia de datos que se usa como puente
    /// entre las aplicaciones y el API para las entidades País (Country).
    /// Esta clase puede ser utilizada del API hacia las
    /// aplicaciones clientes y viceversa.
    /// </summary>
    public class CountryDto
    {

        #region -- CONSTRUCTOR ----------------------------------------------->
        public CountryDto()
        {
            Links = new List<LinkDto>();
        } 
        #endregion

        /// <summary>
        /// Identificador de sistema del país.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Identificador universalmente único del país.
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        /// Nombre del país.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Población total del país.
        /// </summary>
        [Required]
        public int Population { get; set; }

        /// <summary>
        /// Listado de estados perteneciantes al país.
        /// </summary>
        [Required]
        public ICollection<StateDto> States { get; set; }

        /// <summary>
        /// Colección de enlaces de tipo HATEOAS.
        /// </summary>
        public ICollection<LinkDto> Links { get; set; }

    }
}
