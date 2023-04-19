using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
	public class DocumentAnalysisDataResultJudgement
	{
		public string nombre { get; set; }
		public string jurisdiccion { get; set; }

		[JsonProperty("tipo tribunal")]
		public string tipotribunal { get; set; }
		public string ciudad { get; set; }
		public string numero { get; set; }

	}
}
