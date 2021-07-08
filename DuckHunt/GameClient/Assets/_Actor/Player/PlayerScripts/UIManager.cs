﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Start()
    {
        //await Task.Delay(100);

        if (GameInstance.instance != null)
        {
            if (GameInstance.instance.username != null)
            {
                if (GameInstance.instance.username != "")
                {
                    ConnectedToServer();
                }
            }
        }
        else
        {
            GameObject gi = new GameObject("GameInstance");
            gi.AddComponent<GameInstance>();
            gi.GetComponent<GameInstance>().username = "user";
            ConnectedToServer();
        }
    }

    public void BeginPlay()
    {

    }

    public void ConnectedToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;

        GameInstance.instance.username = usernameField.text;

        Client.instance.ConnectToServer();
    }

    public void CloseMenu()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
