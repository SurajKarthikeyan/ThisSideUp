using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamestageProgress : MonoBehaviour
{
    private Dictionary<int, PackageDropoff> dropoffDict = new Dictionary<int, PackageDropoff> ();
    private Dictionary<int, PackageDropoff> currentDropoffPoints = new Dictionary<int, PackageDropoff> ();

    private StaticTruck truck;
    private CharacterController2D playerController;

    [Header("Timer settings")]
    [Tooltip("Amount of time the player will be given to deliver packages. Level ends when timer is up.")]
    [SerializeField] private float timeUntilLevelOver = 120.0f;
    [Tooltip("The timer text")]
    [SerializeField] private TextMeshProUGUI timeText;
    [Tooltip("The results UI")]
    [SerializeField] GameObject resultsUI;
    [Tooltip("The timer audio source")]
    [SerializeField] AudioSource timerSFX;
    [Tooltip("The amount of time remaining when the timer SFX will start playing")]
    [SerializeField] float timerSFXThreshold;
    //public Text startText; // used for showing countdown from 3, 2, 1 

    private bool levelOver = false;

    private void Awake()
    {
        

    }

    void Start()
    {
        truck = GameObject.FindGameObjectWithTag("Truck").GetComponent<StaticTruck>();
        GameObject dropoffContainer = GameObject.FindGameObjectWithTag("DropoffContainer");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();

        // Construct dictionary of all possible dropoff points
        for (int id = 0; id < dropoffContainer.transform.childCount; id++)
        {
            dropoffDict.Add(id, dropoffContainer.transform.GetChild(id).GetComponent<PackageDropoff>());
        }

        ChooseRandomDropoffs();
        print("Thingys chosen for this run:" + currentDropoffPoints.Count);
        print("Leftover thingys: " + dropoffDict.Count);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }

    private void ChooseRandomDropoffs()
    {
        List<int> numbersGenerated = new List<int>();
        while (currentDropoffPoints.Count < truck.GetNumPackages())
        {
            int i = Random.Range(0, dropoffDict.Count);
            // If we've already generated this number before, reroll until we get a valid one
            while (numbersGenerated.Contains(i)) { i = Random.Range(0, dropoffDict.Count); }
            numbersGenerated.Add(i);
            int instanceID = dropoffDict[i].GetInstanceID();
            currentDropoffPoints[instanceID] = dropoffDict[i];
            dropoffDict.Remove(i);
        }

        foreach(KeyValuePair<int, PackageDropoff> p in dropoffDict)
        {
            Object.Destroy(p.Value.gameObject);
        }
        dropoffDict.Clear();
    }

    private void UpdateTimer()
    {
        if (!levelOver)
        {
            timeUntilLevelOver -= Time.deltaTime;
            timeText.text = Mathf.RoundToInt(timeUntilLevelOver).ToString();
            //startText.text = (timeUntilLevelOver).ToString("0");
            if (timeUntilLevelOver <= 0)
            {
                timerSFX.enabled = false;
                levelOver = true;
                resultsUI.SetActive(true);
                playerController.isDisabled = true;
            }
            else if (timeUntilLevelOver < timerSFXThreshold)
                timerSFX.enabled = true;
        }
        
    }
}
