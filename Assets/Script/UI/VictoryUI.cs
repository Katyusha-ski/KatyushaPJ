using UnityEngine;

public class VictoryUI : Singleton<VictoryUI>
{
    protected override bool PersistAcrossScenes => false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowVictoryUI()
    {
        gameObject.SetActive(true);
    }

    public void HideVictoryUI()
    {
        gameObject.SetActive(false);
    }   
}
