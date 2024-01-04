using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Quest")] public class Quest : ScriptableObject {
    private QuestState _currentQuestState = QuestState.Inactive;
    public QuestState CurrentQuestState {
        get => _currentQuestState;
        set {
            if (value == QuestState.Completed && PlayTextOnComplete && value != _currentQuestState) 
                QuestManager.Instance.StartQuestTimer(this);
            
            _currentQuestState = value;
            if (value == QuestState.Active) QuestManager.Instance.StartQuestTimer(this);
        }
    }

    public void Initialize(InteractableObject interactable) => Interactable = interactable;
    
    [TextArea] public string Hint;
    [field: SerializeField] public float HintTime { get; set; }
    [field: SerializeField] public bool PlayTextOnComplete { get; private set; }
    [field: SerializeField] public Quest LinkedQuest { get; private set; }
    [field: SerializeField] public List<Quest> QuestPrerequisites { get; private set; }
    [field: SerializeField] public InteractableObject Interactable { get; private set; }
    
    public void UpdateQuestState() {
        if (QuestPrerequisites is not { Count: > 0 }) return;
        if (!QuestPrerequisites.TrueForAll(quest => quest.CurrentQuestState is QuestState.Completed)) return;
        if(CurrentQuestState is QuestState.Inactive) CurrentQuestState = QuestState.Active;
    }
    
    public void CompleteQuest() { 
        if (CurrentQuestState is QuestState.Active) CurrentQuestState = QuestState.Waiting;

        if (LinkedQuest is not null) {
            if (LinkedQuest.CurrentQuestState is QuestState.Waiting) {
                LinkedQuest.CurrentQuestState = QuestState.Completed;
                CurrentQuestState = QuestState.Completed;
            }
        }
        else CurrentQuestState = QuestState.Completed;
        
        QuestManager.Instance.UpdateQuests(); 
    }
}

public enum QuestState {
    Inactive, Active, Completed, NotCompleted, Waiting
}