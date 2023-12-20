using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers.Attributes
{
	[TestClass]
	public class CustomAttributeTest
	{
		private class TestCustomAtribute : CustomAttribute
		{
			public TestCustomAtribute(string entityName) : base(entityName)
			{
			}

			public TestCustomAtribute(string entityName, Type theType) : base(entityName, theType)
			{
			}
		}

		[DataTestMethod]
		[DataRow(null, DisplayName = "null entity")]
		[DataRow("", DisplayName = "empty entity")]
		public void CustomAttribute_Constructor(string valor)
		{
			Assert.AreEqual("", new TestCustomAtribute(valor).ValueName);
			Assert.AreEqual(EntityAttribute.DEFAULT_TYPE, new TestCustomAtribute(valor).ValueType);
		}

		[DataTestMethod]
		[DataRow("v1", null, EntityAttribute.DEFAULT_TYPE, DisplayName = "si no hay tipo")]
		[DataRow("v2", EntityAttribute.Type.DateTimeMaybeUTC, EntityAttribute.Type.DateTimeMaybeUTC, DisplayName = "empty entity")]
		public void CustomAttribute_Constructor_Type(string valor, EntityAttribute.Type setType, EntityAttribute.Type expectedType)
		{
			var att = new TestCustomAtribute(valor, setType);
			Assert.AreEqual(valor, att.ValueName);
			Assert.AreEqual(expectedType, att.ValueType);
		}



	}
}
