using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu {
    public class MainMenuController : MonoBehaviour {

        [SerializeField]
        GameObject howToPlayButton;
        [SerializeField]
        GameObject quitButton;

        void Start() {
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                //howToPlayButton.SetActive(false);
                quitButton.SetActive(false);
            }
        }

        public void HowToPlay() {
            Deft.Vaults.playerDirectory = new Dictionary<int, Deft.PlayerType>();
            Deft.Vaults.playerDirectory.Add(1, Deft.PlayerType.Local);
            Deft.Vaults.playerDirectory.Add(2, Deft.PlayerType.AI);
            Deft.Vaults.mapName = "tutorialMapA";
            Deft.Vaults.wsUnitList = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial for Skirmish");
        }

        public void PlaySingleplayer() {
            Deft.Vaults.playerDirectory = new Dictionary<int, Deft.PlayerType>();
            Deft.Vaults.playerDirectory.Add(1, Deft.PlayerType.Local);
            Deft.Vaults.playerDirectory.Add(2, Deft.PlayerType.AI);
            Deft.Vaults.mapName = "invertedMapBig";
            Deft.Vaults.wsUnitList = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Outfitting");
        }

        public void PlayLocalMultiplayer() {
            Deft.Vaults.playerDirectory = new Dictionary<int, Deft.PlayerType>();
            Deft.Vaults.playerDirectory.Add(1, Deft.PlayerType.Local);
            Deft.Vaults.playerDirectory.Add(2, Deft.PlayerType.Local);
            Deft.Vaults.mapName = "invertedMapBig";
            Deft.Vaults.wsUnitList = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Outfitting");
        }

        public void PlayOnlineMultiplayer() {
            //Don't set playerDirectory, online directory is set in-game
            Deft.Vaults.mapName = "invertedMapBig";
            Deft.Vaults.wsUnitList = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Online Menu");
        }

        public void GoToCredits() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Credits Menu");
        }

        public void Quit() {
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                Screen.fullScreen = false;
            }
            else {
                Application.Quit();
            }
        }
    }
}
