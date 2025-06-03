using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject menuUI; 

    public void ShowMenu()
    {
        menuUI.SetActive(true);
    }

    public void HideMenu()
    {
        menuUI.SetActive(false);
    }
}


