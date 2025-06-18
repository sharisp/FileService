
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
                //  httpContext.Response.ContentType = "application/problem+json";

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //  logger.LogError("WebApi——error", ex);
              var res = ApiResponse<string>.Fail(ex.Message);
              // var res = ex.Message;
                await httpContext.Response.WriteAsJsonAsync(res); //这个写法存在大小写问题
                                                                  //Serialize the problem details object to the Response as JSON (using System.Text.Json)
                                                                  // var stream = httpContext.Response.Body;
                                                                  // await JsonSerializer.SerializeAsync(stream, res);
            }
        }
    }

}
