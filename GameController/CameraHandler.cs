using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deft {
    public class CameraHandler : MonoBehaviour {

        private Camera mainCamera;

        private int screenWidth;
        private int screenHeight;
        private float cameraXLeftBound;
        private float cameraXRightBound;
        private float cameraYBottomBound;
        private float cameraYTopBound;
        private Vector2 bottomLeftOfViewport; // In world coords, accounts for ui elements via viewport input to InitializeScreenScroll
        private Vector2 topRightOfViewport;

        bool cameraLerp;
        private Vector3 cameraLerpTarget;

        private Vector2 cameraPosOnClick;
        private Vector2 mousePosOnClick;
        public bool dragPanEnabled;

        public void Initialize(List<Vector2> hexWorldPositions, Vector2 bottomLeftOfViewport, Vector2 topRightOfViewport) {
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            cameraXLeftBound = 0;
            cameraXRightBound = 0;
            cameraYBottomBound = 0;
            cameraYTopBound = 0;

            foreach (Vector2 hexPos in hexWorldPositions) {
                if (hexPos.x < cameraXLeftBound) {
                    cameraXLeftBound = hexPos.x;
                }
                if (hexPos.x > cameraXRightBound) {
                    cameraXRightBound = hexPos.x;
                }
                if (hexPos.y < cameraYBottomBound) {
                    cameraYBottomBound = hexPos.y;
                }
                if (hexPos.y > cameraYTopBound) {
                    cameraYTopBound = hexPos.y;
                }
            }

            this.bottomLeftOfViewport = bottomLeftOfViewport;
            this.topRightOfViewport = topRightOfViewport;
            Vector2 bottomLeftOfScreen = mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(bottomLeftOfViewport.x, bottomLeftOfViewport.y, 0));
            Vector2 topRightOfScreen = mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(topRightOfViewport.x, topRightOfViewport.y, 0));
            cameraXLeftBound -= bottomLeftOfScreen.x;
            cameraXRightBound -= topRightOfScreen.x;
            cameraYBottomBound -= bottomLeftOfScreen.y;
            cameraYTopBound -= topRightOfScreen.y;
            cameraXLeftBound -= Config.hexToEdgeBuffer;
            cameraXRightBound += Config.hexToEdgeBuffer;
            cameraYBottomBound -= Config.hexToEdgeBuffer;
            cameraYTopBound += Config.hexToEdgeBuffer;
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

            dragPanEnabled = true;
            cameraLerp = false;
        }

        void Update() {
            Vector2 newCameraSpot = mainCamera.transform.position;
            if (Input.mousePosition.x < 0 + Config.edgePanBoundary || Input.GetKey(KeyCode.A)) {
                newCameraSpot.x -= Config.screenScrollSpeed * Time.deltaTime;
                cameraLerp = false;
            }
            if (Input.mousePosition.x > screenWidth - Config.edgePanBoundary || Input.GetKey(KeyCode.D)) {
                newCameraSpot.x += Config.screenScrollSpeed * Time.deltaTime;
                cameraLerp = false;
            }
            if (Input.mousePosition.y < 0 + Config.edgePanBoundary || Input.GetKey(KeyCode.S)) {
                newCameraSpot.y -= Config.screenScrollSpeed * Time.deltaTime;
                cameraLerp = false;
            }
            if (Input.mousePosition.y > screenHeight - Config.edgePanBoundary || Input.GetKey(KeyCode.W)) {
                newCameraSpot.y += Config.screenScrollSpeed * Time.deltaTime;
                cameraLerp = false;
            }

            //Camera drag pan section (including checking if the mouse "clicked as button")
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {

                mousePosOnClick = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                cameraPosOnClick = mainCamera.transform.position;

                Vector2 bottomLeftOfScreen = mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(bottomLeftOfViewport.x, bottomLeftOfViewport.y, 0));
                Vector2 topRightOfScreen = mainCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(topRightOfViewport.x, topRightOfViewport.y, 0));
                if (!(mousePosOnClick.x < topRightOfScreen.x && mousePosOnClick.y < topRightOfScreen.y
                    && mousePosOnClick.x > bottomLeftOfScreen.x && mousePosOnClick.y > bottomLeftOfScreen.y)) { // If the click is not within the screen as defined in InitScreenScroll
                    dragPanEnabled = false;
                }
            }
            if (dragPanEnabled && (Input.GetMouseButton(0) || Input.GetMouseButton(1))) { // dragPanEnabled can be set in an OnMouseDown event and will order correctly
                mainCamera.transform.position = cameraPosOnClick; // Needed so the next line's ScreenToWorldPoint will be contextualized correctly
                newCameraSpot = cameraPosOnClick + (mousePosOnClick - (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition));
                cameraLerp = false;
            }
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                if ((cameraPosOnClick - (Vector2)mainCamera.transform.position).magnitude
                    + (mousePosOnClick - (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition)).magnitude
                    < Config.maxMouseOrCameraMovementInWorldForClick) {

                    IUIHandler uiHandler = GetComponent<IUIHandler>();
                    if (uiHandler != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                        if (Input.GetMouseButtonUp(0)) {
                            uiHandler.LeftClickedAsButton();
                        }
                        if (Input.GetMouseButtonUp(1)) {
                            uiHandler.RightClickedAsButton();
                        }
                    }
                }
                dragPanEnabled = true; // dragPanEnabled is always reset after the button is released
            }

            // Apply camera movement based on lerp target, edge panning, or drag panning
            if (!cameraLerp) {
                newCameraSpot = ConformToBounds(newCameraSpot);
                mainCamera.transform.position = new Vector3(newCameraSpot.x, newCameraSpot.y, -10);
            }
            else {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraLerpTarget, Time.deltaTime * Config.cameraPosLerpRate);
            }
        }

        public void LerpToHexCoords(Vector2 hexCoords) {
            Vector2 newCameraSpot = HexVectorUtil.worldPositionOfHexCoord(hexCoords);

            newCameraSpot = ConformToBounds(newCameraSpot);

            cameraLerpTarget = new Vector3(newCameraSpot.x, newCameraSpot.y, -10);
            cameraLerp = true;
        }

        public void SnapToHexCoords(Vector2 hexCoords) {
            Vector2 newCameraSpot = HexVectorUtil.worldPositionOfHexCoord(hexCoords);

            newCameraSpot = ConformToBounds(newCameraSpot);

            mainCamera.transform.position = new Vector3(newCameraSpot.x, newCameraSpot.y, -10);
            cameraLerp = false;
        }

        private Vector2 ConformToBounds(Vector2 cameraSpot) {
            if (cameraSpot.x < cameraXLeftBound) {
                cameraSpot.x = cameraXLeftBound;
            }
            if (cameraSpot.x > cameraXRightBound) {
                cameraSpot.x = cameraXRightBound;
            }
            if (cameraSpot.y < cameraYBottomBound) {
                cameraSpot.y = cameraYBottomBound;
            }
            if (cameraSpot.y > cameraYTopBound) {
                cameraSpot.y = cameraYTopBound;
            }

            return cameraSpot;
        }
    }
}