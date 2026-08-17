using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene Data", menuName = "Katyusha/Cutscene/Cutscene Data")]
public class CutsceneData : ScriptableObject
{
    [SerializeReference] public List<SequenceAction> actions = new();
}