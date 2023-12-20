using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.System.Test
{
	[TestClass]
	public class AssemblyClose
	{

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			Console.WriteLine("AssemblyCleanup");
		}

	}
}