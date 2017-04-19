using UnityEngine;
using System.Collections;

namespace Deft {
    public class ShotController : MonoBehaviour {

        public UnitSpriteManager target;
        private Rigidbody2D myRigidbody;

        void Start() {
            myRigidbody = GetComponent<Rigidbody2D>();
            //Vector2 path = (Vector2)target.transform.position - (Vector2)transform.position;
            //myRigidbody.velocity = Vector3.Normalize(Quaternion.Euler(0, 0, 90) * path) * Config.shotAccel;
            myRigidbody.velocity = Vector3.up * Config.shotAccel;
        }

        void FixedUpdate() {
            if (target == null) {
                Destroy(gameObject);
                return;
            }

            Vector2 path = (Vector2)target.transform.position - (Vector2)transform.position;

            if (path.sqrMagnitude < Config.shotDestroyRange) {
                Destroy(gameObject);
            }
            transform.rotation = Quaternion.FromToRotation(Vector3.right, path);
            myRigidbody.velocity = (Vector3)myRigidbody.velocity + Vector3.Normalize(path) * Config.shotAccel;
        }
    }
}