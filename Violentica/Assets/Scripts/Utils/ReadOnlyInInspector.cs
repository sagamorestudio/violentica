using UnityEngine;

public class ReadOnlyInInspector : PropertyAttribute
{
    public string Message { get; protected set; }
    public float HelpBoxHeight { get; protected set; }
    public ReadOnlyInInspector(string message = "", float height = 24)
    {
        Message = message;
        if (string.IsNullOrEmpty(message))
            HelpBoxHeight = 0;
        else
            HelpBoxHeight = height;
    }
}