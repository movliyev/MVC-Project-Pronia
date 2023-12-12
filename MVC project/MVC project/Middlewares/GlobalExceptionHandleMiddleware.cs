namespace MVC_Project.Middlewares
{
    public class GlobalExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandleMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

            }
            catch (Exception e)
            {

                context.Response.Redirect($"/home/errorpage?error={e.Message}");
            }
        }
    }
}
