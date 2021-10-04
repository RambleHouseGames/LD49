using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private bool showCredits = false;
    private bool indicateCredits = false;

    [SerializeField]
    private Image startIndicator;

    [SerializeField]
    private Image creditsIndicator;

    [SerializeField]
    private float moveSpeed = 5f;

    private float scrollTimer = 0f;

    private void Update()
    {
        float targetY = Screen.height / 2f;
        if (showCredits)
            targetY = Screen.height * 1.5f;

        if (Mathf.Abs(transform.position.y - targetY) < moveSpeed * Time.deltaTime)
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        else
        {
            float newY = transform.position.y + (moveSpeed * Time.deltaTime);
            if (targetY < transform.position.y)
                newY = transform.position.y - (moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) <= .1f)
            scrollTimer = 0f;
        else if (scrollTimer > 0f)
            scrollTimer -= Time.deltaTime;
        else
        {
            toggleIndicator();
        }

        if(Input.GetButtonDown("Jump"))
        {
            if (showCredits)
                showCredits = false;
            else if (indicateCredits)
                showCredits = true;
            else
            {
                SceneManager.UnloadSceneAsync("MenuScene");
                SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
            }
        }
    }

    private void toggleIndicator()
    {
        scrollTimer = .25f;
        indicateCredits = !indicateCredits;
        startIndicator.enabled = !indicateCredits;
        creditsIndicator.enabled = indicateCredits;
    }
}
