using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{
    [SerializeField]
    List<SpriteRenderer> icons;

    private void Start()
    {
        float top = Camera.main.transform.position.y + Camera.main.orthographicSize;
        float right = Camera.main.transform.position.x + (Camera.main.orthographicSize * Camera.main.aspect);

        transform.position = new Vector3(right, top, -5);

        GlobalSignalManager.Inst.AddListener<PlayerGotHitSignal>(onPlayerGotHit);
    }

    private void OnDestroy()
    {
        GlobalSignalManager.Inst.RemoveListener<PlayerGotHitSignal>(onPlayerGotHit);
    }

    private void onPlayerGotHit(GlobalSignal signal)
    {
        PlayerGotHitSignal playerGotHitSignal = (PlayerGotHitSignal)signal;
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].enabled = i <= playerGotHitSignal.remainingLives - 1;
        }
    }
}
