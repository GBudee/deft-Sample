using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteEditor : MonoBehaviour {

    [SerializeField]
    Color background;
    [SerializeField]
    Color playerOne;
    [SerializeField]
    Color playerTwo;
    [SerializeField]
    Color attack;
    [SerializeField]
    Color unitIconFill;
    [SerializeField]
    Color border;
    [SerializeField]
    Color terrainIconFill;
    [SerializeField]
    Color normalTerrain;
    [SerializeField]
    Color roadTerrain;
    [SerializeField]
    Color forestTerrain;
    [SerializeField]
    Color hillsTerrain;
    [SerializeField]
    Color wallsTerrain;
    [SerializeField]
    Color goalTerrain;
    [SerializeField]
    Color pathArrow;
    [SerializeField]
    Color pathFootstep;
    [SerializeField]
    Color healthBar;
    [SerializeField]
    Color hexReachableColor;
    [SerializeField]
    Color hexHoverColor;
    [SerializeField]
    float hexSpacing;
    [SerializeField]
    bool activePalette;


    void OnValidate() {

        if (!activePalette) {
            return;
        }

        Deft.Config.Palette.background = background;
        Deft.Config.Palette.SetPlayerColor(1, playerOne);
        Deft.Config.Palette.SetPlayerColor(2, playerTwo);
        Deft.Config.Palette.attack = attack;
        Deft.Config.Palette.unitIconFill = unitIconFill;
        Deft.Config.Palette.border = border;
        Deft.Config.Palette.terrainIconFill = terrainIconFill;
        Deft.Config.Palette.normalTerrain = normalTerrain;
        Deft.Config.Palette.roadTerrain = roadTerrain;
        Deft.Config.Palette.forestTerrain = forestTerrain;
        Deft.Config.Palette.hillsTerrain = hillsTerrain;
        Deft.Config.Palette.wallTerrain = wallsTerrain;
        Deft.Config.Palette.goalTerrain = goalTerrain;
        Deft.Config.Palette.pathArrow = pathArrow;
        Deft.Config.Palette.pathFootstep = pathFootstep;
        Deft.Config.Palette.healthBar = healthBar;
        Deft.Config.Palette.hexReachableColor = hexReachableColor;
        Deft.Config.Palette.hexHoverColor = hexHoverColor;

        GameObject.Find("GameController").GetComponent<Deft.GameManager>().SetPalette();
        /*
        if (hexSpacing != Deft.Config.hexSize) {
            GameObject.Find("GameController").GetComponent<Deft.GameManager>().SetHexSpacing(hexSpacing);
        }*/
    }
}
