using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unica.Djestit.JSON
{
    public interface ITermReference
    {
        void AddChild(ITermReference child);
    }

    public interface IEmitter
    {
        void RegisterGroundTermEmitter(string prefix, IGroundTermEmitter emitter);
        ITermReference EmitCompositeTerm(Operator op, Dictionary<string, object> args);
        ITermReference EmitGroundTerm(Dictionary<string, object> args);
    }

    public interface IGroundTermEmitter
    {
        ITermReference EmitGroundTerm(string[] declaration, Dictionary<string, object> args);
    }

    public enum Operator
    {
        Choice,
        Disabling,
        OrderIndependence,
        Parallel,
        Sequence,
        Iterative,
        None
    }
}
