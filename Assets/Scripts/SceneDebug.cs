using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneDebug : MonoBehaviour
    {
        public int maxLines = 10;
        private Queue<string> queue = new Queue<string>();
        private string debugString = "";

        void OnEnable()
        {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (queue.Count >= maxLines) queue.Dequeue();

            queue.Enqueue(logString);

            var builder = new StringBuilder();
            foreach (var str in queue)
            {
                builder.Append(str).Append("\n");
            }

            debugString = builder.ToString();
        }

        void OnGUI()
        {
            GUI.Label(
                new Rect(
                    5,                   // x, left offset
                    Screen.height - 150, // y, bottom offset
                    300f,                // width
                    150f                 // height
                ),
                debugString,             // the display text
                GUI.skin.textArea        // use a multi-line text area
            );
        }
    }
}
