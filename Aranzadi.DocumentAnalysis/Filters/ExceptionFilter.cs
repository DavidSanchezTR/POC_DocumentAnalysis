using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Aranzadi.DocumentAnalysis.Filters
{
	public class ExceptionFilter : ExceptionFilterAttribute
	{
        public ExceptionFilter()
        {
            
        }

		public override void OnException(ExceptionContext context)
		{
			Log.Error(context.Exception, context.Exception.Message);
			base.OnException(context);
		}

	}
}
