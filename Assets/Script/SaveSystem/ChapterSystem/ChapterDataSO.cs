using UnityEngine;

[CreateAssetMenu(fileName = "ChapterDataSO", menuName = "Scriptable Objects/ChapterDataSO")]
public class ChapterDataSO : ScriptableObject
{
    public int chapterID;   
    public string chapterName;
    public string mainSceneName;
}
