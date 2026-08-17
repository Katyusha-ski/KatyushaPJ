using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    protected override bool PersistAcrossScenes => false;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject panel;

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
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        if (lineText != null)
            lineText.text = line.text;
    }
}
