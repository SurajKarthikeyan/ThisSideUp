using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPages : MonoBehaviour
{
    [Tooltip("The animator")]
    [SerializeField] Animator anim;
    [Tooltip("The page game objects")]
    [SerializeField] List<GameObject> pages;
    [Tooltip("The starting page index")]
    [SerializeField] int startPage;

    int currentPage = 0;

    private void OnEnable()
    {
        currentPage = startPage;
        SetPage(currentPage);
    }

    public void NextPage()
    {
        currentPage++;
        if (currentPage >= pages.Count)
            Close();
        else
            SetPage(currentPage);
    }

    public void PreviousPage()
    {
        currentPage--;
        if (currentPage < 0)
            Close();
        else
            SetPage(currentPage);
    }

    void DeactivateAllPages()
    {
        foreach (GameObject p in pages)
            p.SetActive(false);
    }

    void SetPage(int page)
    {
        DeactivateAllPages();
        pages[page].SetActive(true);
    }

    void Close()
    {
        currentPage = startPage;
        SetPage(currentPage);
        Time.timeScale = 1;
        anim.Play("Unpause");
    }
}
