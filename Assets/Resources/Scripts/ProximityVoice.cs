using UnityEngine;
using Unity.Netcode;

public class ProximityVoice : NetworkBehaviour
{

    private AudioClip micClip;
    private string micDevice;
    private float[] sampleBuffer;
    private int sampleRate = 44100;
    private int bufferLength = 4410; // ~100ms

    private float sendInterval = 0.1f; // 100ms
    private float lastSendTime;
    private float maxDistance = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsOwner) return;

        micDevice = Microphone.devices[0];
        Debug.Log(micDevice);
        micClip = Microphone.Start(micDevice, true, 1, sampleRate);
        sampleBuffer = new float[bufferLength];
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner || !Microphone.IsRecording(micDevice)) return;

        if (Time.time - lastSendTime > sendInterval)
        {
            int micPos = Microphone.GetPosition(micDevice);
            int startSample = micPos - bufferLength;
            if (startSample < 0) startSample += micClip.samples;

            micClip.GetData(sampleBuffer, startSample);
            byte[] byteData = FloatToBytes(sampleBuffer);

            SendAudioToServerRpc(byteData);

            lastSendTime = Time.time;
        }
    }

    private byte[] FloatToBytes(float[] floats)
    {
        byte[] bytes = new byte[floats.Length * 4];
        System.Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    private void SendAudioToServerRpc(byte[] audioData)
    {
        BroadcastToNearbyClientsClientRpc(audioData, OwnerClientId);
    }

    [ClientRpc]
    private void BroadcastToNearbyClientsClientRpc(byte[] audioData, ulong senderId)
    {
        // Don’t play on self
        if (IsOwner || NetworkManager.LocalClientId == senderId) return;

        var sender = GetPlayerById(senderId);
        if (sender == null) return;

        float distance = Vector3.Distance(transform.position, sender.transform.position);
        if (distance > maxDistance) return; // only play audio if close

        //Debug.Log(GetMicVolume());
        sender.transform.Find("Voice").GetComponent<ProximityListen>().PlayAudioClip(audioData);
    }

    private NetworkBehaviour GetPlayerById(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            return client.PlayerObject.GetComponent<NetworkBehaviour>();
        return null;
    }

    float GetMicVolume()
    {
        float[] samples = new float[128];
        micClip.GetData(samples, 0);

        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }

        return sum / samples.Length;
    }
}
