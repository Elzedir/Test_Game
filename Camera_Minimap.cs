using UnityEngine;

public class Camera_Minimap : MonoBehaviour
{
    public Player _player;
    public RectTransform icon;

    void Update()
    {
        _player = GameManager.Instance.Player;
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
    }
}
