using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Model
{
 
    /// <summary>
    /// Clase de dominio que representa las instancias de usuarios
    /// (Users) en la base de datos.
    /// </summary>
    public class User
    {
        public int UserId { get; set; }

        public Guid Uuid { get; set; }

        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

    }
}
