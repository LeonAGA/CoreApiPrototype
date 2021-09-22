using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model.Utilities
{
    /// <summary>
    /// Clase utilitaria para ser utilizada como puente entre unidades de
    /// trabajo y controladores de Web Api que puede contener objetos,
    /// colecciones de mensajes, excepciones y banderas de estado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericContainerWithMessages<T>
    {

        public GenericContainerWithMessages()
        {
            this.Messages = new List<string>();
        }

        public T Element { get; set; }

        public ICollection<String> Messages { get; set; }

        public bool ContainsErrors { get; set; }

        public Exception ExceptionContainer { get; set; }

    }
}
