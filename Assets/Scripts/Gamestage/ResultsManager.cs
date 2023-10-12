using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The pause manager")]
    [SerializeField] GameObject pauseManager;
    [Tooltip("The results screen animator")]
    [SerializeField] Animator resultsAnimator;
    [Tooltip("The score elements parent game object")]
    [SerializeField] GameObject scoreElements;
    [Tooltip("The packages delivered text")]
    [SerializeField] TextMeshProUGUI packagesText;
    [Tooltip("The damage dealt text")]
    [SerializeField] TextMeshProUGUI damageText;
    [Tooltip("The total score text")]
    [SerializeField] TextMeshProUGUI scoreText;
    [Tooltip("The star images")]
    [SerializeField] Image[] starImages;
    [Tooltip("The continue button")]
    [SerializeField] GameObject continueButton;

    [Header("Scoring")]
    [Tooltip("The multiplier to apply to the number of packages")]
    [SerializeField] int packageMultiplier = 10000;
    [Tooltip("The score thresholds for various star scores")]
    [SerializeField] int[] starThresholds;

    [Header("Animation")]
    [Tooltip("How long to wait for the opening animation")]
    [SerializeField] float openAnimDelay = 2f;
    [Tooltip("How long to wait for the next page animation")]
    [SerializeField] float nextAnimDelay = 1f;
    [Tooltip("How many packages to add each frame")]
    [SerializeField] float packagesSpeed = 0.1f;
    [Tooltip("How much damage to add each frame")]
    [SerializeField] float damageSpeed = 100;
    [Tooltip("How much score to add each frame")]
    [SerializeField] float scoreSpeed = 100;
    [Tooltip("How many stars to add each frame")]
    [SerializeField] float starSpeed = 0.1f;

    Vector2Int score;
    
    private void Start()
    {
        //Stop game
        Time.timeScale = 0;
        pauseManager.SetActive(false);
        //Get score
        score = ScoreManager.S.GetScore();
        //Display score
        StartCoroutine(nameof(DisplayScore));
    }

    IEnumerator DisplayScore()
    {
        yield return new WaitForSecondsRealtime(openAnimDelay);
        scoreElements.SetActive(true);
        //Packages delivered
        for (int i=0; i<=score.x; i+=Mathf.CeilToInt(packagesSpeed))
        {
            packagesText.text = i.ToString() + "x" + packageMultiplier.ToString();
            for (int j = 0; j < 1 / packagesSpeed; j++)
                yield return new WaitForEndOfFrame();
        }
        //Damage dealt
        for (int i=0; i<=score.y; i+=Mathf.CeilToInt(damageSpeed))
        {
            damageText.text = i.ToString();
            for (int j = 0; j < 1 / damageSpeed; j++)
                yield return new WaitForEndOfFrame();
        }
        //Total score
        int totalScore = score.x * packageMultiplier + score.y;
        for (int i=0; i <= totalScore; i += Mathf.CeilToInt(scoreSpeed))
        {
            scoreText.text = i.ToString();
            for (int j = 0; j < 1 / scoreSpeed; j++)
                yield return new WaitForEndOfFrame();
        }
        //Transition to next screen
        resultsAnimator.Play("Score Next");
        yield return new WaitForSecondsRealtime(nextAnimDelay);
        //Stars
        int starScore = 0;
        for (int i=0; i<starThresholds.Length; i++)
        {
            if (starThresholds[i] > totalScore)
                break;
            starScore = i + 1;
        }
        for (int i = 0; i <= starScore; i += Mathf.CeilToInt(starSpeed))
        {
            starImages[i].enabled = true;
            for (int j = 0; j < 1 / starSpeed; j++)
                yield return new WaitForEndOfFrame();
        }
        continueButton.SetActive(true);
        yield return null;
    }
}
