using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro turnCounter;

    [SerializeField]
    private TextMeshProUGUI levelCounter;

    [SerializeField]
    private GameObject helpText;

    private void Awake()
    {
        if (GameManager.currentLevel == 1)
        {
            helpText.SetActive(true);
            helpText.GetComponent<Animator>().enabled = false;
        }
        else
        {
            helpText.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameManager.onTurnCounterUpdate += UpdateTurnCounter;
        GameManager.onFirstInput += DisableHelpText;
        GameManager.onLevelUpdate += UpdateLevelCounter;
    }

    private void OnDisable()
    {
        GameManager.onTurnCounterUpdate -= UpdateTurnCounter;
        GameManager.onFirstInput -= DisableHelpText;
        GameManager.onLevelUpdate -= UpdateLevelCounter;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        GameManager.ReloadScene();
    }

    private void UpdateTurnCounter(int turnCount)
    {
        turnCounter.text = turnCount.ToString();
    }

    private void UpdateLevelCounter(int level)
    {
        levelCounter.text = $"Level {level}";
    }

    public void DisableHelpText()
    {
        helpText.GetComponent<Animator>().enabled = true;
    }

}
