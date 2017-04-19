using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
    public class ScreenFlashScript : MonoBehaviour {

        [SerializeField]
        UnityEngine.UI.Image image;

        // Update is called once per frame
        void Update() {
            image.color = new Color(1, 1, 1, Mathf.Lerp(image.color.a, 0, Time.deltaTime * 3f));

            if (image.color.a < .01f) {
                image.color = Color.clear;
                this.enabled = false;
            }
        }
    }
}