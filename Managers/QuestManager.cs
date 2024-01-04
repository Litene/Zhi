using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;




public class QuestManager : GenericSingleton<QuestManager> {
    [SerializeField] private GameObject _textCanvas;
    private delegate void QuestBind();
    [SerializeField] private List<QuestBind> _questsUpdates;
    private readonly Queue<Quest> _questTextToDisplay = new();
    private TextMeshProUGUI _textBox;
    private bool _isDisplayingText;

    #region Consts
    private const string QUEST_SO = "QuestSO";
    private const string TEXT_BOX = "Textbox";
    private const string SCREEN_TEXT = "Screentext";
    private const string TEXT_PANEL = "TextPanel";
    private const int HINT_TIME_MUTLIPLIER = 60;
    #endregion
    private void Awake() {
        LoadQuests();
        SetUpdateDelegate();
        _textBox ??= GameObject.Find(TEXT_BOX).GetComponent<TextMeshProUGUI>();
        _textCanvas ??= GameObject.Find(SCREEN_TEXT).transform.Find(TEXT_PANEL).gameObject;
        _textCanvas.SetActive(false);
    }
    
    [SerializeField] private List<Quest> _quests;
    private void LoadQuests() => _quests = Resources.LoadAll<Quest>(QUEST_SO).ToList();
    private void SetUpdateDelegate() => _questsUpdates = (from quest in _quests select new QuestBind(quest.UpdateQuestState)).ToList();
    public void UpdateQuests() => _questsUpdates.ForEach(quest => quest?.Invoke());
    private IEnumerator QuestTimer(Quest quest) {  
        yield return new WaitForSeconds(quest.HintTime * HINT_TIME_MUTLIPLIER);
        _questTextToDisplay.Enqueue(quest);
        if (_isDisplayingText) yield break;
        if (quest.CurrentQuestState == QuestState.Active || quest.PlayTextOnComplete) 
            StartCoroutine(PrintHint(_questTextToDisplay.Dequeue().Hint));
    }
    public void StartQuestTimer(Quest quest) {
        if (quest.HintTime == 0) return;

        StartCoroutine(QuestTimer(quest)); 
    }
    private IEnumerator PrintHint(string hint) { 
        _isDisplayingText = true;
        _textCanvas.SetActive(true);
        StringBuilder builder = new();
        for (var i = 0; i < hint.Length; i++) {
            builder.Append(hint[i]);
            _textBox.text = builder.ToString();
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(3); 
        _textBox.text = "";

        _isDisplayingText = false;
        if (_questTextToDisplay.Count == 0) _textCanvas.SetActive(false);
        else StartQuestTimer(_questTextToDisplay.Dequeue());
    }
    
#if UNITY_EDITOR
    private void Update() {
        if (Input.GetKey(KeyCode.J) && Input.GetKeyDown(KeyCode.K)) {
            foreach (var quest in _quests) {
                quest.CurrentQuestState = QuestState.Inactive;
                if (quest.name == "FronDoorButtonHuman" || quest.name == "FrontDoorButtonGhost" ||
                    quest.PlayTextOnComplete) {
                    quest.CurrentQuestState = QuestState.Active;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            UpdateQuests();
        }
    }
#endif
}
public enum GameState {
    NormalState,
    PuzzleState,
    TransitionToPuzzle,
    TransitionToNormal
}