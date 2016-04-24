using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Unica.Djestit;

namespace Unica.Djestit.JSON
{
    public class ObjectEmitter : IEmitter
    {
        private Dictionary<string, IGroundTermEmitter> groundTermEmitters;


        public ObjectEmitter()
        {
            groundTermEmitters = new Dictionary<string, IGroundTermEmitter>();
        }

        public ITermReference EmitCompositeTerm(Operator op, Dictionary<string, object> args)
        {

            CompositeTerm c = null;

            switch (op)
            {
                case Operator.Iterative:
                    Iterative iterative = new Iterative();
                    if(args != null && args.ContainsKey("iterations"))
                    {
                        iterative.Iterations = (int)args["iterations"];
                    }
                    c = iterative;
                    break;

                case Operator.Choice:
                    Choice choice = new Choice();
                    c = choice;
                    break;

                case Operator.Disabling:
                    Disabling disabling = new Disabling();
                    c = disabling;
                    break;

                case Operator.OrderIndependence:
                    OrderIndependece order = new OrderIndependece();
                    c = order;
                    break;

                case Operator.Parallel:
                    Parallel parallel = new Parallel();
                    c = parallel;
                    break;

                case Operator.Sequence:
                    Sequence sequence = new Sequence();
                    c = sequence;
                    break;

            }



            if (c != null)
            {
                if(args != null)
                {
                    if (args.ContainsKey("name"))
                    {
                        c.Name = (string)args["name"];
                    }

                    
                }
                return new ObjectTerm(c);
            }
            
            return null;
        }

        public ITermReference EmitGroundTerm(Dictionary<string, object> args)
        {
            ITermReference gt = null;

            if(args != null && args.ContainsKey("gt"))
            {
                string[] declaration = args["gt"].ToString().Split(new char[] { '.'});
                IGroundTermEmitter gte = groundTermEmitters[declaration[0]];

                if(gte != null)
                {
                    gt = gte.EmitGroundTerm(declaration, args);
                    GroundTerm g = (gt as ObjectTerm).Ground;
                    fillProperties(g, args);
                }
            }

            return gt;
        }

        public void  RegisterGroundTermEmitter(string prefix, IGroundTermEmitter emitter)
        {
            groundTermEmitters.Add(prefix, emitter);
        }

        private void fillProperties(GroundTerm g, Dictionary<string, object> args)
        {
            if (args.ContainsKey("id"))
            {
                g.Name = args["id"].ToString();
            }

            // TODO gestire gli accepts
        }
    }

    public class ObjectTerm : ITermReference
    {
        public GroundTerm Ground { get; internal set; }
        public CompositeTerm Composite { get; internal set; }

        public ObjectTerm(GroundTerm term)
        {
            this.Ground = term;
        }

        public ObjectTerm(CompositeTerm term)
        {
            this.Composite = term;
        }

        public Term GetTerm()
        {
            if (Ground == null) return Composite;
            return Ground;
        }

        void ITermReference.AddChild(ITermReference child)
        {
            if (Composite == null)
                throw new ArgumentException("this reference represents a ground term");
            ObjectTerm t = child as ObjectTerm;
            Composite.AddChild(t.GetTerm());
            
        }
    }

}
