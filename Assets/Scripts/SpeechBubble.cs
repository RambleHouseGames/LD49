using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer bubbleRenderer;

    [SerializeField]
    private TextMeshPro text;

    [SerializeField]
    private float delay = .1f;

    private float delayTimer = 0f;

    private string targetMessage = "";
    private string currentMessage = "";

    private bool visible = false;

    private void Update()
    {
        if (visible)
        {
            if (currentMessage.Length > 0 && currentMessage.Length < targetMessage.Length && Input.GetButtonDown("Jump"))
            {
                currentMessage = targetMessage;
                text.text = currentMessage;
                GlobalSignalManager.Inst.FireSignal(new TextFinishedSignal());
            }
            if (delayTimer <= 0f)
            {
                if (currentMessage.Length < targetMessage.Length)
                {
                    currentMessage = targetMessage.Substring(0, currentMessage.Length + 1);
                    if (currentMessage.Length >= targetMessage.Length)
                        GlobalSignalManager.Inst.FireSignal(new TextFinishedSignal());
                    text.text = currentMessage;
                    delayTimer = delay;
                }
            }
            else
            {
                delayTimer -= Time.deltaTime;
            }
        }
        else
        {
            bubbleRenderer.enabled = false;
            currentMessage = "";
        }
    }

    public void SetVisible(bool visible)
    {
        currentMessage = "";
        text.text = currentMessage;
        bubbleRenderer.enabled = visible;
        this.visible = visible;
    }

    public void SetMessage(string message)
    {
        currentMessage = "";
        targetMessage = message;
    }
}
