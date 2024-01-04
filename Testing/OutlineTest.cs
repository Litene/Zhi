using System;
using System.Collections;
using System.Collections.Generic;
using Highlighters;
using UnityEngine;

public class OutlineTest : MonoBehaviour {
    [SerializeField] private Highlighter _highlighter;
    [SerializeField] private HighlighterSettings _settings;

    private void OnValidate() {
        // if (_highlighter == null) {
        //     _highlighter = GetComponent<Highlighter>();
        // }

        // if (_settings == null) {
        //     _settings = _highlighter.Settings;
        // }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            _highlighter.Settings.MeshOutlineBack.Color = new Color(1, 1, 1, 255f);
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            _highlighter.Settings.MeshOutlineBack.Color = new Color(0, 0, 0, 0);
            
        }
    }
}