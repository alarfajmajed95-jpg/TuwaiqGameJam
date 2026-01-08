using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip testClip;
    
    void Update()
    {
        // Press T to test audio
        if (Input.GetKeyDown(KeyCode.T))
        {
            AudioSource.PlayClipAtPoint(testClip, Camera.main.transform.position, 1f);
            Debug.Log("Playing test audio!");
        }
    }
}