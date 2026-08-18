using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class SequenceAction
{
    /// <summary>GameObject đang chạy sequence (do SequencePlayer inject). Không serialize.</summary>
    [System.NonSerialized] public GameObject Runner;

    public abstract IEnumerator Execute();
}
