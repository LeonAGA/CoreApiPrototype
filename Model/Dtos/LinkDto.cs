using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model.Dtos
{
    /// <summary>
    /// Clase de objeto de transferencia de datos para agregar enlaces
    /// de tipo HATEOAS a las respuestas del API.
    /// </summary>
    public class LinkDto
    {
        #region -- CONSTRUCTOR ----------------------------------------------->
        public LinkDto(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        } 
        #endregion

        /// <summary>
        /// Enlace al recurso.
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Rel { get; private set; }

        /// <summary>
        /// Método o verbo HTTP.
        /// </summary>
        public string Method { get; private set; }

    }
}
