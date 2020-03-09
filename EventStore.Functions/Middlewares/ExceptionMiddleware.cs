using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using FluentValidation;
using System.Linq;
using Newtonsoft.Json;
using Numaka.Common.Exceptions;

namespace EventStore.Functions.Middlewares
{
    public class ExceptionMiddleware : HttpMiddleware
    {
        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            try
            {
                if (Next != null)
                {
                    await Next.InvokeAsync(context);
                }
            }
            catch (EntityNotFoundException entityNotFoundEx)
            {
                var value = new { entityNotFoundEx.Message };

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, JsonConvert.SerializeObject(value));
            }
            catch (ValidationException validationEx)
            {
                var errors = validationEx.Errors.Select(e => new { e.ErrorMessage, e.PropertyName });

                var value = new { Message = "Entity validation errors. See 'errors' property for more details", Errors = errors };

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(value));
            }
            catch (ConcurrencyException concurrencyEx)
            {
                var value = new { concurrencyEx.Message };

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Conflict, JsonConvert.SerializeObject(value));
            }
            catch (RepositoryException repositoryEx)
            {
                context.Logger.LogError(repositoryEx, repositoryEx.Message);

                var value = new { Message = "Something went terribly wrong. Please contact the system administrator." };

                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(value));
            }
        }
    }
}