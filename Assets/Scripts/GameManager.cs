using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    public GameObject PlayerPrefab;
    public Text Message;
    private Character char_script; 

    void Start()
    {
        Vector3 pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        char_script = PlayerPrefab.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("{0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("{0} left room", otherPlayer.NickName);
    }

    public void SendMessage(){
        char_script.setMessage(Message.text);
    }
}
