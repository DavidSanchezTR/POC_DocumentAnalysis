using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
	public class RequirementProviderTest
	{

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RequirementProvider_NullEventAnaconda_ThrowArgumentNullException()
		{
			EventAnaconda eventAnaconda = null;
			EntityAnaconda entityAnaconda = new EntityAnaconda();
			new RequirementProvider(eventAnaconda, entityAnaconda);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RequirementProvider_NullEntityAnaconda_ThrowArgumentNullException()
		{
			EventAnaconda eventAnaconda = new EventAnaconda();
			EntityAnaconda entityAnaconda = null;
			new RequirementProvider(eventAnaconda, entityAnaconda);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RequirementProvider_NoTypeFechaSenalamiento_ThrowArgumentException()
		{
			EventAnaconda eventAnaconda = new EventAnaconda();
			EntityAnaconda entityAnaconda = new EntityAnaconda();
			new RequirementProvider(eventAnaconda, entityAnaconda);
		}

		[TestMethod]
		public void RequirementProvider_FechaSenalamientoNoValid()
		{
			string fechaSenalamiento = Guid.NewGuid().ToString();
			EventAnaconda eventAnaconda = new EventAnaconda();
			EntityAnaconda entityAnaconda = new EntityAnaconda()
			{
				Type = RequirementProvider.REQUIREMENT_FECHASENALAMIENTO,
				Value = fechaSenalamiento
			};
			RequirementProvider requirementProvider = new RequirementProvider(eventAnaconda, entityAnaconda);
			Assert.IsNull(requirementProvider.RequirementDate);
			Assert.AreEqual(fechaSenalamiento, requirementProvider.RequirementDateDescription);
		}

		[TestMethod]
		public void RequirementProvider_FechaSenalamientoValid()
		{
			DateTime d = DateTime.UtcNow.AddDays(-3);
			DateTime date = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
			var dateStr = date.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE);
			EventAnaconda eventAnaconda = new EventAnaconda();
			EntityAnaconda entityAnaconda = new EntityAnaconda()
			{
				Type = RequirementProvider.REQUIREMENT_FECHASENALAMIENTO,
				Value = dateStr
			};
			RequirementProvider requirementProvider = new RequirementProvider(eventAnaconda, entityAnaconda);
			Assert.IsNotNull(requirementProvider.RequirementDate);
			Assert.AreEqual(requirementProvider.RequirementDate.Value, date);
			Assert.AreEqual(dateStr, requirementProvider.RequirementDateDescription);
		}

	}
}
