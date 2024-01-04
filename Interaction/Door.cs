using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class Door : InteractableObject {
	public Animator Anim { get; private set; }
	[field: SerializeField] public List<Lock> Locks { get; private set; } 
	protected override void InteractAction(Player player) {
		if (player is not null)
			OpenDoor(player.InventoryComponent);
	}
	public void OpenDoor(PlayerInventory inventory) {
		if (TryUnlock(inventory)) {
			InvokeEvents();
			Anim.SetTrigger("DoorOpen");
			AudioManager.Instance.Play("DoorOpen", transform.position);
			Quest.CompleteQuest();
		}
	}
	private bool TryUnlock(PlayerInventory inventory) {
		if (Locks.Count < 1 || Locks is null) return true;

		(from doorLock in Locks
				where doorLock.TypeOfLock is LockType.Quest || inventory.HeldItem is not null
				select doorLock).ToList()
			.ForEach(doorLock => doorLock.TestUnlock(doorLock.TypeOfLock is LockType.Quest ? null : inventory));

		return Locks.TrueForAll(doorLock => doorLock.Unlocked);
	}
	private void InvokeEvents() => events.ForEach(ev => ev?.Invoke());
	private void Start() => Anim = GetComponent<Animator>();
}

[Serializable] public class Lock {
	[field: SerializeField] public Quest QuestToUnlock { get; private set; }
	[field: SerializeField] public bool Unlocked { get; private set; }
	[field: SerializeField] public LockType TypeOfLock { get; private set; }
	[field: SerializeField] public PickUpable Key { get; private set; }
	[field: SerializeField] public List<UnityEvent> OnUnlockEvents { get; private set; }
	public void InvokeEvents() => OnUnlockEvents.ForEach(ev => ev?.Invoke());
	private bool CanUnlock(PlayerInventory inventory) => TypeOfLock == LockType.Quest && QuestToUnlock.CurrentQuestState is QuestState.Completed 
	                                                     || inventory is not null && inventory.HeldItem == Key;
	public void TestUnlock(PlayerInventory inventory) {
		if (CanUnlock(inventory)) {
			Unlocked = true;
			InvokeEvents();
			if (inventory.HeldItem == Key) 
				inventory.UseInventoryObject();
		}
	}
}

public enum LockType {
	Quest,
	Key
}