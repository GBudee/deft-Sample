using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu {
    public class CreditsMenuController : MonoBehaviour {
        
        public void ReturnToMain() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    }
}
