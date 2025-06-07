using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject UI; 

    public void ShowMenu()
    {
        UI.SetActive(true);
    }

    public void HideMenu()
    {
        UI.SetActive(false);
    }
}


