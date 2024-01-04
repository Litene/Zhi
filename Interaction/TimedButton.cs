using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedButton : InteractableObject {
    public AudioClip Sound;
    public float TimeActive = 5;
    public bool Activated;
    private float _timer;

    protected override void InteractAction(Player player) {
        if (Quest != null) Quest.CompleteQuest();
        
        events.ForEach(buttonEvent => buttonEvent?.Invoke());

        Activated = true;
    }

    private void Update() {
        if (!Activated) return;
        _timer += Time.deltaTime;
        if (_timer > TimeActive) {
            if (Quest == null) return;
            
            if (Quest.LinkedQuest.CurrentQuestState != QuestState.Completed) {
                Quest.CurrentQuestState = QuestState.Active;
                Quest.LinkedQuest.CurrentQuestState = QuestState.Active;
            }

            Activated = false;
        }
    }
}