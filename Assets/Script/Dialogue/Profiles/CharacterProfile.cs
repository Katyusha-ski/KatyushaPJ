using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "Scriptable Objects/CharacterProfile")]
public class CharacterProfile : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
}
