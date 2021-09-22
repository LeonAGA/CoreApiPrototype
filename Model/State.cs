using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model
{

    /// <summary>
    /// Clase de dominio que representa las entidades de tipo Estado
    /// (Estado) en la base de datos.
    /// </summary>
    public class State
    {

        public State() {}

        public int StateId { get; set; }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public int Population { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

      
    }
}
