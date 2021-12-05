using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Services
{
    public class AuthService : IAuthorizationRequirement
    {
        public int MinimumGrade { get; }

        public AuthService(int minimumGrade)
        {
            MinimumGrade = minimumGrade;
        }
    }

    public class AuthServiceHandler : AuthorizationHandler<AuthService>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthService requirement)
        {
            if (!context.User.HasClaim(z => z.Type == "rol"))
            {
                return Task.CompletedTask;
            }
            else if (Convert.ToInt32(context.User.FindFirst(c => c.Type == "rol").Value) >= requirement.MinimumGrade)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
