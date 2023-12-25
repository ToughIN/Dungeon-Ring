using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerUIPopupManager : MonoBehaviour
{
    [Header("YOU DIED Pop Up")] [SerializeField]
    private GameObject youDiedPopUpGameObject;

    [SerializeField] private TextMeshProUGUI youDiedPopUpBackgroundText;
    [SerializeField] private TextMeshProUGUI youDiedPopUpText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

    public void SendYouDiedPopUp()
    {
        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackgroundText.characterSpacing = 0;
        StartCoroutine(StrechPopUpTextOverTime(youDiedPopUpBackgroundText, 8, 1.5f));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeoutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
    }

    private IEnumerator StrechPopUpTextOverTime(TextMeshProUGUI text, float duration,float stretchAmount)
    {
        if (duration > 0f)
        {
            float initialSpacing = text.characterSpacing; // Assuming you start with an initial value.
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                // Interpolate the character spacing based on the time elapsed relative to the duration
                text.characterSpacing = Mathf.Lerp(initialSpacing, initialSpacing*stretchAmount, timer / duration);
                yield return null; // Wait for the next frame
            }

            // Ensure the final spacing is set to the stretchAmount after the loop ends
            text.characterSpacing = stretchAmount;
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        // Ensure the canvas is enabled and start from transparent.
        canvas.alpha = 0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        // Check if duration is positive.
        if (duration > 0f)
        {
            float timer = 0f;
        
            while (timer < duration)
            {
                timer += Time.deltaTime;
                // Update the alpha based on the time elapsed relative to the duration
                canvas.alpha = Mathf.Lerp(0f, 1f, timer / duration);
                yield return null; // Wait for the next frame
            }
        }

        // Ensure the alpha is set to 1 after the loop ends
        canvas.alpha = 1f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    private IEnumerator WaitThenFadeoutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0)
        {
            while (delay > 0)
            {
                delay = delay - Time.deltaTime;
                yield return null;
            }
            canvas.alpha = 1;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
            }
        }

        canvas.alpha = 0;
        yield return null;
    }

}