using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandlebarsDotNet;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using Unica.Djestit;

namespace Unica.Djestit.JSON
{
    public class InputTemplate
    {
        private const string SEQUENCE = "sequence";
        private const string CHOICE = "choice";
        private const string DISABLING = "disabling";
        private const string ANYORDER = "anyOrder";
        private const string PARALLEL = "parallel";
        private const string ITERATIVE = "iterative";
        private const string GROUND = "gt";

        private const string ITERATIONS = "iterations";


        public string Template { get; set; }
        public List<string> Libs { get; internal set; }

        public InputTemplate()
        {
            Libs = new List<string>();
            Handlebars.RegisterHelper("dj", (writer, context, args) =>
            {
                if (args.Length == 1)
                {
                    writer.Write(args[0]);
                }
                else
                {
                    if (args[0] is JObject)
                    {
                        JObject jObject = (JObject)args[0];
                        jObject.AddFirst(new JProperty("id", new JValue(args[1].ToString())));
                        writer.Write(jObject);
                    }

                }

            });

        }


        public bool GenerateGestureDefinition(string targetFile)
        {
            if (Template == null || Libs.Count == 0)
            {
                return false;
            }
            try
            {
                using (StreamReader reader = File.OpenText(Template))
                {
                    using (StreamWriter writer = File.CreateText(targetFile))
                    {
                        Handlebars.Compile(reader).Invoke(writer, CompileLibraries());
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }

        }

        public ITermReference ParseDeclaration(string declarationFile, IEmitter emitter)
        {
            ITermReference t = null;
            using (StreamReader jsonStream = File.OpenText(declarationFile))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonStream))
            {
                JObject current = (JObject)JToken.ReadFrom(jsonTextReader);
                t = this.ParseTerm(current, emitter);
            }

            return t;
        }

        private ITermReference ParseTerm(JObject json, IEmitter emitter)
        {
            JProperty prop = json.Property(ITERATIVE);
            JValue val = null;
            Dictionary<string, object> args = null;
            if (prop != null)
            {
                val = (JValue) prop.Value;
                if (val.Equals(true))
                {
                    prop = json.Property(ITERATIONS);
                    if (prop != null)
                    {
                        args = new Dictionary<string, object>();
                        args.Add("iterations", (int)prop.Value);
                    }
                    ITermReference it = emitter.EmitCompositeTerm(Operator.Iterative, args);
                    it.AddChild(ParseGroundOrComposite(json, emitter));
                    return it;
                }
            }
            return ParseGroundOrComposite(json, emitter);

        }

        private ITermReference ParseGroundOrComposite(JObject json, IEmitter emitter)
        {
            if(json.Property(GROUND)!= null)
            {
                return ParseGroundTerm(json, emitter);
            }

            Operator op = Operator.None;
            JProperty childrenProp = null;
            if (json.Property(CHOICE) != null)
            {
                childrenProp = json.Property(CHOICE);
                op = Operator.Choice;
            }

            if (json.Property(DISABLING) != null)
            {
                childrenProp = json.Property(DISABLING);
                op = Operator.Disabling;
            }

            if(json.Property(ANYORDER) != null)
            {
                childrenProp = json.Property(ANYORDER);
                op = Operator.OrderIndependence;
            }

            if(json.Property(PARALLEL) != null)
            {
                childrenProp = json.Property(PARALLEL);
                op = Operator.Parallel;
            }

            if(json.Property(SEQUENCE) != null)
            {
                childrenProp = json.Property(SEQUENCE);
                op = Operator.Sequence;
            }

            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach(JProperty p in json.Properties())
            {
                if(p.Value.Type != JTokenType.Array && p.Value.Type != JTokenType.Object)
                {
                    args.Add(p.Name, ((JValue)p.Value).Value);
                }
            }


            JArray children = (JArray)childrenProp.Value;
            JProperty symmetricProperty = json.Property("symmetric");
            if (symmetricProperty != null && ((JValue) symmetricProperty.Value).Value.Equals(true))
            {
                // symmetric set, create a choice and replicate the 
                // the children for right and left 
                ITermReference sym = emitter.EmitCompositeTerm(Operator.Choice, args);
                ITermReference cleft = emitter.EmitCompositeTerm(op, args);
                foreach (JToken t in children.Children())
                {
                    
                    if (t.Type == JTokenType.Object)
                    {
                        JObject obj = (JObject)t;
                        obj.Add("_left", new JValue(true));
                        cleft.AddChild(ParseTerm(obj, emitter));
                        obj.Remove("_left");
                    }
                }
                ITermReference cright = emitter.EmitCompositeTerm(op, args);
                foreach (JToken t in children.Children())
                {

                    if (t.Type == JTokenType.Object)
                    {
                        JObject obj = (JObject)t;
                        obj.Add("_right", new JValue(true));
                        cright.AddChild(ParseTerm(obj, emitter));
                        obj.Remove("_right");
                    }
                }
                sym.AddChild(cleft);
                sym.AddChild(cright);
                return sym;
            }
            else
            {
                ITermReference c = emitter.EmitCompositeTerm(op, args);
                foreach (JToken t in children.Children())
                {
                    if (t.Type == JTokenType.Object)
                    {
                        JObject obj = (JObject)t;
                        if (json.Property("_left") != null) obj.Add("_left", new JValue(true));
                        if (json.Property("_right") != null) obj.Add("_right", new JValue(true));
                        c.AddChild(ParseTerm((JObject)t, emitter));
                        if (json.Property("_left") != null) obj.Remove("_left");
                        if (json.Property("_right") != null) obj.Remove("_right");
                    }
                }

                return c;
            }

            
        }

        private ITermReference ParseGroundTerm(JObject json, IEmitter emitter)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach(JProperty p in json.Properties())
            {
                args.Add(p.Name, ((JValue)p.Value).Value);
            }
            return emitter.EmitGroundTerm(args);
        }

        private JObject CompileLibraries()
        {
            JObject libs = new JObject();
            foreach (string filePath in Libs)
            {
                using (StreamReader jsonStream = File.OpenText(filePath))
                using (JsonTextReader jsonTextReader = new JsonTextReader(jsonStream))
                {
                    JObject current = (JObject)JToken.ReadFrom(jsonTextReader);
                    libs.Add(current.First);
                    //libs.Add(current.Properties);
                }
            }
            Debug.WriteLine(libs);
            return libs;
        }




    }
}
