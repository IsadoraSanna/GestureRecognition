using System;
using System.Collections.Generic;
using System.Reflection;

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
                fillProperties(c, args);
                fillCompositeProperties(c, args);
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
                    fillGroundProperties(g, args);
                }
            }

            return gt;
        }

        public void  RegisterGroundTermEmitter(string prefix, IGroundTermEmitter emitter)
        {
            groundTermEmitters.Add(prefix, emitter);
        }

        /// <summary>
        /// Assegna i diversi attributi al term in soggetto.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="args"></param>
        private void fillProperties(Term term, Dictionary<string, object> args)
        {
            // Id
            if (args.ContainsKey("id"))
            {
                term.Name = args["id"].ToString();
            }
            // Complete Handler
            if (args.ContainsKey("complete_handler") && GestureLibrary.dictionary_handler_accept.ContainsKey(args["complete_handler"].ToString()))
            {
                term.setCompleteHandler(Delegate.CreateDelegate(typeof(GestureEventHandler), 
                    (MethodInfo)GestureLibrary.dictionary_handler_accept[args["complete_handler"].ToString()]) as GestureEventHandler);
            }
            // Error Handler
            if (args.ContainsKey("error_handler") && GestureLibrary.dictionary_handler_accept.ContainsKey(args["error_handler"].ToString()))
            {
                term.setErrorHandler(Delegate.CreateDelegate(typeof(GestureEventHandler), (MethodInfo)GestureLibrary.dictionary_handler_accept[args["error_handler"].ToString()]) as GestureEventHandler);
            }
        }
        /// <summary>
        /// Assegna i diversi attributi propri del composite term.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="args"></param>
        private void fillCompositeProperties(CompositeTerm c, Dictionary<string, object>args)
        {
            if(args.ContainsKey("error_tolerance"))
            {
                c.setErrorTolerance();
            }
        }
        /// <summary>
        /// Assegna i diversi attributi propri del ground term.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="args"></param>
        private void fillGroundProperties(GroundTerm g, Dictionary<string, object> args)
        {
            // Accepts
            if (args.ContainsKey("accepts") && GestureLibrary.dictionary_handler_accept.ContainsKey(args["accepts"].ToString()))
            {
                g.Accepts = (Delegate.CreateDelegate(typeof(Accepts<Token>),
                    (MethodInfo)GestureLibrary.dictionary_handler_accept[args["accepts"].ToString()]) as Accepts<Token>);
            }
            if (args.ContainsKey("_accepts") && GestureLibrary.dictionary_handler_accept.ContainsKey(args["_accepts"].ToString()))
            {
                g._Accepts = (Delegate.CreateDelegate(typeof(Accepts<Token>),
                    (MethodInfo)GestureLibrary.dictionary_handler_accept[args["_accepts"].ToString()]) as Accepts<Token>);
            }
            // Likelihood
            if (args.ContainsKey("likelihood") && GestureLibrary.dictionary_handler_accept.ContainsKey(args["likelihood"].ToString()) )
            {
                g.likelihood = Convert.ToSingle(GestureLibrary.dictionary_handler_accept[args["likelihood"].ToString()]);
            }
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
