using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Switch : MonoBehaviour {
    public bool IncludedInPuzzle;
    public bool Activated = false; //For later use
    public int CorrespondingNumber;
    public MeshRenderer CorrespondingLight;
    private LayerMask _layer;

    [SerializeField] private PlayerType _type;
    [SerializeField] private Material _redLight;
    [SerializeField] private Material _greenLight;

    [SerializeField] private Animator _anim;

    private void Awake() {
        _anim = GetComponent<Animator>();
        CorrespondingLight.material = _redLight;
        _layer = gameObject.layer;
    }

    public void Activate() {
        Activated = true;
        CorrespondingLight.material = _greenLight;
        _anim.SetBool("Activated", Activated);
        AudioManager.Instance.Play("SwitchSound1", transform.position);
    }

    public void Deactivate() {
        Activated = false;
        CorrespondingLight.material = _redLight;
        _anim.SetBool("Activated", Activated);
        AudioManager.Instance.Play("SwitchSound1", transform.position);
    }

    public void Outline() {
        if (_type == PlayerType.Ghost) {
            gameObject.layer = 14;
        }
        else if (_type == PlayerType.Human) {
            gameObject.layer = 15;
        }
        else {
            gameObject.layer = 13;
        }
    }

    public void StopOutline() {
        gameObject.layer = _layer;
    }
}