using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Services
{

    /// <summary>
    /// Clase de implementación de servicio para obtener información
    /// de las llaves privadas de las variables de entorno del sistema 
    /// operativo Windows y en base a ella generar una llave para
    /// firmar digitalmente los tokens con tecnología JWT.
    /// </summary>
    public static class PrivateKeyWindowsService 
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->
        private const string WAP2_PRIVATE_KEY_ENVIRONMENT_VARIABLE = "wap2";
        #endregion

        #region -- MÉTODO PARA OBTENER CLAVES DE SEGURIDAD PARA TOKENS ------->
        /// <summary>
        /// Método para obtener el token de seguridad de JWT en base
        /// a la información de un usuario.
        /// </summary>
        /// <returns></returns>
        public static GenericContainerWithMessages<SymmetricSecurityKey> GetTokenPrivateKey()
        {
            GenericContainerWithMessages<SymmetricSecurityKey> result =
                new GenericContainerWithMessages<SymmetricSecurityKey>();
            // Verificar el sistema operativo.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                string privateKeyString = 
                    Environment.GetEnvironmentVariable(WAP2_PRIVATE_KEY_ENVIRONMENT_VARIABLE);
                if (privateKeyString == null)
                {
                    string message = "La variable de entorno que contiene la" +
                        "clave privada para la generación de los Tokens no " +
                        "existe en el servidor actual.";
                    result.Messages.Add(message);
                    result.ContainsErrors = true;
                }
                else
                {
                    var tokenSymmetricSecurityKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKeyString));
                    result.ContainsErrors = false;
                    result.Element = tokenSymmetricSecurityKey;
                }
            }
            else
            {
                Console.WriteLine("Este método solo funciona en el sistema " +
                    "operativo Windows.");
            }
            return result;
        } 
        #endregion
    }
}
