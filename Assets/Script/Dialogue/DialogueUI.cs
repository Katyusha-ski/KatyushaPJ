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

    public event Action OnNextClicked;

    private GameObject optionContainer;

    private void Start()
    {
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
    /// Hiện các nút lựa chọn (instantiate từ optionButtonPrefab) xếp dọc tại vị trí
    /// nút Next, thay chỗ Next. Panel tự bật nếu đang ẩn.
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

        optionContainer = new GameObject("OptionContainer", typeof(RectTransform));
        optionContainer.transform.SetParent(panel.transform, false);
        var containerRT = (RectTransform)optionContainer.transform;

        if (nextButton != null)
        {
            var nextRT = (RectTransform)nextButton.transform;
            containerRT.anchorMin = nextRT.anchorMin;
            containerRT.anchorMax = nextRT.anchorMax;
            containerRT.pivot = nextRT.pivot;
            containerRT.anchoredPosition = nextRT.anchoredPosition;
            containerRT.sizeDelta = new Vector2(nextRT.sizeDelta.x, 0f);
        }

        var layout = optionContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleCenter;

        var fitter = optionContainer.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

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
            Destroy(optionContainer);
            optionContainer = null;
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
