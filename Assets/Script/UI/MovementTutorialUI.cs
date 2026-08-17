using UnityEngine;

public class MovementTutorialUI : MonoBehaviour
{
    private const float MovementInputThreshold = 0.01f;

    private bool isHidden;

    private void Start()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isHidden)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > MovementInputThreshold ||
            Mathf.Abs(verticalInput) > MovementInputThreshold)
        {
            HidePanel();
        }
    }

    private void HidePanel()
    {
        isHidden = true;
        gameObject.SetActive(false);
    }
}
