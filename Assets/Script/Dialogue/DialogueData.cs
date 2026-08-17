using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Katyusha/Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines = new();
}

[Serializable]
public class DialogueLine
{
    public CharacterProfile speaker;
    [TextArea] public string text;
}
