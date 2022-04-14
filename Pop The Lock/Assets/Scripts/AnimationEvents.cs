using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private void SetUpLevel()
    {
        gameManager.SetUpLevel();
    }
}
