using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class PhotonStatus : MonoBehaviour
{
	void OnGUI()
	{
		string status = "Photon: " + PhotonNetwork.NetworkClientState.ToString() + "\n";

		if (PhotonNetwork.InRoom)
		{
			status += "-------------------------------------------------------\n";
			status += "VR Raum-Name: " + PhotonNetwork.CurrentRoom.Name + "\n";
			status += "Teilnehmer-Anzahl: " + PhotonNetwork.CurrentRoom.PlayerCount + "\n";
			status += "-------------------------------------------------------\n";
			status += "Player Nr.: " + PhotonNetwork.LocalPlayer.ActorNumber + "\n";
			status += "IsMasterClient: " + PhotonNetwork.IsMasterClient + "\n";
			status += "Score: " + GameController.Instance.GetScore().ToString("D2"); //added score
		}
		GUI.TextField(new Rect(10, 10, 220, 125), status);
	}
}