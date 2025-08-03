
using System.Net;

namespace FileService.Api.MiddleWares
{


    public class CustomerExceptionMiddleware(RequestDelegate next, ILogger<CustomerExceptionMiddleware> logger)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //  logger.LogError("WebApi——error", ex);
                var res = ApiResponse<string>.Fail(ex.Message);               
                await httpContext.Response.WriteAsJsonAsync(res);
            }
        }
    }

}
