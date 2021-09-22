using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPrototype2.Tools
{
    
    /// <summary>
    /// Clase para extraer y descifrar la carga útil de los tokens JWT
    /// en los encabezados de las peticiones HTTP.
    /// </summary>
    public static class JwtTools
    {
        #region -- MÉTODOS PARA LA EXTRACCIÓN Y DESCIFRADO DE TOKENS JWT ----->
        /// <summary>
        /// Método para extraer el token JWT de los encabezados de 
        /// una solicitud.
        /// </summary>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        public static string ExtractJwtFromHeader(IHeaderDictionary requestHeaders)
        {
            var rawToken = requestHeaders["Authorization"].ToString();

            if (rawToken != null)
            {
                // Extraer el token del encabezado de autorización.
                string jwt = rawToken.Replace("Bearer ", "");
                return jwt;
            }
            return null;
        }

        /// <summary>
        /// Método para obtener la carga útil de un token JWT.
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public static string GetJwtPayload(string jwt)
        {
            if (!string.IsNullOrEmpty(jwt))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                // Decifrar el contenido del token.
                string cipheredPayload = token.Payload.ToList()[1].Value.ToString();
                string payload = CipherTextTools.DecipherText(cipheredPayload);
                return payload.ToString();
            }
            return null;
        } 
        #endregion
    }
}
