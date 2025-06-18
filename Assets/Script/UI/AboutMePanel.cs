using UnityEngine;
using UnityEngine.UI;

public class AboutMePanel : MonoBehaviour
{
    [Header("UI References")]
    public Button closeButton;
    public Text aboutMeText;
    
    private void Start()
    {
        // Ẩn panel khi bắt đầu
        gameObject.SetActive(false);
        
        // Thiết lập text mặc định
        if (aboutMeText != null)
        {
            aboutMeText.text = "About Me\n\n" +
                              "Game Developer\n" +
                              "Unity Developer\n" +
                              "Version: 1.0\n" +
                              "Made with ❤️";
        }
        
        // Kết nối button đóng
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
    }
    
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
    
    private void OnCloseButtonClick()
    {
        HidePanel();
    }
} 