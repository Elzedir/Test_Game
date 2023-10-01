using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Menu_CharacterSelect : Menu_UI
{
    public static Menu_CharacterSelect Instance;

    private Actor_Base _selectedActor;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (_isOpen)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                if (hit.collider != null && hit.collider.gameObject.TryGetComponent<Actor_Base>(out Actor_Base actor))
                {
                    _selectedActor = actor;
                    OnCharacterSelect();
                }

                if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Camera_Move_Up]))
                {
                    CameraMotor.Instance.ManualMove(Vector2.up);
                }
                if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Camera_Move_Down]))
                {
                    CameraMotor.Instance.ManualMove(Vector2.down);
                }
                if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Camera_Move_Left]))
                {
                    CameraMotor.Instance.ManualMove(Vector2.left);
                }
                if (Input.GetKey(Manager_Input.Instance.KeyBindings.Keys[ActionKey.Camera_Move_Right]))
                {
                    CameraMotor.Instance.ManualMove(Vector2.right);
                }
            }
        }
    }

    public override void OpenMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(true);
        _isOpen = true;
        GameManager.Instance.ChangeState(GameState.Paused);

        foreach (Actor_Base actor in Manager_Actors.Instance.AllActorsList)
        {
            actor.Highlight();
        }

        CameraMotor.Instance.GetComponent<Camera>().orthographicSize *= 3;
        CameraMotor.Instance.PlayerCameraEnabled = false;
    }

    public void OnCharacterSelect()
    {
        GameManager.Instance.FindTransformRecursively(this.transform, "CharacterIcon").GetComponent<Image>().sprite = _selectedActor.GetComponent<SpriteRenderer>().sprite;
        GameManager.Instance.FindTransformRecursively(this.transform, "CharacterNameText").GetComponent<TextMeshProUGUI>().text = _selectedActor.gameObject.name;
    }

    public override void CloseMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(false);
        _isOpen = false;
        GameManager.Instance.ChangeState(GameState.Playing);

        foreach (Actor_Base actor in Manager_Actors.Instance.AllActorsList)
        {
            actor.RemoveHighlight();
        }

        CameraMotor.Instance.GetComponent<Camera>().orthographicSize /= 3;
        CameraMotor.Instance.PlayerCameraEnabled = true;
    }


    public void SelectCharacterButtonPress()
    {
        _selectedActor.SetToPlayer();
        GameManager.Instance.Player.PlayerActor.SetToNPC();
    }
}
