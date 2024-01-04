using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OutlineScript : MonoBehaviour {
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float outlineScaleFactor;
    [SerializeField] private Color outlineColor;
    private Renderer _outlineRenderer;
    private Renderer _originalRenderer;
    private Material _originalMat;

    private void Awake() {
     

      
    }


    [CanBeNull]
    

    private void RemoveOutLine() {
    }

  
}