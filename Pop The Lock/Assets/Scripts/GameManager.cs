using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        lockPin = FindObjectOfType<LockPin>();
        if (lockAnimator != null)
            lockAnimator.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(SetUpLevel());
    }

    private void Update()
    {
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
                StopAllCoroutines();
                StartCoroutine(ResetScene(0.7f));
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

    private IEnumerator SetUpLevel()
    {
        levelProperties = LevelManager.GetLevelProperties(currentLevel);
        Camera.main.backgroundColor = levelProperties.levelColor;
        MoveLockCircle();
        currentTurns = levelProperties.maxTurns;
        onTurnCounterUpdate?.Invoke(currentTurns);
        yield return new WaitForSeconds(0.4f);
        lockPin.CurrentVelocity = levelProperties.startingVelocity;
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
        if (lockAnimator != null)
            lockAnimator.enabled = true;
        StartCoroutine(ResetScene(1.75f));
    }

    private IEnumerator ResetScene(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
