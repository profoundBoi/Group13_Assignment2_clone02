using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] GameObject ExitPosition;

    [Header("Flash Panels")]
    [SerializeField] CanvasGroup flashPanelPlayer01;
    [SerializeField] CanvasGroup flashPanelPlayer02;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportWithFlash(other.gameObject, flashPanelPlayer01));
            Debug.Log("Player teleported");
        }
        else if (other.CompareTag("Player02"))
        {
            StartCoroutine(TeleportWithFlash(other.gameObject, flashPanelPlayer02));
            Debug.Log("Player02 teleported");
        }
    }

    IEnumerator TeleportWithFlash(GameObject player, CanvasGroup flashPanel)
    {
        yield return StartCoroutine(Flash(flashPanel, 1f, 0.2f));

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = ExitPosition.transform.position;
            controller.enabled = true;
        }

        yield return StartCoroutine(Flash(flashPanel, 0f, 0.2f));
    }

    IEnumerator Flash(CanvasGroup panel, float targetAlpha, float duration)
    {
        float startAlpha = panel.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            panel.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        panel.alpha = targetAlpha;
    }
}
