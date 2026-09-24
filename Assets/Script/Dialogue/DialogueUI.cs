using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private GameObject optionContainer;

    public event Action OnNextClicked;

    protected override void Awake()
    {
        base.Awake();
        if (panel != null)
            panel.SetActive(false);

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() => DialogueManager.Instance.AdvanceLine());
            nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());
        }
    }

    public void Show(DialogueLine line)
    {
        gameObject.SetActive(true);

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

    public void SetNextVisible(bool visible)
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(visible);
    }

    /// <summary>
    /// Hiện các nút lựa chọn (instantiate từ optionButtonPrefab) vào container có sẵn
    /// trong prefab (đặt tại vị trí nút Next), thay chỗ Next. Panel tự bật nếu đang ẩn.
    /// </summary>
    public void ShowOptions(List<string> labels, Action<int> onPick)
    {
        ClearOptions();
        SetNextVisible(false);

        if (panel != null && !panel.activeSelf)
            panel.SetActive(true);
        if (panel == null || labels == null || labels.Count == 0 || optionButtonPrefab == null)
        {
            if (optionButtonPrefab == null)
                Debug.LogWarning("[DialogueUI] optionButtonPrefab chưa gán.");
            return;
        }

        if (optionContainer == null)
        {
            Debug.LogWarning("[DialogueUI] optionContainer chưa gán.");
            return;
        }

        optionContainer.SetActive(true);

        for (int i = 0; i < labels.Count; i++)
        {
            int index = i;
            GameObject btnGO = Instantiate(optionButtonPrefab, optionContainer.transform, false);
            var tmp = btnGO.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
                tmp.text = labels[index];
            var btn = btnGO.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => onPick?.Invoke(index));
        }
    }

    public void ClearOptions()
    {
        if (optionContainer != null)
        {
            for (int i = optionContainer.transform.childCount - 1; i >= 0; i--)
                Destroy(optionContainer.transform.GetChild(i).gameObject);
            optionContainer.SetActive(false);
        }
        SetNextVisible(true);
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
