using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    public static VictoryUI Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
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
