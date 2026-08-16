using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float holdSeconds = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator FadeToBlack(float duration){
        DOTween.Kill(fadePanel);
        fadePanel.color = new Color(0f,0f,0f,0f);
        fadePanel.raycastTarget = true;
        yield return fadePanel.DOFade(1f, duration).SetEase(Ease.InQuad).WaitForCompletion();
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        DOTween.Kill(fadePanel);
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        yield return fadePanel.DOFade(0f, duration).SetEase(Ease.OutQuad).WaitForCompletion();
        fadePanel.raycastTarget = false;
    }

    public IEnumerator Teleport(Rigidbody2D playerRB, Vector2 destination, string loadingMessage)
    {
        if (playerRB == null)
        {
            Debug.LogError("[TeleportManager] Player Rigidbody2D is null. Cannot teleport.");
            yield break;
        }

        yield return FadeToBlack(fadeDuration);

        if (!string.IsNullOrEmpty(loadingMessage))
        {
            loadingText.text = loadingMessage;
            DOTween.Kill(loadingText);
            yield return loadingText.DOFade(1f, fadeDuration).WaitForCompletion();
        }

        yield return new WaitForSeconds(holdSeconds);

        playerRB.position = destination;

        if (!string.IsNullOrEmpty(loadingMessage)) 
        {
            yield return loadingText.DOFade(0f, fadeDuration).WaitForCompletion();
        }
        yield return FadeFromBlack(fadeDuration);
    }
}
