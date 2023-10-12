using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItemReceiver : MonoBehaviour
{
    [Tooltip("The start package")]
    [SerializeField] GameObject startPackage;
    [Tooltip("The truck animator")]
    [SerializeField] Animator startAnim;
    [Tooltip("The truck audio source")]
    [SerializeField] AudioSource truckSFX;
    [Tooltip("The delay before the game starts loading")]
    [SerializeField] float animDelayTime;
    [Tooltip("The right border collider")]
    [SerializeField] BoxCollider2D rightCollider;
    [Tooltip("The ramming collider")]
    [SerializeField] PolygonCollider2D rammingCollider;
    [Tooltip("The tutorial package")]
    [SerializeField] GameObject tutorialPackage;
    [Tooltip("The animator that controls the tutorial pages")]
    [SerializeField] Animator tutorialAnim;
    [Tooltip("The quit package")]
    [SerializeField] GameObject quitPackage;

    Movable movableInside;
    PolygonCollider2D coll;
    bool started = false;

    private void Awake()
    {
        Time.timeScale = 1;
        coll = GetComponentInChildren<PolygonCollider2D>();
    }

    private void Update()
    {
        if (movableInside != null && Movable.selected != movableInside)
            ExecuteItem(movableInside.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movable collisionMovable = collision.gameObject.GetComponent<Movable>();
        if (collisionMovable != null)
            movableInside = collisionMovable;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Movable collisionMovable = collision.gameObject.GetComponent<Movable>();
        if (collisionMovable != null && movableInside == collisionMovable)
            movableInside = null;
    }

    void ExecuteItem(GameObject item)
    {
        if (started == false)
        {
            if (item.gameObject == startPackage)
                StartCoroutine(nameof(StartGame));
            else if (item.gameObject == tutorialPackage)
            {
                Time.timeScale = 0;
                tutorialAnim.Play("Pause");
            }
            else if (item.gameObject == quitPackage)
                Application.Quit();
            movableInside = null;
        }
    }

    IEnumerator StartGame()
    {
        started = true;
        rightCollider.enabled = false;
        rammingCollider.enabled = true;
        startAnim.Play("Truck");
        truckSFX.Play();
        yield return new WaitForSeconds(animDelayTime);
        SceneManager.LoadScene("CityBlockout");
        yield return null;
    }
}
