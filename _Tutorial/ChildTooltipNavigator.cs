using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
    public class ChildTooltipNavigator : MonoBehaviour {

        [SerializeField]
        string playerPrefMinimizationKey;

        int currentTooltip = 1;
        bool minimized = true;

        void Start() {
            if (PlayerPrefs.HasKey(playerPrefMinimizationKey)) {
                if (PlayerPrefs.GetInt(playerPrefMinimizationKey) == 0) {
                    MinimizeOrMaximize();
                }
            }
            else {
                MinimizeOrMaximize();
            }
        }

        public void Previous() {
            transform.GetChild(currentTooltip).gameObject.SetActive(false);
            currentTooltip--;
            transform.GetChild(currentTooltip).gameObject.SetActive(true);
        }

        public void Next() {
            transform.GetChild(currentTooltip).gameObject.SetActive(false);
            currentTooltip++;
            transform.GetChild(currentTooltip).gameObject.SetActive(true);
        }

        public void MinimizeOrMaximize() {
            if (minimized == false) {
                transform.GetChild(currentTooltip).gameObject.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(true);
                minimized = true;
                PlayerPrefs.SetInt(playerPrefMinimizationKey, 1);
            }
            else {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(currentTooltip).gameObject.SetActive(true);
                minimized = false;
                PlayerPrefs.SetInt(playerPrefMinimizationKey, 0);
            }
            PlayerPrefs.Save();
        }
    }
}