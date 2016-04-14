using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect Gesture
using Microsoft.Kinect.VisualGestureBuilder;

namespace RecognitionGestureFeed_Universal.Gesture.Kinect.Kinect_VisualGestureBuilder
{
    public class GestureProgressDiscrete : GestureProgress
    {
        /* Attributi */
        // Ultimo frame ricevuto
        private IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, DiscreteGestureResult> results;
        // Buffer temporale
        public Dictionary<string, Queue<ResultDiscrete>> dictionaryResults { get; private set; } = new Dictionary<string, Queue<ResultDiscrete>>();

        /* Metodi */
        public void SetLastProgress(ulong id, TimeSpan timeSpan, IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, DiscreteGestureResult> frameData)
        {
            // Aggiorna i valori dell'ultimo frame ricevuto
            bodyId = id;
            skeletonId = (int)id % 6;
            results = frameData;

            // Buffer
            foreach (var result in results)
            {
                if (!dictionaryResults.ContainsKey(result.Key.Name))
                {
                    // Gesture non presente nel dizionario
                    dictionaryResults.Add(result.Key.Name, new Queue<ResultDiscrete>());
                    break;
                }
                // Gesture già presente 
                // Buffer non ancora pieno
                if (Math.Abs(dictionaryResults[result.Key.Name].Last().timeSpan.TotalSeconds - timeSpan.TotalSeconds) > 1)
                    dictionaryResults[result.Key.Name].Enqueue(new ResultDiscrete(result.Value.Detected, result.Value.Confidence, timeSpan));
                else
                {
                    // Buffer pieno
                    dictionaryResults[result.Key.Name].Dequeue();// Rimuovi il primo elemento
                    dictionaryResults[result.Key.Name].Enqueue(new ResultDiscrete(result.Value.Detected, result.Value.Confidence, timeSpan));// Inserisce ultimo elemento
                }
            }
        }

        /// <summary>
        /// Restituisce l'ultimo FrameGestureBuilder ricevuto
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, DiscreteGestureResult> GetLastProgress()
        {
            return results;
        }

        /// <summary>
        /// Restituisce l'ultimo progresso relativo ad una determinata gesture. Se questa gesture non è presente nel dizionario, restituisce -1.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool getLastestProgress(string name)
        {
            if (dictionaryResults.ContainsKey(name))
                return dictionaryResults[name].Last().detected;

            return false;
        }
        /// <summary>
        /// Restituisce gli progressi relativi ad una determinata gesture, entro un certo intervallo temporale. Se questa gesture non è presente nel dizionario, 
        /// restituisce una lista vuota.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxIntervall"></param>
        /// <returns></returns>
        public List<bool> getLastProgress(string name, float maxIntervall)
        {
            // Verifica se il dizionario contiene la gesture in questione
            if (dictionaryResults.ContainsKey(name))
            {
                // Prende l'ultimo timeLast registrato
                TimeSpan timeLast = dictionaryResults[name].Last().timeSpan;
                // Restituisce
                return dictionaryResults[name].Where(item => Math.Abs(item.timeSpan.TotalMilliseconds - timeLast.TotalMilliseconds) <= maxIntervall).Select(item => item.detected).ToList();
            }

            return new List<bool>();
        }
    }
}
