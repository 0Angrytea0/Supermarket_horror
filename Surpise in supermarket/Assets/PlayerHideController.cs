using UnityEngine;

public class PlayerHideController : MonoBehaviour
{
    public bool isHidden = false;

    public float maxHideTime = 15f;
    public float shakeStartTime = 10f;
    public float currentHideTime = 0f;

    [Header("Hide Audio")]
    public AudioSource breathingAudioSource;
    public AudioClip breathingClip;
    public float breathingVolume = 0.25f;

    public AudioSource heartbeatAudioSource;
    public AudioClip heartbeatClip;
    public float heartbeatVolume = 0.35f;

    private HideSpot currentHideSpot;
    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private MouseLook mouseLook;
    private CameraShake cameraShake;
    private ScreenFade screenFade;

    private bool heartbeatStarted = false;

    public GameObject interactText;
    

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();

        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            cameraShake = cam.GetComponent<CameraShake>();
        }

        if (interactText != null)
            interactText.SetActive(false);

        screenFade = FindFirstObjectByType<ScreenFade>();

        if (breathingAudioSource != null)
        {
            breathingAudioSource.loop = true;
            breathingAudioSource.playOnAwake = false;
            breathingAudioSource.spatialBlend = 0f;
            breathingAudioSource.volume = breathingVolume;
        }

        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.loop = true;
            heartbeatAudioSource.playOnAwake = false;
            heartbeatAudioSource.spatialBlend = 0f;
            heartbeatAudioSource.volume = heartbeatVolume;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHidden && currentHideSpot != null && currentHideSpot.playerInRange)
            {
                EnterHideSpot();
            }
            else if (isHidden)
            {
                ExitHideSpot();
            }
        }

        if (isHidden)
        {
            currentHideTime += Time.deltaTime;

            if (currentHideTime >= shakeStartTime && cameraShake != null)
            {
                cameraShake.StartShake();

                if (!heartbeatStarted)
                {
                    StartHeartbeat();
                }
            }

            if (currentHideTime >= maxHideTime)
            {
                Debug.Log("Too long in hide spot!");
                ExitHideSpot();
            }
        }

        if (!isHidden && currentHideSpot != null && currentHideSpot.playerInRange)
        {
            if (interactText != null && !interactText.activeSelf)
                interactText.SetActive(true);
        }
        else
        {
            if (interactText != null && interactText.activeSelf)
                interactText.SetActive(false);
        }
    }

    void EnterHideSpot()
    {
        isHidden = true;
        currentHideTime = 0f;
        heartbeatStarted = false;

        if (currentHideSpot != null)
            currentHideSpot.PlayEnterSound();

        StartBreathing();
        StopHeartbeat();
        if (interactText != null)
        interactText.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (cameraShake != null)
            cameraShake.StopShake();

        if (screenFade != null)
            screenFade.SetTargetAlpha(0.35f);

        if (characterController != null)
            characterController.enabled = false;

        transform.position = currentHideSpot.hidePoint.position;

        if (characterController != null)
            characterController.enabled = true;
    }

    void ExitHideSpot()
    {
        isHidden = false;
        currentHideTime = 0f;
        heartbeatStarted = false;

        if (currentHideSpot != null)
            currentHideSpot.PlayExitSound();

        StopBreathing();
        StopHeartbeat();

        if (cameraShake != null)
            cameraShake.StopShake();

        if (screenFade != null)
            screenFade.SetTargetAlpha(0f);

        if (characterController != null)
            characterController.enabled = false;

        transform.position = currentHideSpot.enterPoint.position;

        if (characterController != null)
            characterController.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    void StartBreathing()
    {
        if (breathingAudioSource == null || breathingClip == null)
            return;

        if (breathingAudioSource.clip != breathingClip)
            breathingAudioSource.clip = breathingClip;

        breathingAudioSource.volume = breathingVolume;

        if (!breathingAudioSource.isPlaying)
            breathingAudioSource.Play();
    }

    void StopBreathing()
    {
        if (breathingAudioSource != null && breathingAudioSource.isPlaying)
            breathingAudioSource.Stop();
    }

    void StartHeartbeat()
    {
        heartbeatStarted = true;

        if (heartbeatAudioSource == null || heartbeatClip == null)
            return;

        if (heartbeatAudioSource.clip != heartbeatClip)
            heartbeatAudioSource.clip = heartbeatClip;

        heartbeatAudioSource.volume = heartbeatVolume;

        if (!heartbeatAudioSource.isPlaying)
            heartbeatAudioSource.Play();
    }

    void StopHeartbeat()
    {
        if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying)
            heartbeatAudioSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        HideSpot hideSpot = other.GetComponent<HideSpot>();
        if (hideSpot != null)
        {
            currentHideSpot = hideSpot;
            hideSpot.playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HideSpot hideSpot = other.GetComponent<HideSpot>();
        if (hideSpot != null)
        {
            hideSpot.playerInRange = false;

            if (!isHidden)
                currentHideSpot = null;
        }
    }
}