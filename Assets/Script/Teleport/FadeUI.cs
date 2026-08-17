using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : Singleton<FadeUI>
{
    [SerializeField] private Image fadePanel;
    [SerializeField] private TextMeshProUGUI loadingText;

    public Image FadePanel => fadePanel;
    public TextMeshProUGUI LoadingText => loadingText;

    protected override void OnSingletonAwake()
    {
        Transform panel = transform.Find("FadePanel");
        Transform text = transform.Find("LoadingText");
        if (panel != null) fadePanel = panel.GetComponent<Image>();
        if (text != null) loadingText = text.GetComponent<TextMeshProUGUI>();
    }
}