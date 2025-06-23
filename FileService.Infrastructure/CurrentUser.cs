using Domain.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FileService.Infrastructure
{
    public class CurrentUser(IHttpContextAccessor contextAccessor) : ICurrentUser
    {
        public long? UserId =>
            long.TryParse(contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
                ? id : null;


    }
}
