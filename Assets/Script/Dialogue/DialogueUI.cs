using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(() => DialogueManager.Instance.AdvanceLine());
    }

    public void Show(DialogueLine line)
    {
        if (panel != null)
            panel.SetActive(true);

        SetDialogueUI(line);
    }

    public void UpdateLine(DialogueLine line)
    {
        if (panel == null) return;

        SetDialogueUI(line);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
    private void SetDialogueUI(DialogueLine line)
    {
        if (line == null) return;

        var speaker = line.speaker;
        if (speaker != null)
        {
            if (nameText != null)
                nameText.text = speaker.characterName;
        }

        if (portraitImage != null)
        {
            if (speaker != null && speaker.portrait != null)
            {
                portraitImage.sprite = speaker.portrait;
                portraitImage.enabled = true;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }

        if (lineText != null)
            lineText.text = line.text;
    }
}
