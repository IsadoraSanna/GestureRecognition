using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Kinect Gesture
using Microsoft.Kinect.VisualGestureBuilder;

namespace Unica.Djestit.Kinect2.VisualGestureBuilder
{
    public class GestureProgressContinuous : GestureProgress
    {
        /* Attributi */
        // Ultimo frame ricevuto
        private IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, ContinuousGestureResult> results;
        // Buffer temporale
        public Dictionary<string, Queue<ResultContinuous>> dictionaryResults { get; private set; } = new Dictionary<string, Queue<ResultContinuous>>(); 

        /* Metodi */
        public void setLastProgress(ulong id, TimeSpan timeSpan ,IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, ContinuousGestureResult> frameData)
        {
            // Aggiorna i valori dell'ultimo frame ricevuto
            bodyId = id;
            skeletonId = (int)(id % 6);
            results = frameData;

            // Buffer
            foreach(var result in results)
            {
                if (!dictionaryResults.ContainsKey(result.Key.Name))
                {
                    // Gesture non presente nel dizionario
                    dictionaryResults.Add(result.Key.Name, new Queue<ResultContinuous>());
                }
                // Gesture già presente 
                // Buffer non ancora pieno
                if (dictionaryResults[result.Key.Name].Count == 0 || Math.Abs(dictionaryResults[result.Key.Name].Last().timeSpan.TotalSeconds - timeSpan.TotalSeconds) < 1)
                    dictionaryResults[result.Key.Name].Enqueue(new ResultContinuous(result.Value.Progress, timeSpan));
                else
                {
                    // Buffer pieno
                    dictionaryResults[result.Key.Name].Dequeue();// Rimuovi il primo elemento
                    dictionaryResults[result.Key.Name].Enqueue(new ResultContinuous(result.Value.Progress, timeSpan));// Inserisce ultimo elemento
                }
            }
        }

        /// <summary>
        /// Restituisce l'ultimo FrameGestureBuilder ricevuto
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<Microsoft.Kinect.VisualGestureBuilder.Gesture, ContinuousGestureResult> getLastestProgress()
        {
            return results;
        }
        /// <summary>
        /// Restituisce l'ultimo progresso relativo ad una determinata gesture. Se questa gesture non è presente nel dizionario, restituisce -1.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public float getLastestProgress(string name)
        {
            if (dictionaryResults.ContainsKey(name))
                return dictionaryResults[name].Last().progress;
               
            return -1;
        }
        /// <summary>
        /// Restituisce gli progressi relativi ad una determinata gesture, entro un certo intervallo temporale. Se questa gesture non è presente nel dizionario, 
        /// restituisce una lista vuota.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxIntervall"></param>
        /// <returns></returns>
        public List<float> getLastProgress(string name, float maxIntervall)
        {
            // Verifica se il dizionario contiene la gesture in questione
            if(dictionaryResults.ContainsKey(name))
            {
                // Prende l'ultimo timeLast registrato
                TimeSpan timeLast = dictionaryResults[name].Last().timeSpan;
                // Restituisce
                return dictionaryResults[name].Where(item => Math.Abs(item.timeSpan.TotalMilliseconds-timeLast.TotalMilliseconds) <= maxIntervall).Select(item => item.progress).ToList();
            }

            return new List<float>();
        }
    }
}
