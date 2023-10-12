using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [Tooltip("The pause screen animator")]
    [SerializeField] Animator pauseAnimator;

    bool paused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            Time.timeScale = (paused ? 0 : 1);
            string anim = (paused ? "Pause" : "Unpause");
            pauseAnimator.Play(anim);
        }
    }
}
