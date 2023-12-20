using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.System.Test
{
	public class AssemblyApp
	{
		public static WebApplicationBuilder builder = null;
		public static DocumentAnalysisOptions? documentAnalysisOptions = null;
		public static WebApplication? app = null;

		public static string TenantId = "";
		public static string UserId = "";
		public static string sasToken = "";
		public static string urlBaseDocumentAnalysisService = "";
			
		public static int waitingSeconds = 180;
	}
}
