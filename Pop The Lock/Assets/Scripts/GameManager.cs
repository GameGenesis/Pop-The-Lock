using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int currentLevel = 1;
    public static event Action<int> onTurnCounterUpdate;
    public static event Action<int> onLevelUpdate;
    public static event Action onFirstInput;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Transform lockTransform;
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
        LevelManager.levelCurve = new AnimationCurve(new Keyframe(1f, 0.2f, 0.25f, 0.02f),
                                                     new Keyframe(LevelManager.totalLevelCount, 1f));
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        lockPin = FindObjectOfType<LockPin>();
    }

    private void Start()
    {
        PanCameraEntry();
        SetUpLevel();
    }

    private void Update()
    {
        if (lockCircle == null || lockPin == null || !lockCircle.gameObject.activeSelf)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (currentLevel == 1)
                onFirstInput?.Invoke();

            if (isPinColliding)
            {
                MoveLockCircle();
                UpdateLockPinVelocity();
                UpdateCurrentTurnCount();
                AudioManager.instance.Play("Pop");
            }
            else
            {
                if (lockPin.CurrentVelocity == 0)
                {
                    lockPin.CurrentVelocity = levelProperties.startingVelocity;
                    return;
                }

                AudioManager.instance.Play("Fail");
                lockPin.CurrentVelocity = 0;
                lockCircle.gameObject.SetActive(false);
                mainCamera.backgroundColor = new Color(0.8f, 0.215f, 0.215f);
                lockAnimator.SetTrigger("Jiggle");
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

    public static void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public static void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PanCameraEntry()
    {
        mainCamera.GetComponent<Animator>().SetTrigger("PanLeft");
    }

    public void SetUpLevel()
    {
        lockAnimator.SetBool("Unlocked", false);
        levelProperties = LevelManager.GetLevelProperties(currentLevel);
        mainCamera.backgroundColor = levelProperties.levelColor;
        currentTurns = levelProperties.maxTurns;
        onTurnCounterUpdate?.Invoke(currentTurns);
        onLevelUpdate?.Invoke(currentLevel);
        ResetLockPin();
        lockCircle.gameObject.SetActive(true);
        MoveLockCircle();
    }

    private void ResetLockPin()
    {
        lockPin.transform.localPosition = new Vector2(0, 1.505f);
        lockPin.transform.localRotation = Quaternion.Euler(0, 0, 0);
        lockPin.CurrentAngle = 0;
        lockPin.CurrentVelocity = 0;
    }

    private void SetCollisionStatus(bool colliding)
    {
        isPinColliding = colliding;
    }

    private void MoveLockCircle()
    {
        float angleMultiplier = lockPin.CurrentVelocity <= 0 ? 0 : 1;
        float angle = (lockPin.CurrentAngle + (UnityEngine.Random.Range(levelProperties.minAngleOffset, levelProperties.maxAngleOffset) * angleMultiplier)) * Mathf.Deg2Rad;
        lockCircle.position = new Vector2(Mathf.Cos(angle) * lockRadius + lockTransform.position.x, Mathf.Sin(angle) * lockRadius + lockTransform.position.y);
    }

    private void UpdateLockPinVelocity()
    {
        lockPin.CurrentVelocity *= -1f;
        if (Mathf.Abs(lockPin.CurrentVelocity) < levelProperties.maxVelocity)
            lockPin.CurrentVelocity *= levelProperties.velocityMultiplier;
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
        AudioManager.instance.Play("Win");
        lockPin.CurrentVelocity = 0;
        lockCircle.gameObject.SetActive(false);
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        lockAnimator.SetBool("Unlocked", true);
        StartCoroutine(ResetScene(0.8f, true));
    }

    private IEnumerator ResetScene(float seconds, bool transition = false)
    {
        yield return new WaitForSeconds(seconds);
        if (transition)
        {
            mainCamera.GetComponent<Animator>().SetTrigger("PanRight");
        }
        else
        {
            SetUpLevel();
        }
    }
}
