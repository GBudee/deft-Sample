using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Deft.Netcode {
    public class NetworkRepPlayer : NetworkBehaviour {
        
        MatchController matchController;

        void Start() {

            matchController = GameObject.Find("MatchController").GetComponent<MatchController>();
            DontDestroyOnLoad(gameObject);

            if (isLocalPlayer) {
                if (isServer) {
                    matchController.whoAmI = 1;
                    matchController.netRepPlayers.Add(1, this);
                }
                else {
                    matchController.whoAmI = 2;
                    matchController.netRepPlayers.Add(2, this);
                }
            }
            else {
                if (isServer) {
                    matchController.netRepPlayers.Add(2, this);
                    matchController.RpcLoadWeaponSelect();
                }
                else {
                    matchController.netRepPlayers.Add(1, this);
                }
            }
        }

        [Command]
        public void CmdSendUnits(int myPlayerNumber, byte[] serializedUnits) {
            matchController.RpcDistributeUnits(myPlayerNumber, serializedUnits);
        }

        [Command]
        public void CmdSendMoves(int myPlayerNumber, byte[] serializedMoves) {
            matchController.RpcDistributeTurn(myPlayerNumber, serializedMoves);
        }
        
    }
}