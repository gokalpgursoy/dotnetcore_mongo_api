using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public class ExceptionHandler // Global Exception Handler
{
  public async Task Invoke(HttpContext context)
  {
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

    Exception ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (ex == null) return;
    Object error = new
    {
      success = false,
      message = ex.InnerException != null ? ex.InnerException.Message : ex.Message
    };

    context.Response.ContentType = "application/json";

    using (StreamWriter writer = new StreamWriter(context.Response.Body))
    {
      new JsonSerializer().Serialize(writer, error);
      await writer.FlushAsync().ConfigureAwait(false);
    }
  }
}