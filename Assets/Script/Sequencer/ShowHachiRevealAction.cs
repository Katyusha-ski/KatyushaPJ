using System.Collections;
using UnityEngine;

[System.Serializable]
public class ShowHachiRevealAction : SequenceAction
{
    public override IEnumerator Execute()
    {
        Transform reveal = Runner != null ? Runner.transform.Find("HachiReveal") : null;
        if (reveal != null)
            reveal.gameObject.SetActive(true);
        yield return null;
    }
}