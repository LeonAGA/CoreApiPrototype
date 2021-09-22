using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPrototype2.Model.Utilities;

namespace WebApiPrototype2.Model.RepositoryInterfaces
{
    public interface ITokenRepository
    {

        public GenericContainerWithMessages<string> 
            GenerateToken(User user, SymmetricSecurityKey privateKey);

    }
}
