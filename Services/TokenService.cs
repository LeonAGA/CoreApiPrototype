using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Model.Utilities;
using WebApiPrototype2.Tools;

namespace WebApiPrototype2
{
    /// <summary>
    /// Clase para la generación de tokens de seguridad
    /// con tecnología JWT.
    /// </summary>
    public class TokenService : ITokenRepository
    {
        #region -- CONSTANTES Y CAMPOS --------------------------------------->
        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public TokenService()
        {
        }

        public ClaimsIdentity CipherTools { get; private set; }
        #endregion

        #region -- MÉTODO PARA GENERAR TOKENS DE SEGURIDAD ------------------->
        /// <summary>
        /// Método para generar el token de seguridad que le proporcionará
        /// acceso al API en nombre de los usuarios.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public GenericContainerWithMessages<string> GenerateToken(
            User user,
            SymmetricSecurityKey privateKey)
        {
            GenericContainerWithMessages<string> result = new GenericContainerWithMessages<string>();
            try
            {
                // Generar la afirmaciones.
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
                    new Claim("User",CipherTextTools.CipherText(JsonConvert.SerializeObject(user)))
                };
                // Generar las credenciales.
                var credentials =
                    new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha512Signature);
                // Generar el descriptor del token.
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = credentials
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                result.ContainsErrors = false;
                result.Element = token;
            }
            catch (Exception ex)
            {
                string message = "Ha ocurrido un error al intentar generar " +
                    "el token de seguridad del usuario.";
                result.Messages.Add(message);
                result.ContainsErrors = true;
                result.ExceptionContainer = ex;
            }
            return result;
        } 
        #endregion

    }
}
