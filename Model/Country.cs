using System;
using System.Collections.Generic;

namespace WebApiPrototype2.Model
{
 
    /// <summary>
    /// Clase de dominio que representa las entidades de tipo País
    /// (Country) en la base de datos.
    /// </summary>
    public class Country
    {

        public Country() { }

        public int CountryId { get; set; }

        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public int Population { get; set; }

        public ICollection<State> States { get; set; }

    }
}
