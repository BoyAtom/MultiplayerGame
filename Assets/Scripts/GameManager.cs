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
    public Text ChatField;
    public Text Message;
    private Character char_script;
    private PhotonView photonView;

    void Start()
    {
        Vector3 pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        photonView = GetComponent<PhotonView>();
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

    ///Отправка сообщения другим игрокам, функция запускается при окончании редактирования сообщения
    public void RPCMessage(){
        photonView.RPC("SendMessage", RpcTarget.AllBuffered, Message.text);
    }

    [PunRPC]
    public void SendMessage(string message){
        if (message.Length > 0) {
            if (ChatField.text.Length >= 90) {
                ChatField.text = "";    
            }
            ChatField.text = ChatField.text + "\n" + PhotonNetwork.NickName + ": " + message;
            Message.text = "";
        }
    }
}
