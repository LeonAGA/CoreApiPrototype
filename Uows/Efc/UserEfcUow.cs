using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiPrototype2.Data;
using WebApiPrototype2.Model;
using WebApiPrototype2.Model.Dtos;
using WebApiPrototype2.Model.RepositoryInterfaces;
using WebApiPrototype2.Model.Utilities;
using WebApiPrototype2.Services;

namespace WebApiPrototype2.Uows.Efc
{
    /// <summary>
    /// Clase de unidad de trabajo que implementa la funcionalidad para
    /// la consulta, modificación y eliminación de entidades de tipo 
    /// Usuario (User) así como también los métodos relacionados con 
    /// la autenticación de los mismos por medio de Entity Framework Core.
    /// </summary>
    public class UserEfcUow : IUserRepository
    {

        #region -- CONSTANTES ------------------------------------------------>

        private readonly WebApiPrototype2Context _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ITokenRepository _tokenService;

        #endregion

        #region -- CONSTRUCTOR ----------------------------------------------->
        public UserEfcUow(
            WebApiPrototype2Context context, 
            IMapper mapper,
            ITokenRepository tokenService,
            ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _logger = logger;
        } 
        #endregion

        #region -- MÉTODOS DE CONSULTA --------------------------------------->
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _context
                .User
                .ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);

        }
        public async Task<UserDto> GetUser(Guid uuid)
        {
            var user = await _context
                .User
                .Where(u => u.Uuid == uuid)
                .SingleOrDefaultAsync();
            return _mapper.Map<UserDto>(user);
        }
        #endregion

        #region -- MÉTODO DE ACTUALIZACIÓN ----------------------------------->
        public Task<GenericContainerWithMessages<UserDto>> UpdateUser(Guid uuid, Country country)
        {
            throw new NotImplementedException();
        } 
        #endregion

        public Task<GenericContainerWithMessages<UserDto>> InsertUser()
        {
            throw new NotImplementedException();
        }
        public Task<GenericContainerWithMessages<UserDto>> DeleteUser(Guid uuid)
        {
            throw new NotImplementedException();
        }

        #region --MÉTODO PARA LA AUTENTICACIÓN DE USUARIOS ------------------->
        public async Task<GenericContainerWithMessages<UserTokenDto>> Authenticate(AuthenticationDto login)
        {
            GenericContainerWithMessages<UserTokenDto> result =
                new GenericContainerWithMessages<UserTokenDto>();
            var user = await _context.User
                .SingleOrDefaultAsync(u => u.UserName == login.UserName);
            if (user == null)
            {
                string message = "El usuario proporcionado no existe.";
                result.Messages.Add(message);
                result.ContainsErrors = true;
            }
            else
            {
                // Calcular el hash del password en base a la
                // información almacenada en la base de datos.
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash =
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
                // Comparar todos y cada uno de los bytes del hash calculado
                // con el valor almacenado en la base de datos.
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i])
                    {
                        string message = 
                            "La contraseña proporcionada no es válida.";
                        result.Messages.Add(message);
                        result.ContainsErrors = true;
                        return result;
                    }
                }
                var userDto = _mapper.Map<User, UserDto>(user);
                var privateKey = PrivateKeyWindowsService.GetTokenPrivateKey().Element;
                if (privateKey == null)
                {
                    string message =
                            "No se ha podido obtener la llave privada para generar " +
                            "el token de seguridad del usuario.";
                    result.Messages.Add(message);
                    result.ContainsErrors = true;
                    return result;
                }
                var token = _tokenService.GenerateToken(user, privateKey).Element;
                var userTokenDto = new UserTokenDto
                {
                    User = userDto,
                    Token = token
                };
                result.Element = userTokenDto;
                result.ContainsErrors = false;
                _logger.LogInformation($"El usuario {user.UserName} ha " +
                    $"ingresado al sistema.");
            }            
            return result;
        } 
        #endregion
    }
}
