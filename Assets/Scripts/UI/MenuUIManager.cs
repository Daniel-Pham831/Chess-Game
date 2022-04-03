using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private Animator menuUIAnim;
    [SerializeField] private TMP_InputField addressInput;

    public Server server;
    public Client client;

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

        server.Init(8007);
        client.Init("127.0.0.1", 8007);
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

        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }
    public void OnConnectBtnClicked()
    {
        Debug.Log("Connect");

        client.Init(this.addressInput.text, 8007);
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

        server.Shutdown();
        client.Shutdown();
    }
}
