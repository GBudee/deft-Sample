using UnityEngine;
using System.Collections;
using Deft;

namespace MapEditor {
    public class MEInputManager : MonoBehaviour {

        MEController meController;

        void Start() {
            meController = GetComponent<MEController>();

            InstantiateScreenScroll();
        }
        
        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                meController.OutputMapToFile();
            }

            UpdateEdgePan();
        }

        public void OnHexMouseFour(MEHexEntry hexClicked) {
            meController.DeleteHex(hexClicked);
        }

        public void OnHexClick(MEHexEntry hexClicked) {
            meController.TerrainCycle(hexClicked);
        }

        public void OnHexDown(MEHexEntry hexDown) {
            meController.SetExtensionTerrain(hexDown);
        }

        public void OnHexMouseEnter(MEHexEntry hexEntered) {
            if (Input.GetMouseButton(0)) {
                meController.TerrainExtend(hexEntered);
            }
        }

        public void OnUnitDown(MEHexEntry unitDown) {
            meController.UnitCycle(unitDown);
        }

        //<Camera management>
        private GameObject mainCamera;
        private int screenWidth;
        private int screenHeight;

        float cameraXLeftBound;
        float cameraXRightBound;
        float cameraYBottomBound;
        float cameraYTopBound;

        private void InstantiateScreenScroll() {
            mainCamera = GameObject.Find("Main Camera");
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            cameraXLeftBound = 0;
            cameraXRightBound = 0;
            cameraYBottomBound = 0;
            cameraYTopBound = 0;

            foreach (MEHexEntry hex in meController.HexGrid.Values) {
                Vector2 hexCenter = HexVectorUtil.worldPositionOfHexCoord(hex.BoardPos);
                if (hexCenter.x < cameraXLeftBound) {
                    cameraXLeftBound = hexCenter.x;
                }
                if (hexCenter.x > cameraXRightBound) {
                    cameraXRightBound = hexCenter.x;
                }
                if (hexCenter.y < cameraYBottomBound) {
                    cameraYBottomBound = hexCenter.y;
                }
                if (hexCenter.y > cameraYTopBound) {
                    cameraYTopBound = hexCenter.y;
                }
            }

            Vector2 bottomLeft = mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0));
            Vector2 topRight = mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1, 1));
            cameraXLeftBound -= bottomLeft.x;
            cameraXRightBound -= topRight.x;
            cameraYBottomBound -= bottomLeft.y;
            cameraYTopBound -= topRight.y;
            cameraXLeftBound -= MEConfig.hexToEdgeBound;
            cameraXRightBound += MEConfig.hexToEdgeBound;
            cameraYBottomBound -= MEConfig.hexToEdgeBound;
            cameraYTopBound += MEConfig.hexToEdgeBound;
            if (cameraXLeftBound > 0) {
                cameraXLeftBound = 0;
            }
            if (cameraXRightBound < 0) {
                cameraXRightBound = 0;
            }
            if (cameraYBottomBound > 0) {
                cameraYBottomBound = 0;
            }
            if (cameraYTopBound < 0) {
                cameraYTopBound = 0;
            }
        }

        private void UpdateEdgePan() {
            float newCameraX = mainCamera.transform.position.x;
            float newCameraY = mainCamera.transform.position.y;
            if (Input.mousePosition.x < 0 + MEConfig.edgePanBoundary || Input.GetKey(KeyCode.A)) {
                newCameraX -= MEConfig.screenScrollSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x > screenWidth - MEConfig.edgePanBoundary || Input.GetKey(KeyCode.D)) {
                newCameraX += MEConfig.screenScrollSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y < 0 + MEConfig.edgePanBoundary || Input.GetKey(KeyCode.S)) {
                newCameraY -= MEConfig.screenScrollSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y > screenHeight - MEConfig.edgePanBoundary || Input.GetKey(KeyCode.W)) {
                newCameraY += MEConfig.screenScrollSpeed * Time.deltaTime;
            }
            if (newCameraX < cameraXLeftBound) {
                newCameraX = cameraXLeftBound;
            }
            if (newCameraX > cameraXRightBound) {
                newCameraX = cameraXRightBound;
            }
            if (newCameraY < cameraYBottomBound) {
                newCameraY = cameraYBottomBound;
            }
            if (newCameraY > cameraYTopBound) {
                newCameraY = cameraYTopBound;
            }
            mainCamera.transform.position = new Vector3(newCameraX, newCameraY, -10);
        }
        //</End camera management>
    }
}