using UnityEngine;

[System.Serializable]
public class QuestionData
{
    [TextArea(2, 4)]
    public string questionText;
    
    public string[] answers = new string[4];
    
    [Range(0, 3)]
    public int correctAnswerIndex;
    
    [Header("Audio Settings")]
    [Tooltip("Audio clip to read the question")]
    public AudioClip questionAudio;
    
    public bool IsCorrectAnswer(int answerIndex)
    {
        return answerIndex == correctAnswerIndex;
    }
}