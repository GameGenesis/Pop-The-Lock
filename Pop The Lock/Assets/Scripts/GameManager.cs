using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int currentLevel = 1;
    public static event Action<int> onTurnCounterUpdate;

    [SerializeField]
    private Transform lockCircle;
    [SerializeField]
    private float lockRadius;

    private LockPin lockPin;
    private bool isPinColliding;

    private LevelProperties levelProperties;

    private int currentTurns;

    private void Awake()
    {
        lockPin = FindObjectOfType<LockPin>();
    }

    private void Start()
    {
        SetUpLevel();
    }

    private void Update()
    {
        if (lockCircle == null || lockPin == null)
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    }

    private void UpdateCurrentTurnCount()
    {
        currentTurns--;
        onTurnCounterUpdate?.Invoke(currentTurns);
        if (currentTurns <= 0)
        {
            currentLevel++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
