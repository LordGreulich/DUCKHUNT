using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameobject0;
    public GameObject localPlayerPrefab;
    public string username0;

    void Start()
    {
<<<<<<< Updated upstream
        GameManager.instance.SpawnPlayer(0, "local", new Vector3(0,10,0), new Quaternion(0,0,0,0));
=======
        GameManager.instance.SpawnPlayer(0, "local", new Vector3(0, 10, 0), new Quaternion(0, 0, 0, 0));
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {

    }
}
