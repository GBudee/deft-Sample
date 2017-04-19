using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu {
    public class OnlineMenuController : MonoBehaviour {

        public void Quit() {
            //Clean up netcode objects
            foreach (GameObject playerNetworkRep in GameObject.FindGameObjectsWithTag("Player")) {
                Destroy(playerNetworkRep);
            }
            Destroy(GameObject.Find("NetworkManager"));
            Destroy(GameObject.Find("MatchController"));

            //Load main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    }
}
