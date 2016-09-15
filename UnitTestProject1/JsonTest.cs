//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.IO;
//using Unica.Djestit.JSON;
//using Unica.Djestit.JSON.Kinect2;
//using System.Text.RegularExpressions;
//using System.Text;

//namespace UnitTestProject1
//{
//    [TestClass]
//    public class JSONTest
//    {
//        [TestMethod]
//        public void JsonTest1()
//        {
//            InputTemplate template = new InputTemplate(); 
//            template.Template = @"C:\Users\Davide\Documents\GestureRecognition\UnitTestProject1\JSON\appGesture.txt";
//            template.Libs.Add(@"C: \Users\Davide\Documents\GestureRecognition\UnitTestProject1\JSON\kinect2GL.json");
//            string current = @"C:\Users\Davide\habdlebars.json";
//            ObjectEmitter emitter = new ObjectEmitter();
//            emitter.RegisterGroundTermEmitter("kinect2", new KinectObjectEmitter());
//            template.GenerateGestureDefinition(current); 
//            ITermReference term = template.ParseDeclaration(current, emitter);
//            term.ToString();

//            // selezione
//            string pattern = @"#nome1.sequence.disabling";
//            Assert.IsNotNull(template.Resolve(pattern));

//        }

//        [TestMethod]
//        public void JsonSelection()
//        {
//            string declarationFile = @"C:\Users\Davide\habdlebars.json";
//            using (StreamReader jsonStream = File.OpenText(declarationFile))
//            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonStream))
//            {
//                JObject current = (JObject)JToken.ReadFrom(jsonTextReader);
//                string pattern = @"#[\w\d_\-]*";
//                string reference = @"#nome1.sequence.#nome2";
//                StringBuilder jsonPath = new StringBuilder();
//                MatchCollection matches = Regex.Matches(reference, pattern);
//                jsonPath.Append("$.");
//                int cursor = 0;
//                foreach (Match m in matches)
//                {
//                    jsonPath.Append(reference.Substring(cursor, m.Index - cursor));
//                    jsonPath.Append("*[?(@.id == '").Append(m.ToString().Remove(0, 1)).Append("')]");
//                    cursor = m.Index + m.Length;
//                }
//                var panRight = current.SelectToken("$.*[?(@.id == 'nome2')]");
//                Assert.AreEqual(jsonPath.ToString(), "$.*[?(@.id == 'nome1')].sequence.*[?(@.id == 'nome2')]");
//            }

//        }
//    }
//}
