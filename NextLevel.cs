using UnityEngine;

public class NextLevel : Hitbox
{
    public string[] sceneNames;
    protected BoxCollider2D levelColl;
    protected override BoxCollider2D Coll => levelColl;

    protected override void Start()
    {
        base.Start();
        levelColl = GetComponent<BoxCollider2D>();
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            GameManager.instance.SaveState();
            string sceneName = sceneNames[Random.Range(0,sceneNames.Length)];
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
