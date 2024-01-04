using Highlighters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))] public class InteractableObject : MonoBehaviour {

	[SerializeField] protected CameraMode _cameraMode;
	[SerializeField] protected bool _isHighlightable;
	public bool HideInteraction;
	private Player _interactionPlayer;
	private LayerMask _mask;


	[SerializeField, Tooltip("If cameraType is Puzzlemode")]
	protected Transform _cameraTransform;

	[SerializeField] protected List<UnityEvent> events;

	protected MeshRenderer _meshRenderer;
	protected Collider _collider;
	public bool TriggerEventOnInteract;
	[SerializeField] protected GameObject[] _renderObject;
	private void Awake() {
		if (Quest) Quest.Initialize(this);

		if (_renderObject is { Length: > 0 }) _mask = _renderObject[0].gameObject.layer;

		_meshRenderer = GetComponentInChildren<MeshRenderer>();
		_collider = GetComponent<Collider>();
	}
	[field: SerializeField] protected Quest Quest { get; set; }
	[field: SerializeField] protected PlayerType InteractTarget { get; set; }
	private int GetMask() {
		return InteractTarget switch {
			PlayerType.Both => 13,
			PlayerType.Ghost => 14,
			PlayerType.Human => 15,
			_ => LayerMask.GetMask("Default")
		};
	}
	private void InvokeEvents() => events.ForEach(ev => ev?.Invoke());
	public void MakeInteractable() => HideInteraction = false;
	public void Interact(Player player, PlayerType type) {
		if (type != InteractTarget && InteractTarget is not PlayerType.Both) return;

		if (_interactionPlayer is not null) return;

		if (player.CurrentGameState is GameState.TransitionToPuzzle 
		    or GameState.TransitionToNormal) return;

		if (TriggerEventOnInteract) InvokeEvents(); 

		if (_cameraMode is CameraMode.Puzzle) {
			player.UpdateCameraTransform(_cameraTransform);
			EnterPuzzleCameraMode(player);
		}

		InteractAction(player);
	}

	public void StopInteract(Player player, PlayerType type) {
		if (_interactionPlayer != player) return;

		if (player.CurrentGameState == GameState.TransitionToPuzzle ||
		    player.CurrentGameState == GameState.TransitionToNormal)
			return;

		if (_cameraMode == CameraMode.Puzzle) {
			player.UpdateCameraTransform(_cameraTransform);
			ExitPuzzleCameraMode(player);
			StopInteractAction(player);
		}
	}

	protected virtual void InteractAction(Player player) { }

	protected virtual void StopInteractAction(Player player) { }

	protected virtual void StopInteractAction() { }

	public void CompleteQuest() => Quest.CompleteQuest();
	public bool IsVisible() => _meshRenderer.isVisible;

	public bool ComparePlayerType(PlayerType type) {
		if (InteractTarget == PlayerType.Both)
			return true;

		return type == InteractTarget;
	}

	public bool InLineOfSight(Player player, LayerMask blockingLayers) =>
		!Physics.Linecast(player.transform.position, transform.position, blockingLayers);

	#region Camera modes
	
	protected void EnterPuzzleCameraMode(Player player) {
		player.InteractionComponent.CanToggle = false;
		_interactionPlayer = player;

		ToInteraction(player);
	}

	protected void ExitPuzzleCameraMode(Player player) {
		FromInteraction(player);

		player.InteractionComponent.CanToggle = true;
		_isHighlightable = true;
		_interactionPlayer = null;
	}

	private void ToInteraction(Player player) {
		if (_cameraMode == CameraMode.Puzzle && player.CurrentGameState == GameState.NormalState) 
			player.CurrentGameState = GameState.TransitionToPuzzle;

		if (Quest is null || Quest.CurrentQuestState != QuestState.Completed) return;
		
		if (events == null || events.Count < 1) return; 
		foreach (var gameEvent in events) {
			gameEvent?.Invoke();
		}
	}

	private void FromInteraction(Player player) {
		if (player.CurrentGameState == GameState.PuzzleState) {
			player.CurrentGameState = GameState.TransitionToNormal;
		}
	}

	#endregion

	#region HighLight

	

	

	public void Outline() {
		//if (!_isHighlightable) return;
	}

	public void StopOutline() { }

	public void Highlight() {
		if (_renderObject == null || _renderObject.Length == 0) return;

		if (!_isHighlightable) return;
		
		foreach (var renderObject in _renderObject) 
			renderObject.layer = GetMask();
	}

	public void StopHighlight() {
		if (_renderObject == null || _renderObject.Length == 0) return;

		foreach (var renderObject in _renderObject) 
			renderObject.layer = _mask;
	}

	public void DisableHighlighter() {
		HighlightManager.Instance.RemoveFromHighlight(gameObject);
		HighlightManager.Instance.RemoveFromOutline(gameObject);
	}
	#endregion
}

	

public enum PlayerType {
	Ghost,
	Human,
	Both
}

public enum CameraMode {
	Normal,
	Puzzle
}