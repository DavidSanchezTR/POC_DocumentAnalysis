using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using System.Text.RegularExpressions;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
	public class RequirementProvider : IRequirementProvider
	{
		internal const string REQUIREMENT_FECHASENALAMIENTO = "Fecha señalamiento";

		private EventAnaconda eventAnaconda;
		private EntityAnaconda entity;

		[DocumentAnalysisEntity(REQUIREMENT_FECHASENALAMIENTO, EntityAttribute.Type.DateTimeMaybeUTC)]
		public DateTime? RequirementDate { get; private set; }

		[DocumentAnalysisEntity(REQUIREMENT_FECHASENALAMIENTO, EntityAttribute.Type.String)]
		public string RequirementDateDescription { get; private set; }

		public RequirementProvider(EventAnaconda eventAnaconda, EntityAnaconda entity)
		{
			ValidateParameters(eventAnaconda, entity);
			this.eventAnaconda = eventAnaconda;
			this.entity = entity;
			this.RequirementDateDescription = string.Empty;

			EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(new List<EntityAnaconda>() { entity }, this);
		}

		private static void ValidateParameters(EventAnaconda eventAnaconda, EntityAnaconda entity)
		{
			if (eventAnaconda == null)
			{
				throw new ArgumentNullException(nameof(eventAnaconda));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}



			if (!IsValidRequirement(entity))
			{
				throw new ArgumentException("La entity no es un Requirement: " + nameof(entity));
			}
		}

		internal static bool IsValidRequirement(EntityAnaconda entityAnaconda)
		{
			if (entityAnaconda == null)
			{
				return false;
			}

			return REQUIREMENT_FECHASENALAMIENTO.Equals(entityAnaconda.Type);
		}

	}
}
