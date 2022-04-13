using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI turnCounter;

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
}
