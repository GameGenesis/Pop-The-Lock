using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI turnCounter;

    [SerializeField]
    private TextMeshProUGUI levelCounter;

    private void Awake()
    {
        UpdateLevelCounter(GameManager.currentLevel);
    }

    private void OnEnable()
    {
        GameManager.onTurnCounterUpdate += UpdateTurnCounter;
    }

    private void OnDisable()
    {
        GameManager.onTurnCounterUpdate -= UpdateTurnCounter;
    }

    public void UpdateTurnCounter(int turnCount)
    {
        turnCounter.text = turnCount.ToString();
    }
    public void UpdateLevelCounter(int level)
    {
        levelCounter.text = $"Level {level}";
    }

}
