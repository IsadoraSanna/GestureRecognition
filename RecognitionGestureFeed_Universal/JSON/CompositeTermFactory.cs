using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unica.Djestit;
using Newtonsoft.Json.Linq; 
namespace Unica.Djestit.JSON
{
    class CompositeTermFactory
    {
        CompositeTerm ParseDeclaration(JObject declaration)
        {
            CompositeTerm term = new CompositeTerm();
            return term;
        }
    }
}
