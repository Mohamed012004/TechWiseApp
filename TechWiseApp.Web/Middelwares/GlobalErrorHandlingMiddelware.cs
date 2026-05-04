using TechWise.Domains.Exceptions.BadRequest;
using TechWise.Domains.Exceptions.NotFound;
using TechWise.Domains.Exceptions.UnAuthorized;
using TechWise.Shared.ErrorsModels;

namespace TechWise.Web.Middelwares
{
    public class GlobalErrorHandlingMiddelware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddelware> _logger;

        public GlobalErrorHandlingMiddelware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddelware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next.Invoke(context);
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    context.Response.ContentType = "application/json";
                    var response = new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessege = $"Then Endpoint With {context.Request.Path} is Not Found"
                    };

                    await context.Response.WriteAsJsonAsync(response);
                }

            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.LogError(ex, ex.Message);

                // 1. Set Status Code For Response
                //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                // 2. Set Content Type Code For Response
                context.Response.ContentType = "application/json";
                // 3. Response Object (Body)
                var response = new ErrorDetails()
                {
                    ErrorMessege = ex.Message
                };

                response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    UnAuthorizedException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError  // The Error Sent By Server
                };

                context.Response.StatusCode = response.StatusCode;

                // 4. Return Response
                await context.Response.WriteAsJsonAsync(response);


            }
        }

    }

}
