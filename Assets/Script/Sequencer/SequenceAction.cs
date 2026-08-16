using System.Collections;

[System.Serializable]
public abstract class SequenceAction
{
    public abstract IEnumerator Execute();
}
