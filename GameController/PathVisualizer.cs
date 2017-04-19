using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Deft {
    public class PathVisualizer : MonoBehaviour {
        ScenarioLoader scenarioLoader;

        [SerializeField]
        GameObject pathSegment;
        [SerializeField]
        GameObject hitArrow;
        [SerializeField]
        GameObject friendlyFireWarning;
        [SerializeField]
        GameObject rangeIndicator;

        GameObject pathSegmentContainer;
        GameObject hitArrowContainer;
        GameObject rangeIndicatorContainer;

        List<GameObject> pathSegments;
        List<GameObject> hitArrows;
        List<GameObject> ffWarnings;
        IDictionary<IUnitController, List<GameObject>> rangeIndicatorSegments;

        // Use this for initialization
        void Start() {
            scenarioLoader = GetComponent<ScenarioLoader>();

            pathSegments = new List<GameObject>();
            hitArrows = new List<GameObject>();
            ffWarnings = new List<GameObject>();
            rangeIndicatorSegments = new Dictionary<IUnitController, List<GameObject>>();

            pathSegmentContainer = new GameObject("PathSegmentContainer");
            hitArrowContainer = new GameObject("HitArrowContainer");
            rangeIndicatorContainer = new GameObject("RangeIndicatorContainer");
        }

        public void SetPath(List<Outcome> pathInput, HexEntry currentHex) {
            
            for (int i = 0; i < pathInput.Count; i++) {

                GameObject newSegment = Instantiate(pathSegment, Vector3.zero, Quaternion.identity);
                newSegment.GetComponent<SpriteRenderer>().color = Config.Palette.pathFootstep;
                newSegment.transform.SetParent(pathSegmentContainer.transform);
                pathSegments.Add(newSegment);
                if (i == 0) {
                    OrientSegment(currentHex, pathInput[i].position, newSegment);
                }
                else {
                    OrientSegment(pathInput[i - 1].position, pathInput[i].position, newSegment);
                }
            }
        }

        public void ClearPath() {
            foreach(GameObject segment in pathSegments) {
                Destroy(segment);
            }
            pathSegments.Clear();
        }

        private void OrientSegment(HexEntry start, HexEntry dest, GameObject segment) {
            Vector2 startWorldPos = HexVectorUtil.worldPositionOfHexCoord(start.BoardPos);
            Vector2 destWorldPos = HexVectorUtil.worldPositionOfHexCoord(dest.BoardPos);

            segment.transform.position = new Vector3(startWorldPos.x, startWorldPos.y, 2);
            segment.transform.rotation = Quaternion.FromToRotation(Vector2.right, destWorldPos - startWorldPos);
        }

        public void CreateHitArrow(HexEntry start, HexEntry dest) {
            GameObject newArrow = Instantiate(hitArrow, Vector3.zero, Quaternion.identity);
            newArrow.GetComponent<SpriteRenderer>().color = Config.Palette.pathArrow;
            newArrow.transform.SetParent(hitArrowContainer.transform);
            hitArrows.Add(newArrow);
            OrientArrow(start, dest, newArrow);
        }

        public void ClearArrows() {
            foreach (GameObject arrow in hitArrows) {
                Destroy(arrow);
            }
            hitArrows.Clear();
        }

        private void OrientArrow(HexEntry start, HexEntry dest, GameObject arrow) {
            Vector2 startWorldPos = HexVectorUtil.worldPositionOfHexCoord(start.BoardPos);
            Vector2 destWorldPos = HexVectorUtil.worldPositionOfHexCoord(dest.BoardPos);

            arrow.transform.position = new Vector3(startWorldPos.x, startWorldPos.y, -3) + (Config.hitArrowPosScaler) * Vector3.Normalize(destWorldPos - startWorldPos);
            arrow.transform.rotation = Quaternion.FromToRotation(Vector2.right, destWorldPos - startWorldPos);
        }

        public void CreateFriendlyFireWarning(HexEntry hex) {
            GameObject newFFWarning = Instantiate(friendlyFireWarning, HexVectorUtil.worldPositionOfHexCoord(hex.BoardPos), Quaternion.identity);
            newFFWarning.GetComponent<SpriteRenderer>().color = Config.Palette.attack;
            newFFWarning.transform.SetParent(hitArrowContainer.transform);
            ffWarnings.Add(newFFWarning);
        }

        public void ClearFriendlyFireWarnings() {
            foreach (GameObject ffWarning in ffWarnings) {
                Destroy(ffWarning);
            }
            ffWarnings.Clear();
        }

        public void ShowRangeIndicator(IUnitController unit, HexEntry location, int range) {

            if (location == null) {
                return;
            }

            if (!rangeIndicatorSegments.ContainsKey(unit)) {
                rangeIndicatorSegments[unit] = new List<GameObject>();
            }

            List<HexEntry> hexesInRange = new List<HexEntry>();

            for (int dx = -range; dx <= range; dx++) {
                for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++) {
                    Vector2 hexPosition = new Vector2((int)location.BoardPos.x + dx, (int)location.BoardPos.y - dx - dy);
                    if (scenarioLoader.HexGrid.ContainsKey(hexPosition)) {
                        hexesInRange.Add(scenarioLoader.HexGrid[hexPosition]);
                    }
                }
            }

            foreach (HexEntry h in hexesInRange) {
                foreach (Bearing b in Enum.GetValues(typeof(Bearing))) {
                    if (!h.Neighbors.ContainsKey(b)) {
                        MarkEdge(unit, h, b);
                    }
                    else if (!hexesInRange.Contains(h.Neighbors[b])) {
                        MarkEdge(unit, h, b);
                    }
                }
            }
        }

        public void ClearRangeIndicator(IUnitController unit) {
            if (rangeIndicatorSegments.ContainsKey(unit)) {
                foreach (GameObject segment in rangeIndicatorSegments[unit]) {
                    Destroy(segment);
                }
                rangeIndicatorSegments[unit].Clear();
            }
        }

        private void MarkEdge(IUnitController unit, HexEntry hex, Bearing b) {
            Vector2 cornerSpot = new Vector2();
            Vector2 rotationVec = new Vector2();

            switch (b) {
                case Bearing.E:
                    cornerSpot = FindCorner(hex, -30);
                    rotationVec = Vector2.down;
                    break;
                case Bearing.NNE:
                    cornerSpot = FindCorner(hex, -90);
                    rotationVec = new Vector2(1.732f, -1);
                    break;
                case Bearing.NNW:
                    cornerSpot = FindCorner(hex, -150);
                    rotationVec = new Vector2(1.732f, 1);
                    break;
                case Bearing.W:
                    cornerSpot = FindCorner(hex, -210);
                    rotationVec = Vector2.up;
                    break;
                case Bearing.SSW:
                    cornerSpot = FindCorner(hex, -270);
                    rotationVec = new Vector2(-1.732f, 1);
                    break;
                case Bearing.SSE:
                    cornerSpot = FindCorner(hex, -330);
                    rotationVec = new Vector2(-1.732f, -1);
                    break;
            }
            GameObject segment = Instantiate(rangeIndicator, new Vector3(cornerSpot.x, cornerSpot.y, 3), Quaternion.FromToRotation(Vector2.right, rotationVec));
            segment.transform.SetParent(rangeIndicatorContainer.transform);
            segment.GetComponent<SpriteRenderer>().color = Config.Palette.PlayerColor(unit.PlayerOwner);
            
            rangeIndicatorSegments[unit].Add(segment);
        }

        private Vector2 FindCorner(HexEntry hex, float angle) {
            return HexVectorUtil.worldPositionOfHexCoord(hex.BoardPos) + (Vector2)(Quaternion.AngleAxis(angle, Vector3.back) * Vector3.right * (Config.hexSize));
        }
    }
}