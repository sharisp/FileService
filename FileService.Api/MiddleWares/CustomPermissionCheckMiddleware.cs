using FileService.Api.Attributes;
using FileService.Domain.Constants;
using Infrastructure.SharedKernel;
using Listening.Admin.Api;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;

namespace FileService.Api.MiddleWares;

/// <summary>
/// permission check middleware
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
public class CustomPermissionCheckMiddleware(
    RequestDelegate next,
    ILogger<CustomPermissionCheckMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                // If no endpoint is found, just continue to the next middleware
                await next(context);
                return;
            }
            //check if the endpoint has AllowAnonymous attribute, if so, just return
            var allowAnonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
            if (allowAnonymous != null)
            {
                await next(context);
                return;
            }
            //check if it is a white list url, if so, just return
            var questUrl = context.Request.Path.Value?.ToLower();
            if (string.IsNullOrEmpty(questUrl)) return;

            if (CheckWhiteList(questUrl))
            {
                await next.Invoke(context);
                return;
            }


            // get userId from claims
            var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                var res = ApiResponse<string>.Fail("no login");

                await context.Response.WriteAsJsonAsync(res);
                //  return;
                //Serialize the problem details object to the Response as JSON (using System.Text.Json)
                //  var stream = context.Response.Body;
                //await JsonSerializer.SerializeAsync(stream, res);
                return;
            }


            //check permission key
            var permissionKey = endpoint.Metadata
                .GetMetadata<PermissionKeyAttribute>()?.Key;

            if (permissionKey == null)
            {
                await next(context);
                return;
            }

            string? permissionStr = context?.User.FindFirst("permissions")?.Value;
            if (!string.IsNullOrWhiteSpace(permissionStr))
            {
                var permissions = permissionStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (permissions.Contains(ConstantValues.SystemName + "." + permissionKey, StringComparer.OrdinalIgnoreCase))
                {
                    await next(context);
                    return;
                }
            }

            //can not use DI here
            var permissionHelper = context.RequestServices.GetRequiredService<PermissionCheckHelper>();

            var userIDInt = Convert.ToInt64(userId);
            var resFlag = await permissionHelper.CheckPermission(userIDInt, permissionKey);

            if (resFlag == false)
            {
                //not contains, no permission
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                var response = ApiResponse<string>.Fail("no auth to this action");

                await context.Response.WriteAsJsonAsync(response);

                return;
            }

            await next(context);
        }
        catch (Exception ex)
        {
            //  httpContext.Response.ContentType = "application/problem+json";

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            logger.LogError("WebApi——异常", ex);
            var res = ApiResponse<string>.Fail(ex.Message);

            await context.Response.WriteAsJsonAsync(res);

        }
    }

    private bool CheckWhiteList(string url)
    {
        var whiteList = AppHelper.ReadAppSettingsSection<List<string>>("WhiteList");

        if (whiteList == null || whiteList.Count == 0) return false;

        foreach (var urlitem in whiteList)
            if (urlitem.Trim('/').Equals(url.Trim('/'), StringComparison.OrdinalIgnoreCase))
                return true;
        /*  if (Urlitem.Url.IndexOf("****") > 0)
              {
                  string UrlitemP = Urlitem.url.Replace("****", "");
                  if (Regex.IsMatch(url, UrlitemP, RegexOptions.IgnoreCase)) return true;
                  if (url.Length >= UrlitemP.Length && UrlitemP.ToLower() == url.Substring(0, UrlitemP.Length).ToLower()) return true;

              }*/
        return false;
    }
}