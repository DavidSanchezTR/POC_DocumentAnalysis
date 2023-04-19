using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
	public class DocumentAnalysisDataResultProcedureInitialProcedure
	{
		public string juzgado { get; set; }
		[JsonProperty("numero autos")]
		public string numeroautos { get; set; }
	}
}
