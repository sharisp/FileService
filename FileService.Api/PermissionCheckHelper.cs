using FileService.Api;
using FileService.Domain.Constants;
using FileService.Infrastructure;

namespace Listening.Admin.Api
{
    public class PermissionCheckHelper(IConfiguration configuration, ApiClientHelper apiClientHelper, IHttpContextAccessor httpContextAccessor)
    {

        public async Task<bool> CheckPermission(long userId, string permissionKey, CancellationToken cancellation = default)
        {

            var authHeader = Convert.ToString(httpContextAccessor.HttpContext?.Request.Headers["Authorization"]);
            if (string.IsNullOrEmpty(authHeader))
            {
                return false;
            }
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var url = configuration["OuterApiUrl:CheckPermissionApiUrl"];
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            var dto = new
            {
                PermissionKey = permissionKey,
                UserId = userId,
                SystemName = ConstantValues.SystemName
            };
            apiClientHelper.SetBearerToken(token);
            var res = await apiClientHelper.PostAsync<ApiResponse<string>>(url, dto);
            if (res?.Success == true)
            {
                return true;
            }
            return false;

        }
    }
}
