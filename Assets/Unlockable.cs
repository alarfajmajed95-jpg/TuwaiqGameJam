using UnityEngine;
using System.Collections;

public class Unlockable : MonoBehaviour
{
    public float interactRange = 2f;

    [Header("Unlock Actions")]
    public GameObject objectToActivate;        
    public GameObject objectToActivateAlso;    
    public GameObject objectToDeactivate;      

    [Header("Main Object Lerp")]
    [SerializeField] private float lerpDuration = 3f;

    [Header("Flag Lerp")]
    [SerializeField] private GameObject flag;
    [SerializeField] private float flagHeightIncrease = 3f;
    [SerializeField] private float flagLerpDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // القديم
    [SerializeField] private AudioClip newAudio;      // الجديد

    Transform player;
    bool isUnlocked = false;

    // ===== START TRANSFORM =====
    private Vector3 startPosition = new Vector3(
        -6.880000114440918f,
        11.600000381469727f,
        -14.220000267028809f
    );

    private Quaternion startRotation = new Quaternion(
        0.1872251033782959f,
        -0.04729415476322174f,
        0.009000050835311413f,
        0.9811366200447083f
    );

    // ===== MID TRANSFORM =====
    private Vector3 midPosition = new Vector3(
        -7.320000171661377f,
        6.079999923706055f,
        2.6500000953674318f
    );

    private Quaternion midRotation = new Quaternion(
        0.11182449012994766f,
        0.0000029315337997104509f,
        -0.00002605106601549778f,
        0.99372798204422f
    );

    // ===== FINAL TRANSFORM =====
    private Vector3 endPosition = new Vector3(
        -8.031999588012696f,
        11.965999603271485f,
        11.642000198364258f
    );

    private Quaternion endRotation = new Quaternion(
        0.0029394507873803379f,
        0.9573697447776794f,
        -0.009868863970041275f,
        0.28868192434310915f
    );

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isUnlocked) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            TryUnlock();
        }
    }

    void TryUnlock()
    {
        if (GameManager.Instance.CanUnlock())
        {
            isUnlocked = true;

            // ===== AUDIO SWAP =====
            if (audioSource != null && newAudio != null)
            {
                audioSource.Stop();
                audioSource.clip = newAudio;
                audioSource.Play();
            }

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
                objectToActivate.transform.SetPositionAndRotation(
                    startPosition,
                    startRotation
                );

                StartCoroutine(LerpTwoStage(objectToActivate.transform));
            }

            if (objectToActivateAlso != null)
                objectToActivateAlso.SetActive(true);

            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(false);

            if (flag != null)
                StartCoroutine(LerpFlagUp(flag.transform));
        }
    }

    IEnumerator LerpTwoStage(Transform target)
    {
        float halfDuration = lerpDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            target.position = Vector3.Lerp(startPosition, midPosition, t);
            target.rotation = Quaternion.Slerp(startRotation, midRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.SetPositionAndRotation(midPosition, midRotation);

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            target.position = Vector3.Lerp(midPosition, endPosition, t);
            target.rotation = Quaternion.Slerp(midRotation, endRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.SetPositionAndRotation(endPosition, endRotation);
    }

    IEnumerator LerpFlagUp(Transform flagTransform)
    {
        Vector3 startPos = flagTransform.position;
        Vector3 endPos = startPos + Vector3.up * flagHeightIncrease;

        float elapsed = 0f;
        while (elapsed < flagLerpDuration)
        {
            float t = elapsed / flagLerpDuration;
            flagTransform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        flagTransform.position = endPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
