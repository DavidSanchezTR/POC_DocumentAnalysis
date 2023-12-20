using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers.Attributes
{
	[TestClass]
	public class CategoryAttributeTest
	{
		private class TestCategoryAtribute : CategoryAttribute
		{
			public TestCategoryAtribute(string name) : base(name)
			{
			}

		}


		[DataTestMethod]
		[DataRow(null, DisplayName = "null category")]
		[DataRow("", DisplayName = "empty category")]
		public void CategoryAttribute_Constructor(string valor)
		{
			Assert.AreEqual("", new TestCategoryAtribute(valor).ValueName);
			Assert.AreEqual(CategoryAttribute.DEFAULT_TYPE, new TestCategoryAtribute(valor).ValueType);
		}

		[DataTestMethod]
		[DataRow("v1", CategoryAttribute.DEFAULT_TYPE, DisplayName = "si no hay tipo")]
		public void CategoryAttribute_Constructor_Type(string valor, CategoryAttribute.Type expectedType)
		{
			var att = new TestCategoryAtribute(valor);
			Assert.AreEqual(valor, att.ValueName);
			Assert.AreEqual(expectedType, att.ValueType);
		}


		private const string WITHVALUE_TIPE_CATEGORY = "TipoCategoryConValor";
		private const string PRIVATE_WITHVALUE_TIPE_CATEGORY = "TipoCategoryConValor_Private";
		private const string PRIVATEGET_TIPE_CATEGORY = "TipoCategoryConValor_SinGet";
		private const string PRIVATE_VALUE = "Private_VAlue";


		private class FakeClassWithProperties
		{
			[DocumentAnalysisCategory(WITHVALUE_TIPE_CATEGORY)]
			public string ConValor { get; set; }

			[DocumentAnalysisCategory(PRIVATE_WITHVALUE_TIPE_CATEGORY)]
			internal string ConValorPrivate { get; set; }

			[DocumentAnalysisCategory(PRIVATEGET_TIPE_CATEGORY)]
			internal string SinSet { get => PRIVATE_VALUE; }

		}


		[TestMethod()]
		public void SetPropertiesByAtribute_SetValue()
		{
			var elValor1 = Guid.NewGuid().ToString();
			var elValor2 = Guid.NewGuid().ToString();
			var theFakeClas = new FakeClassWithProperties();

			var categories = new List<CategoryAnaconda>()
			{
				new CategoryAnaconda()
				{
					Label = elValor1,
					Type = WITHVALUE_TIPE_CATEGORY
				},
				new CategoryAnaconda()
				{
					Label = elValor2,
					Type = PRIVATE_WITHVALUE_TIPE_CATEGORY
				},
			};
			CategoryAttribute.SetPropertiesByAtribute<DocumentAnalysisCategoryAttribute>(categories, theFakeClas);

			Assert.AreEqual(elValor1, theFakeClas.ConValor);
			Assert.AreEqual(elValor2, theFakeClas.ConValorPrivate);
			Assert.AreEqual(PRIVATE_VALUE, theFakeClas.SinSet);
		}



		[TestMethod()]
		public void SetPropertiesByAtribute_SetValueNulosVacios()
		{
			var noSeToca = Guid.NewGuid().ToString();

			var theFakeClas = new FakeClassWithProperties()
			{
				ConValor = noSeToca,
				ConValorPrivate = noSeToca
			};

			var categories = new List<CategoryAnaconda>()
			{
				new CategoryAnaconda()
				{
					Label = null,
					Type = WITHVALUE_TIPE_CATEGORY
				},
				new CategoryAnaconda()
				{
					Label = "   ",
					Type = PRIVATE_WITHVALUE_TIPE_CATEGORY
				},
			};
			CategoryAttribute.SetPropertiesByAtribute<DocumentAnalysisCategoryAttribute>(categories, theFakeClas);

			Assert.AreEqual(noSeToca, theFakeClas.ConValor);
			Assert.AreEqual(noSeToca, theFakeClas.ConValorPrivate);
		}

		[TestMethod()]
		public void SetPropertiesByAtribute_IgnoreNulls()
		{
			CategoryAttribute.SetPropertiesByAtribute<DocumentAnalysisCategoryAttribute>(null, null);
		}

	}
}
