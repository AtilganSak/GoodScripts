using System.Collections.Generic;
using UnityEditor;

public class EditorMessageHandler
{
    const byte emitMessageLimit = 6;
    List<EditorMessage> editorMessages = new List<EditorMessage>();

    [System.Serializable]
    public struct EditorMessage
    {
        public string Message;
        public MessageType MessageType;
        public float ShowDuring;
        public float timer { get; set; }
    }

    public void EmitMessage(string _message, MessageType _messageType = MessageType.None, float _duringTime = 3)
    {
        if (editorMessages.Count < emitMessageLimit)
        {
            editorMessages.Add(
                new EditorMessage
                {
                    Message = _message,
                    MessageType = _messageType,
                    ShowDuring = _duringTime,
                    timer = (float)EditorApplication.timeSinceStartup
                }
            );
        }
    }
    public void MessageHandler()
    {
        for (int i = 0; i < editorMessages.Count; i++)
        {
            EditorGUILayout.HelpBox(editorMessages[i].Message, editorMessages[i].MessageType);
            if (EditorApplication.timeSinceStartup - editorMessages[i].timer > editorMessages[i].ShowDuring)
            {
                editorMessages.RemoveAt(i);
            }
        }
    }
}
