using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandlebarsDotNet;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
// Optional Parameters
using System.Runtime.InteropServices;

namespace Unica.Djestit.JSON
{
    /// <summary>
    /// Fornisce i metodi necessari per la creazione di una libreria di gesture a partire da un file json.
    /// </summary>
    public static class GestureLibrary
    {
        /* Attributi */
        public static Dictionary<string, object> dictionary_handler_accept { get; private set; }

        /* Metodi */ 
        /// <summary>
        /// A partire dalla libreria di gesture in input crea e restituisce le gesture definite nel file path_template.
        /// </summary>
        /// <param name="path_template">Path file template .txt</param>
        /// <param name="path_library">Path file library .json</param>
        /// <param name="prefix">Prefisso</param>
        /// /// <param name="object_emitter">Tipo di emettitore</param>
        /// <returns></returns>
        public static ObjectTerm createGesture(string path_template, string path_library, string prefix, string object_emitter, 
            [Optional] Dictionary<string, object> dictionary)
        {
            // Template
            InputTemplate template = new InputTemplate();
            template.Template = path_template;
            template.Libs.Add(path_library);
            // File temporaneo
            string current = @"handlebars.json";
            File.Create(current).Close();
            // ObjectEmitter
            ObjectEmitter emitter = new ObjectEmitter();
            // Espressione - Albero
            emitter.RegisterGroundTermEmitter(prefix, (IGroundTermEmitter)Activator.CreateInstance(null, object_emitter).Unwrap());
            // Lista delle gesture selezionate
            template.GenerateGestureDefinition(current);
            // Generazione dei term che descrivono la gesture
            dictionary_handler_accept = dictionary;
            ITermReference term = template.ParseDeclaration(current, emitter);
            // Cancella il file temporaneo
            File.Delete(current);
            

            return (term as ObjectTerm);
        }
    }

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
        private const int MAXREFS = 2;

        private JObject current = null;
        private Dictionary<JObject, List<ITermReference>> lookup;
        public string Template { get; set; }
        public List<string> Libs { get; internal set; }

        public InputTemplate()
        {
            Libs = new List<string>();
            lookup = new Dictionary<JObject, List<ITermReference>>();
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
            lookup.Clear();
            using (StreamReader jsonStream = File.OpenText(declarationFile))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonStream))
            {
                current = (JObject)JToken.ReadFrom(jsonTextReader);
                t = this.ParseTerm(current, emitter);
            }

            return t;
        }

        public List<ITermReference> Resolve(string path)
        {
            string pattern = @"#[\w\d_\-]*";
            StringBuilder jsonPath = new StringBuilder();
            MatchCollection matches = Regex.Matches(path, pattern);
            jsonPath.Append("$.");
            int cursor = 0;
            foreach (Match m in matches)
            {
                jsonPath.Append(path.Substring(cursor, m.Index - cursor));
                jsonPath.Append("*[?(@.id == '").Append(m.ToString().Remove(0, 1)).Append("')]");
                cursor = m.Index + m.Length;
            }
            var selected = current.SelectToken(jsonPath.ToString());
            if(selected != null && selected is JObject)
            {
                JObject json = selected as JObject;
                return lookup[json];
            }

            return null;

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
                AddReference(json, sym);
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
                AddReference(json, c);
                return c;
            }

            
        }

        private void AddReference(JObject key, ITermReference value)
        {
            if (lookup.ContainsKey(key))
            {
                List<ITermReference> refs = lookup[key];
                refs.Add(value);
            }
            else
            {
                List<ITermReference> refs = new List<ITermReference>(MAXREFS);
                refs.Add(value);
                lookup.Add(key, refs);
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
