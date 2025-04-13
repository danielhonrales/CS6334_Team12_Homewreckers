using UnityEngine;

public class ProximityListen : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    public void PlayAudioClip(byte[] audioData)
    {
        float[] floatData = BytesToFloat(audioData);
        AudioClip clip = AudioClip.Create("VoiceClip", floatData.Length, 1, 44100, false);
        clip.SetData(floatData, 0);

        audioSource.clip = clip;
        audioSource.Play();
    }

    private float[] BytesToFloat(byte[] bytes)
    {
        float[] floats = new float[bytes.Length / 4];
        System.Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
        return floats;
    }
}
