using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Deft.Netcode {
    public class MatchController : NetworkBehaviour {

        public int whoAmI;
        public IDictionary<int, NetworkRepPlayer> netRepPlayers;

        void Start() {

            netRepPlayers = new Dictionary<int, NetworkRepPlayer>();
            DontDestroyOnLoad(gameObject);
        }

        [ClientRpc]
        public void RpcLoadWeaponSelect() {

            GameObject networkManager = GameObject.Find("NetworkManager");
            networkManager.GetComponent<NetworkManagerHUD>().showGUI = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Outfitting");
        }

        [ClientRpc]
        public void RpcDistributeUnits(int whoseUnits, byte[] serializedUnits) {
            if (whoAmI == whoseUnits) { // Don't distribute to the person whose units it was
                return;
            }

            GameObject.Find("WSController").GetComponent<WeaponSelect.WSController>().NetReceiveUnits(whoseUnits, serializedUnits);
        }

        [ClientRpc]
        public void RpcDistributeTurn(int whoseTurnWasThis, byte[] serializedMoves) {
            if (whoAmI == whoseTurnWasThis) { // Don't distribute to the person whose turn it was
                return;
            }

            GameObject.Find("GameController").GetComponent<GameManager>().NetReceiveMoves(serializedMoves);
        }
    }
}