using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePointDisplayer : MonoBehaviour {
    
    [SerializeField]
    GameObject point1;
    [SerializeField]
    GameObject point2;
    [SerializeField]
    GameObject point3;
    [SerializeField]
    GameObject point4;
    [SerializeField]
    GameObject point5;
    [SerializeField]
    GameObject point6;

    public void SetPointDisplay(int numPoints) {
        
        if (numPoints < 1) {
            point1.SetActive(false);
        }
        else {
            point1.SetActive(true);
        }
        if (numPoints < 2) {
            point2.SetActive(false);
        }
        else {
            point2.SetActive(true);
        }
        if (numPoints < 3) {
            point3.SetActive(false);
        }
        else {
            point3.SetActive(true);
        }
        if (numPoints < 4) {
            point4.SetActive(false);
        }
        else {
            point4.SetActive(true);
        }
        if (numPoints < 5) {
            point5.SetActive(false);
        }
        else {
            point5.SetActive(true);
        }
        if (numPoints < 6) {
            point6.SetActive(false);
        }
        else {
            point6.SetActive(true);
        }
    }
}
