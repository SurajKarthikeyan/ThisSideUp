using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager S;

    [Header("Score Settings")]
    [Tooltip("Amount of damage points required to pass the level")]
    [SerializeField] private int minDamagePoints = 100000;
    [Tooltip("Min amount of packages to deliver to pass the level")]
    [SerializeField] private int minPackagesDelivered = 6;

    [Header("Display Settings")]
    [Tooltip("The text to display the total score")]
    [SerializeField] TextMeshProUGUI scoreText;
    [Tooltip("The text to display the current package's damage")]
    [SerializeField] TextMeshProUGUI damageText;
    [Tooltip("The text to display the total number of packages delivered")]
    [SerializeField] TextMeshProUGUI packageText;
    [Tooltip("The animator that controls the damage text")]
    [SerializeField] Animator damageAnim;

    private int currentDamageTotal = 0;
    private int currentPackagesDelivered = 0;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
        {
            Debug.LogWarning("Duplicate ScoreManager in scene");
            Destroy(gameObject);
        } 
    }

    public void TallyPoints(int damage)
    {
        currentDamageTotal += damage;
        currentPackagesDelivered++;
        string printStr1 = "Damage: " + currentDamageTotal;
        string printStr2 = currentPackagesDelivered.ToString();
        scoreText.text = printStr1;
        packageText.text = printStr2;
        damageAnim.Play("UpFade");
    }

    public void SetDamage(int damage)
    {
        damageText.text = "+" + damage;
        damageAnim.Play("FadeIn");
    }

    /// <summary>
    /// Return the score in the format Vector2Int(packages,damage)
    /// </summary>
    public Vector2Int GetScore()
    {
        return new Vector2Int(currentPackagesDelivered, currentDamageTotal);
    }

    private void OnDestroy()
    {
        if (S == this)
            S = null;
    }
}
