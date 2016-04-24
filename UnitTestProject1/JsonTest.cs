using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unica.Djestit.JSON;
using Unica.Djestit.JSON.Kinect2;

namespace UnitTestProject1
{
    [TestClass]
    public class JSONTest
    {
        [TestMethod]
        public void JsonTest1()
        {
            InputTemplate template = new InputTemplate(); 
            template.Template = @"C:\Users\Davide\Documents\GestureRecognition\UnitTestProject1\JSON\appGesture.txt";
            template.Libs.Add(@"C: \Users\Davide\Documents\GestureRecognition\UnitTestProject1\JSON\kinect2GL.json");
            string current = @"C:\Users\Davide\habdlebars.json";
            ObjectEmitter emitter = new ObjectEmitter();
            emitter.RegisterGroundTermEmitter("kinect2", new KinectObjectEmitter());
            template.GenerateGestureDefinition(current); 
            ITermReference reference = template.ParseDeclaration(current, emitter);
            reference.ToString();
        }
    }
}
