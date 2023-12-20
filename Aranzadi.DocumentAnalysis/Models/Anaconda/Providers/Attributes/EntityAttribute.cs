using System.Reflection;
using System.Linq;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EntityAttribute : CustomAttribute
    {
		internal EntityAttribute(string entityName) : base(entityName)
        { }

        internal EntityAttribute(string entityName, Type theType) : base(entityName, theType)
        { }

        internal static void SetPropertiesByAtribute<T>(IEnumerable<EntityAnaconda> theEntities, object theObject) where T : EntityAttribute
        {
            if (theEntities != null && theEntities.Count() > 0 && theObject != null)
            {


                var theType = theObject.GetType();
                var property = theType.GetProperties(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.SetProperty);

                foreach (var prop in property)
                {
                    var entityAtributes = prop.GetCustomAttributes<T>();
                    if (entityAtributes != null)
                    {
                        foreach (var entityAtribute in entityAtributes)
                        {
                            string theValue = theEntities
                                .Where(x => entityAtribute.ValueName.Equals(x.Type))
                                .Select(y => y.Value)
                                .FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(theValue))
                            {
                                TrySetValue(theObject, prop, entityAtribute, theValue);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}
