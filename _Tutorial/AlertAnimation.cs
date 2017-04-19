using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
    public class AlertAnimation : MonoBehaviour {

        [SerializeField]
        Transform pointerTransform;
        [SerializeField]
        Transform clickAnimTransform;
        [SerializeField]
        SpriteRenderer clickSprite;
        [SerializeField]
        SpriteRenderer clickSpriteBorder;

        float previousPreviousScale = 0;
        float previousScale = 0;

        // Update is called once per frame
        void Update() {
            float oscillatingScale = Mathf.Sin(Time.time * 3f)*.12f + 2;
            pointerTransform.localScale = new Vector3(oscillatingScale, oscillatingScale, 1);

            if (previousPreviousScale > previousScale && previousScale < oscillatingScale) {
                clickAnimTransform.localScale = new Vector3(.01f, .01f, 1);
                clickSprite.color = new Color(1,1,1,.2f);
                clickSpriteBorder.color = new Color(0, 0, 0, .2f);
            }
            float inflatingScale = Mathf.MoveTowards(clickAnimTransform.localScale.x, .2f, Time.deltaTime * .5f);
            clickAnimTransform.localScale = new Vector3(inflatingScale, inflatingScale, 1);
            clickSprite.color = new Color(1, 1, 1, 1-inflatingScale/ .2f);
            clickSpriteBorder.color = new Color(0, 0, 0, 1-inflatingScale / .2f);

            previousPreviousScale = previousScale;
            previousScale = oscillatingScale;
        }
    }
}
