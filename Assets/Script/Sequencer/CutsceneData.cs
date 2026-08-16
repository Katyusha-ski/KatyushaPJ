using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene Data", menuName = "Scriptable Object/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [SerializeReference] public List<SequenceAction> actions = new();
}