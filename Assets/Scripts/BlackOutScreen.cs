using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOutScreen : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed = 1f;

    [SerializeField]
    private SpriteRenderer myRenderer;

    private bool blackOut = false;

    private void Start()
    {
        GlobalSignalManager.Inst.AddListener<PlayerDiedSignal>(onPlayerDied);
    }

    private void OnDestroy()
    {
        GlobalSignalManager.Inst.RemoveListener<PlayerDiedSignal>(onPlayerDied);
    }

    private void Update()
    {
        if(blackOut && myRenderer.color.a < 1f)
        {
            float delta = fadeSpeed * Time.deltaTime;
            if(1f - myRenderer.color.a < delta)
            {
                myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, 1f);
                GlobalSignalManager.Inst.FireSignal(new FinishedFadeOutSignal());
            }
            else
                myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, myRenderer.color.a + delta);
        }
        else if(!blackOut && myRenderer.color.a > 0f)
        {
            float delta = fadeSpeed * Time.deltaTime;
            if (myRenderer.color.a < delta)
            {
                myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, 0f);
                GlobalSignalManager.Inst.FireSignal(new FinishedFadeInSignal());
            }
            else
                myRenderer.color = new Color(myRenderer.color.r, myRenderer.color.g, myRenderer.color.b, myRenderer.color.a - delta);
        }
    }

    private void onPlayerDied(GlobalSignal signal)
    {
        blackOut = true;
    }
}
