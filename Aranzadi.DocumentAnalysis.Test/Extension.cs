using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{
    public static class MyMsTestExtensions
    {



        public static bool IsEquivalent<T, Z>(this IEnumerable<T> aEnumerable, IEnumerable<Z> bEnumerable,
            Func<T, Z, bool> comparer)
        {
            if (bEnumerable==null)
            {
                return false;
            }
            foreach (T aItem in aEnumerable)
            {
                var encontrado = false;
                foreach (Z bItem in bEnumerable)
                {
                    if (comparer(aItem, bItem))
                    {
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    throw new AssertFailedException($"No se ha encontrado {aItem} in {bEnumerable} according to {comparer}");
                }
            }

            foreach (Z bItem in bEnumerable)
            {
                var encontrado = false;
                foreach (T aItem in aEnumerable)
                {
                    if (comparer(aItem, bItem))
                    {
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    throw new AssertFailedException($"No se ha encontrado {bItem} in {aEnumerable} according to {comparer}");
                }
            }
            return true;

            //foreach (T t in a)
            //{
            //    if (b.First<T>(x => comp.Compare(x, t) == 0) == null)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

        public static void AreEquivalent<T,Z>(this Assert aa, IEnumerable<T> a, IEnumerable<Z> b, Func<T, Z, bool> comparer)
        {
            if (!(AreEquivalentImp<T, Z>(a, b, comparer))) {
                throw new AssertFailedException($"{a} it is not equivalent to {b} according to {comparer}");
            };
        }
        
        private static bool AreEquivalentImp<T, Z>(IEnumerable<T> a, IEnumerable<Z> b, Func<T, Z, bool> comparer)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
            if (a.Count()!= b.Count()) {
                throw new AssertFailedException($"{a.Count()} es ditinto a {b.Count()}");
            };

            return a.IsEquivalent<T, Z>(b, comparer);            
        }

    }
}
