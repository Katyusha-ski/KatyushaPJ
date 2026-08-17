using UnityEngine;

public class JumpTutorialTrigger : MonoBehaviour
{
    [SerializeField] private InputConfig inputConfig;
    [SerializeField] private GameObject jumpTutorialPanel;

    private bool isPanelVisible;

    private void Awake()
    {
        if (inputConfig == null)
        {
            inputConfig = InputConfig.GetDefault();
        }
    }

    private void Update()
    {
        if (!isPanelVisible) return;
        if (jumpTutorialPanel == null) return;

        if (Input.GetKeyDown(inputConfig.jumpKey))
        {
            HidePanel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (jumpTutorialPanel == null) return;

        jumpTutorialPanel.SetActive(true);
        isPanelVisible = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (jumpTutorialPanel == null) return;

        isPanelVisible = jumpTutorialPanel.activeSelf;
    }

    private void HidePanel()
    {
        jumpTutorialPanel.SetActive(false);
        isPanelVisible = false;
    }
}
