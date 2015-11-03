using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
//
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.CustomAttributes;
using RecognitionGestureFeed_Universal.Feed.FeedBack.Tree.Wrapper.Handler;


namespace UnitTestProject1
{
    [TestClass]
    public class Test_Modifies
    {
        public SortedDictionary<int, List<Modifies>> mapHandlersModifies { get; private set; }

        [TestMethod]
        public void Test_Modifies1()
        {
            mapHandlersModifies = new SortedDictionary<int, List<Modifies>>();

            Modifies A = new Modifies("A", new object());
            Modifies B = new Modifies("B", new object());
            Modifies C = new Modifies("C", new object());
            Modifies D = new Modifies("D", new object());
            Modifies E = new Modifies("E", new object());
            Modifies F = new Modifies("F", new object());

            List<Modifies> list1 = new List<Modifies>();
            list1.Add(A);
            list1.Add(B);
            list1.Add(C);
            List<Modifies> list2 = new List<Modifies>();
            list2.Add(C);
            list2.Add(D);
            List<Modifies> list3 = new List<Modifies>();
            list3.Add(E);
            list3.Add(F);
            List<Modifies> list4 = new List<Modifies>();
            list4.Add(A);
            list4.Add(C);
            list4.Add(F);

            mapHandlersModifies.Add(1, list1.ToList());
            mapHandlersModifies.Add(2, list2.ToList());
            mapHandlersModifies.Add(3, list3.ToList());
            mapHandlersModifies.Add(4, list4.ToList());

            determineConflict();

            Debug.WriteLine("porcoddio");
        }

        private void determineConflict()
        {
            // Lista degli attributi non in comune
            List<Modifies> list = new List<Modifies>();
            // Lista degli attributi in conflitto
            List<Modifies> listConfl = new List<Modifies>();

            // Determina tutti gli attributi non in comune
            foreach (var elem in this.mapHandlersModifies)
            {
                if (list.Count() > 0)
                {
                    // Memorizza temporaneamente il vecchio contenuto della lista
                    List<Modifies> temp = list.ToList();
                    // Unione con gli attributi della funzione in oggetto.
                    list = list.Union(elem.Value).ToList();
                    // Intersezione con la vecchia lista
                    listConfl = listConfl.Union(temp.Intersect(elem.Value)).ToList();
                    // Intersezione con la vecchia lista e Differenza tra le due liste
                    list = list.Except(listConfl).ToList();
                }
                else
                {
                    list = list.Union(elem.Value).ToList();
                }
            }

            // Quindi faccio la differenza con la lista creata in precedenza (così determino la lista degli attributi
            // in comune).
            foreach (var elem in this.mapHandlersModifies)
            {
                foreach (Modifies element in elem.Value.Except(list).ToList())
                    elem.Value.Remove(element);
            }
        }
    }
}
