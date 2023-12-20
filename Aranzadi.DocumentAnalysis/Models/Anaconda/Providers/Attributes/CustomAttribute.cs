using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes
{
	public abstract class CustomAttribute : Attribute
	{
		public enum Type
		{
			String = 1,
			Int = 2,
			DateTimeMaybeUTC = 3
		}

		internal const Type DEFAULT_TYPE = Type.String;
		internal const string DATETIME_UTC_STRING_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";
		internal const string DATETIME_UNKNOWN_TIME_ZONE = "yyyy-MM-dd HH:mm:ss";

		internal CustomAttribute(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				ValueName = string.Empty;
			}
			else
			{
				ValueName = name;
			}
			ValueType = DEFAULT_TYPE;
		}

		internal CustomAttribute(string name, Type theType) : this(name)
		{
			if (theType == 0)
			{
				ValueType = DEFAULT_TYPE;
			}
			else
			{
				ValueType = theType;
			}

		}

		public string ValueName { get; }
		public Type ValueType { get; }

		protected static void TrySetValue<T>(object theObject, PropertyInfo prop, T customAtribute, string theValue)
			where T : CustomAttribute
		{
			try
			{
				if (customAtribute.ValueType == Type.DateTimeMaybeUTC)
				{
					TrySetDatatimeValue(theObject, prop, theValue);
				}
				else if (customAtribute.ValueType == Type.String)
				{
					if (string.IsNullOrWhiteSpace((string?)prop.GetValue(theObject)))
					{
						prop.SetValue(theObject, theValue);
					}
				}
				else
				{
					prop.SetValue(theObject, theValue);
				}
			}
			catch (ArgumentException)
			{

				// If the type of the Property doesn't suit the type of the value we consume the exception, 
				// only try to set
			}

		}

		protected static void TrySetDatatimeValue(object theObject, PropertyInfo prop, string theValue)
		{
			DateTime theDate;
			if (DateTime.TryParse(theValue, out theDate))
			{
				// TryParse, si reconoce fecha en utc, la traduce a local,
				// la mantenemos en UTC y quién la trate que decida 
				if (theDate.Kind == DateTimeKind.Local)
				{
					prop.SetValue(theObject, theDate.ToUniversalTime());
				}
				else
				{
					prop.SetValue(theObject, theDate);
				}
			}
		}

	}
}
