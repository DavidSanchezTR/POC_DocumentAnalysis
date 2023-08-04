namespace Aranzadi.DocumentAnalysis.Integration.Test
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