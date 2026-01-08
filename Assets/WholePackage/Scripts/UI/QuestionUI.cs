using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class QuestionUI : MonoBehaviour
{
    public static QuestionUI Instance;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI[] answerTexts;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Font Settings")]
    [SerializeField] private TMP_FontAsset fontAsset;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float audioDelay = 2f;
    [SerializeField] [Range(0f, 1f)] private float audioVolume = 1f;
    
    [Header("Settings")]
    [SerializeField] private float feedbackDisplayTime = 1.5f;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    
    private QuestionData currentQuestion;
    private Action<bool> onAnswerCallback;
    private bool isShowingFeedback = false;
    private Coroutine audioCoroutine;
    private AudioClip currentAudioClip;
    
    public bool IsOpen => questionPanel != null && questionPanel.activeSelf;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
        
        SetupAudioSource();
    }
    
    void SetupAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = audioVolume;
        audioSource.spatialBlend = 0f;
        
        Debug.Log("AudioSource setup complete");
    }
    
    void Start()
    {
        SetupButtonListeners();
        
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
        
        if (fontAsset != null)
        {
            ApplyFont(fontAsset);
        }
    }
    
    void SetupButtonListeners()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                int buttonIndex = i;
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(buttonIndex));
            }
        }
    }
    
    public void ShowQuestion(QuestionData question, Action<bool> onAnswer, TMP_FontAsset customFont, AudioClip questionAudio)
    {
        if (question == null)
        {
            Debug.LogError("QuestionUI: No question provided!");
            return;
        }
        
        if (GameManager.Instance != null && GameManager.Instance.GameLost)
        {
            return;
        }
        
        currentQuestion = question;
        onAnswerCallback = onAnswer;
        currentAudioClip = questionAudio;
        
        // Apply font
        if (customFont != null)
        {
            ApplyFont(customFont);
        }
        else if (fontAsset != null)
        {
            ApplyFont(fontAsset);
        }
        
        // Set question text
        if (questionText != null)
        {
            questionText.text = question.questionText;
        }
        
        // Set answer texts
        for (int i = 0; i < answerTexts.Length && i < question.answers.Length; i++)
        {
            if (answerTexts[i] != null)
            {
                answerTexts[i].text = question.answers[i];
            }
        }
        
        ResetButtons();
        
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
        
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
        }
        
        // Show cursor
        PlayerCamera5.UnlockCursor();
        
        // DO NOT PAUSE TIME - Audio needs time to run!
        // Time.timeScale = 0f;  // REMOVED!
        
        // Play audio with delay
        if (currentAudioClip != null)
        {
            Debug.Log("Audio clip found: " + currentAudioClip.name);
            
            if (audioCoroutine != null)
            {
                StopCoroutine(audioCoroutine);
            }
            audioCoroutine = StartCoroutine(PlayAudioWithDelay());
        }
        else
        {
            Debug.Log("No audio clip assigned for this question");
        }
        
        Debug.Log("Question UI opened");
    }
    
    IEnumerator PlayAudioWithDelay()
    {
        Debug.Log("Waiting " + audioDelay + " seconds before playing audio...");
        
        // Wait for delay (normal time, not realtime)
        yield return new WaitForSeconds(audioDelay);
        
        // Play audio
        if (audioSource != null && currentAudioClip != null)
        {
            audioSource.clip = currentAudioClip;
            audioSource.volume = audioVolume;
            audioSource.Play();
            
            Debug.Log("NOW PLAYING audio: " + currentAudioClip.name);
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is null!");
        }
    }
    
    public void HideQuestion()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
        
        // Stop audio
        StopAudio();
        
        // Hide cursor
        if (GameManager.Instance == null || !GameManager.Instance.GameLost)
        {
            PlayerCamera5.LockCursor();
        }
        
        // Time is already running, no need to change
        // Time.timeScale = 1f;  // REMOVED!
        
        currentQuestion = null;
        onAnswerCallback = null;
        isShowingFeedback = false;
        
        Debug.Log("Question UI closed");
    }
    
    void StopAudio()
    {
        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }
        
        if (audioSource != null)
        {
            audioSource.Stop();
            Debug.Log("Audio stopped");
        }
    }
    
    void ApplyFont(TMP_FontAsset font)
    {
        if (questionText != null)
        {
            questionText.font = font;
        }
        
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (answerTexts[i] != null)
            {
                answerTexts[i].font = font;
            }
        }
        
        if (feedbackText != null)
        {
            feedbackText.font = font;
        }
    }
    
    void ResetButtons()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                answerButtons[i].interactable = true;
                
                Image image = answerButtons[i].GetComponent<Image>();
                if (image != null)
                {
                    image.color = Color.white;
                }
            }
        }
    }
    
    void SetButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                answerButtons[i].interactable = interactable;
            }
        }
    }
    
    void OnAnswerClicked(int answerIndex)
    {
        if (isShowingFeedback) return;
        if (currentQuestion == null) return;
        
        bool isCorrect = currentQuestion.IsCorrectAnswer(answerIndex);
        
        if (!isCorrect)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMistake();
            }
        }
        
        StartCoroutine(ShowFeedbackAndClose(isCorrect, answerIndex));
    }
    
    IEnumerator ShowFeedbackAndClose(bool isCorrect, int selectedIndex)
    {
        isShowingFeedback = true;
        
        // Stop question audio when answering
        StopAudio();
        
        SetButtonsInteractable(false);
        
        if (selectedIndex >= 0 && selectedIndex < answerButtons.Length)
        {
            Image buttonImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = isCorrect ? correctColor : wrongColor;
            }
        }
        
        if (!isCorrect && currentQuestion != null)
        {
            int correctIndex = currentQuestion.correctAnswerIndex;
            if (correctIndex >= 0 && correctIndex < answerButtons.Length)
            {
                Image correctImage = answerButtons[correctIndex].GetComponent<Image>();
                if (correctImage != null)
                {
                    correctImage.color = correctColor;
                }
            }
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = isCorrect ? "Correct!" : "Wrong!";
            feedbackText.color = isCorrect ? correctColor : wrongColor;
            feedbackText.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(feedbackDisplayTime);
        
        if (onAnswerCallback != null)
        {
            onAnswerCallback.Invoke(isCorrect);
        }
        
        HideQuestion();
    }
}