using Aranzadi.DocumentAnalysis.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging.Test
{
    public class AnalysisContextTests
    {
        public static AnalysisContext ValidContext()
        {
            var context = new AnalysisContext()
            {
                App = "A",
                Tenant = "T",
                Owner = "O"
            };
            Assert.IsTrue(context.Validate());
            return context;
        }

        public static AnalysisContext InvalidValidContext()
        {
            var invalid = new AnalysisContext()
            {
                App = null,
                Tenant = null,
                Owner = null
            };
            Assert.IsFalse(invalid.Validate());
            return invalid;
        }



        [TestMethod()]
        public void Equals_Aplication_Validations()
        {
            var t = new AnalysisContext();
            var t2 = new AnalysisContext();
            AssertEqualsYHashCode(t, t2, (x, y) => x.App = y);
        }

        [TestMethod()]
        public void Equals_Tenant_Validations()
        {
            var t = new AnalysisContext() { App = "A", };
            var t2 = new AnalysisContext() { App = "a" };
            AssertEqualsYHashCode(t, t2, (x, y) => x.Tenant = y);

        }

        [TestMethod()]
        public void Equals_Owner_Validations()
        {
            var t = new AnalysisContext() { App = "A", Tenant = "o" };
            var t2 = new AnalysisContext() { App = "a", Tenant = "O" };
            AssertEqualsYHashCode(t, t2, (x, y) => x.Owner = y);
        }
        private void AssertEqualsYHashCode(AnalysisContext comparado, AnalysisContext comparando,
            Action<AnalysisContext, string> action)
        {
            Assert.IsTrue(comparado.Equals(comparando));
            Assert.AreEqual(comparado.GetHashCode(), comparando.GetHashCode());

            action(comparado, "V");
            Assert.IsFalse(comparado.Equals(comparando));
            Assert.AreNotEqual(comparado.GetHashCode(), comparando.GetHashCode());

            action(comparando, "v");
            Object obj = comparando;
            Assert.IsTrue(comparado.Equals(obj));
            Assert.IsTrue(comparado.Equals(comparando));
            Assert.AreEqual(comparado.GetHashCode(), comparando.GetHashCode());

            action(comparando, "x");
            Assert.IsFalse(comparado.Equals(comparando));
            Assert.AreNotEqual(comparado.GetHashCode(), comparando.GetHashCode());
        }

        [TestMethod()]
        public void Valid_Valid_OK()
        {
            Assert.IsTrue(ValidContext().Validate());
        }


        [TestMethod()]
        public void Valid_NullEmptyAplication_Invalid()
        {
            var context = ValidContext();
            ValidStringPropierty((x, y) => context.App = y, context);
        }

        [TestMethod()]
        public void Valid_NullEmmptyTenant_Invalid()
        {
            var context = ValidContext();
            ValidStringPropierty((x, y) => context.Tenant = y, context);
        }

        [TestMethod()]
        public void Valid_NullEmptyOwner_Invalid()
        {
            var context = ValidContext();
            ValidStringPropierty((x, y) => x.Owner = y, context);
        }

        public static void ValidStringPropierty<T>(Action<T, string> action,
            T validableObject) where T : IValidable
        {
            action(validableObject, null!);
            Assert.IsFalse(validableObject.Validate());
            action(validableObject, "");
            Assert.IsFalse(validableObject.Validate());
            action(validableObject, " ");
            Assert.IsFalse(validableObject.Validate());
        }
    }
}
