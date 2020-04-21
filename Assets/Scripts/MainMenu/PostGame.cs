﻿using Assets.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGame : MonoBehaviour
{
    [SerializeField] private GameObject LoseCanvas;
    [SerializeField] private GameObject WinCanvas;
    // Start is called before the first frame update
    void Start()
    {
        switch ((int)Suspect.CurrentAccusationState) {
            case 1:
                WinCanvas.SetActive(true);
                break;
            default:
                LoseCanvas.SetActive(true);
                break;
        }
    }

    
}
