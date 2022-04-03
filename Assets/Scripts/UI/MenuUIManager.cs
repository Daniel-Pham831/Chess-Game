using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private Animator menuUIAnim;

    public static MenuUIManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null)
            Singleton = this;
    }

    public void OnLocalGameBtnClicked()
    {
        Debug.Log("Local Game");
        menuUIAnim.SetTrigger("InGameUI");
    }
    public void OnOnlineGameBtnClicked()
    {
        Debug.Log("Online Game");
        menuUIAnim.SetTrigger("UI-2");
    }
    public void OnHostBtnClicked()
    {
        Debug.Log("Host");
        menuUIAnim.SetTrigger("UI-3");
    }
    public void OnConnectBtnClicked()
    {
        Debug.Log("Connect");
    }
    public void OnBackUI2BtnClicked()
    {
        Debug.Log("Back UI 2");
        menuUIAnim.SetTrigger("UI-1");
    }

    public void OnBackUI3BtnClicked()
    {
        Debug.Log("Back UI 3");
        menuUIAnim.SetTrigger("UI-2");
    }
}
