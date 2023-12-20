using System.Reflection;
using System.Linq;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class CategoryAttribute : CustomAttribute
	{
		internal CategoryAttribute(string name) : base(name)
		{
		}

		internal static void SetPropertiesByAtribute<T>(IEnumerable<CategoryAnaconda> theCategories, object theObject) where T : CategoryAttribute
		{
			if (theCategories != null && theCategories.Count() > 0 && theObject != null)
			{
				var theType = theObject.GetType();
				var property = theType.GetProperties(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.SetProperty);

				foreach (var prop in property)
				{
					var categoryAtributes = prop.GetCustomAttributes<T>();
					if (categoryAtributes != null)
					{
						foreach (var categoryAtribute in categoryAtributes)
						{
							string theValue = theCategories
								.Where(x => categoryAtribute.ValueName.Equals(x.Type))
								.Select(y => y.Label)
								.FirstOrDefault();
							if (!string.IsNullOrWhiteSpace(theValue))
							{
								TrySetValue(theObject, prop, categoryAtribute, theValue);
								break;
							}
						}
					}
				}
			}
		}

	}
}
