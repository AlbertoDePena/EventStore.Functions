using Numaka.Functions.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FluentValidation;
using System.Linq;
using Newtonsoft.Json;
using Numaka.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace EventStore.Functions.Middlewares
{
    public class ExceptionMiddleware : HttpMiddleware
    {
        public override async Task InvokeAsync(IHttpFunctionContext context)
        {
            try
            {
                await Next?.InvokeAsync(context);
            }
            catch (EntityNotFoundException entityNotFoundEx)
            {
                context.ActionResult = new NotFoundObjectResult(entityNotFoundEx.Message);
            }
            catch (ValidationException validationEx)
            {
                var errors = validationEx.Errors.Select(e => new { e.ErrorMessage, e.PropertyName });

                var model = new { Description = "Entity validation errors. See 'errors' property for more details", Errors = errors };

                context.ActionResult = new BadRequestObjectResult(JsonConvert.SerializeObject(model));
            }
            catch (ConcurrencyException concurrencyEx)
            {
                context.ActionResult = new ConflictObjectResult(concurrencyEx.Message);
            }
            catch (RepositoryException repositoryEx)
            {
                context.Logger.LogError(repositoryEx, repositoryEx.Message);

                context.ActionResult = new InternalServerErrorResult();
            }
        }
    }
}