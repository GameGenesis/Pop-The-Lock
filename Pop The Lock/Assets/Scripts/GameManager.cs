using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int currentLevel = 1;
    public static event Action<int> onTurnCounterUpdate;

    [SerializeField]
    private Animator lockAnimator;
    [SerializeField]
    private Transform lockCircle;
    [SerializeField]
    private float lockRadius;

    private LockPin lockPin;
    private bool isPinColliding;

    private LevelProperties levelProperties;

    private int currentTurns;

    private bool hasWon = false;
    private bool hasLost = false;

    private void Awake()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        lockPin = FindObjectOfType<LockPin>();
        if (lockAnimator != null)
            lockAnimator.enabled = false;
    }

    private void Start()
    {
        SetUpLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if ((hasWon || hasLost) && Input.anyKeyDown)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (lockCircle == null || lockPin == null || !lockCircle.gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPinColliding)
            {
                MoveLockCircle();
                UpdateLockPinVelocity();
                UpdateCurrentTurnCount();
            }
            else
            {
                lockPin.CurrentVelocity = 0;
                lockCircle.gameObject.SetActive(false);
                Camera.main.backgroundColor = new Color(0.8f, 0.215f, 0.215f);
                hasLost = true;
            }
        }
    }

    private void OnEnable()
    {
        if (lockPin != null)
            lockPin.onLockPinCollision += SetCollisionStatus;
    }

    private void OnDisable()
    {
        if (lockPin != null)
            lockPin.onLockPinCollision -= SetCollisionStatus;
    }

    private void SetUpLevel()
    {
        levelProperties = LevelManager.GetLevelProperties(currentLevel);
        Debug.Log(levelProperties.levelColor);
        Camera.main.backgroundColor = levelProperties.levelColor;
        lockPin.CurrentVelocity = levelProperties.startingVelocity;
        MoveLockCircle();
        currentTurns = levelProperties.maxTurns;
        onTurnCounterUpdate?.Invoke(currentTurns);
    }

    private void SetCollisionStatus(bool colliding)
    {
        isPinColliding = colliding;
    }

    private void MoveLockCircle()
    {
        float angleMultiplier = lockPin.CurrentVelocity <= 0 ? 0 : 1;
        float angle = (lockPin.CurrentAngle + (UnityEngine.Random.Range(levelProperties.minAngleOffset, levelProperties.maxAngleOffset) * angleMultiplier)) * Mathf.Deg2Rad;
        lockCircle.position = new Vector2(Mathf.Cos(angle) * lockRadius, Mathf.Sin(angle) * lockRadius);
    }

    private void UpdateLockPinVelocity()
    {
        if (Mathf.Abs(lockPin.CurrentVelocity) < levelProperties.maxVelocity)
            lockPin.CurrentVelocity *= -1.05f;
        else
            lockPin.CurrentVelocity *= -1f;
    }

    private void UpdateCurrentTurnCount()
    {
        currentTurns--;
        onTurnCounterUpdate?.Invoke(currentTurns);
        if (currentTurns <= 0)
            Win();
    }

    private void Win()
    {
        lockPin.CurrentVelocity = 0;
        lockCircle.gameObject.SetActive(false);
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        hasWon = true;
        if (lockAnimator != null)
            lockAnimator.enabled = true;
    }
}
