using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
    public class TermProviderTest
    {
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TermProvider_NullEventAnaconda_ThrowArgumentNullException()
		{
			EventAnaconda eventAnaconda = null;
			EntityAnaconda entityAnaconda = new EntityAnaconda();
			new TermProvider(eventAnaconda, entityAnaconda);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TermProvider_NullEntityAnaconda_ThrowArgumentNullException()
		{
			EventAnaconda eventAnaconda = new EventAnaconda();
			EntityAnaconda entityAnaconda = null;
			new TermProvider(eventAnaconda, entityAnaconda);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TermProvider_NoTypePlazo_ThrowArgumentException()
		{
			EventAnaconda eventAnaconda = new EventAnaconda();
			EntityAnaconda entityAnaconda = new EntityAnaconda();
			new TermProvider(eventAnaconda, entityAnaconda);
		}

		[TestMethod]
		[DataRow(17, "Dias", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 1")]
		[DataRow(16, "Días", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 2")]
		[DataRow(15, "dias", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 3")]
		[DataRow(14, "días", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 4")]
		public void TermProvider_TypePlazoWithValidValues(int days, string period, string type)
		{
			string strPlazo = $"{days} {period}";
			EntityAnaconda entityAnaconda = new EntityAnaconda() { Type = "Plazo", Value = strPlazo };
			EventAnaconda eventAnaconda = new EventAnaconda()
			{
				Type = type,
				Entities = new List<EntityAnaconda>() { entityAnaconda }
			};
			
			TermProvider termProvider = new TermProvider(eventAnaconda, entityAnaconda);
			Assert.AreEqual(days, termProvider.Days);
			Assert.AreEqual(type, termProvider.Text);
		}

		[TestMethod]
		[DataRow("lo que sea", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 1")]
		[DataRow("1234 d  ias", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 2")]
		[DataRow("dias", "tasación costas y/o liquidación intereses", DisplayName = "Ejemplo plazo 3")]
		public void TermProvider_TypePlazoWithNoValidValues(string period, string type)
		{
			string strPlazo = period;
			EntityAnaconda entityAnaconda = new EntityAnaconda() { Type = "Plazo", Value = strPlazo };
			EventAnaconda eventAnaconda = new EventAnaconda()
			{
				Type = type,
				Entities = new List<EntityAnaconda>() { entityAnaconda }
			};

			TermProvider termProvider = new TermProvider(eventAnaconda, entityAnaconda);
			Assert.AreEqual(0, termProvider.Days);
			Assert.AreEqual(type, termProvider.Text);
			Assert.AreEqual(strPlazo, termProvider.PeriodDescription);
		}

	}
}