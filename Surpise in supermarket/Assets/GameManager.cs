using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public AudioSource winAudioSource;
    public AudioClip winClip;
    public Transform mainCamera;
    public MouseLook mouseLook;

    public GameObject jumpscareMonster;
    public Transform jumpscarePoint;

    public AudioSource loseAudioSource;
    public AudioClip loseClip;

    public static GameManager Instance;

    public float winTime = 60f;
    private float currentTime = 0f;
    private bool gameEnded = false;

    public TextMeshProUGUI timerText;
    public GameObject winText;
    public GameObject loseText;

    private ScreenFade screenFade;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f;

        screenFade = FindFirstObjectByType<ScreenFade>();

        if (screenFade != null)
            screenFade.SetAlphaInstant(0f);

        if (winText != null)
            winText.SetActive(false);

        if (loseText != null)
            loseText.SetActive(false);

        if (jumpscareMonster != null)
            jumpscareMonster.SetActive(false);
    }

    void Update()
    {
        if (gameEnded)
            return;

        currentTime += Time.deltaTime;

        float timeLeft = Mathf.Max(0f, winTime - currentTime);

        if (timerText != null)
            timerText.text = "ПРЯЧЬСЯ\n       " + Mathf.CeilToInt(timeLeft).ToString();

        if (currentTime >= winTime)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        AudioListener.volume = 0f;

        if (winAudioSource != null && winClip != null)
        {
            winAudioSource.ignoreListenerVolume = true;
            winAudioSource.PlayOneShot(winClip, 1.5f);
        }

        if (screenFade != null)
            screenFade.SetAlphaInstant(0.75f);

        if (winText != null)
            winText.SetActive(true);

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        Debug.Log("YOU WIN");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        SnapCameraToJumpscarePoint();

        ShowJumpscare();

        AudioListener.volume = 0f;

        if (loseAudioSource != null && loseClip != null)
        {
            loseAudioSource.ignoreListenerVolume = true;
            loseAudioSource.PlayOneShot(loseClip, 2f);
        }

        if (screenFade != null)
            screenFade.SetAlphaInstant(0.2f);

        if (loseText != null)
            loseText.SetActive(true);

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        Debug.Log("YOU LOSE");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    void SnapCameraToJumpscarePoint()
    {
        if (mainCamera == null || jumpscarePoint == null)
            return;

        if (mouseLook != null)
            mouseLook.enabled = false;

        Vector3 direction = jumpscarePoint.position - mainCamera.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            mainCamera.rotation = Quaternion.LookRotation(direction);
        }
    }

    void ShowJumpscare()
    {
        if (jumpscareMonster == null || jumpscarePoint == null)
            return;

        jumpscareMonster.SetActive(true);
        jumpscareMonster.transform.position = jumpscarePoint.position;
        jumpscareMonster.transform.rotation = jumpscarePoint.rotation;
    }
}