using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using TMPro;

public class SpeechRecord : MonoBehaviour
{
    public string Filename = "record_username";

    public AudioClip clip { get; private set; }
    private byte[] bytes;
    private bool recording;
    
    private PowerUpManager powerUpManager;

    private void Start()
    {
        powerUpManager = GetComponent<PowerUpManager>();
        string savedUsername = PlayerPrefs.GetString("Username", "");
        Filename = "record_" + savedUsername;
    }

    public void StartRecording()
    {
        Debug.Log("START HORE");
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No microphone found.");
            return;
        }
        Debug.Log("nyalain mic");
        clip = Microphone.Start(Microphone.devices[0], true, 5, 44100);
        Debug.Log($"update recording: {recording}");
        recording = true;
    }

    public void StopRecording()
    {
        Debug.Log("stop recording");
        if (!Microphone.IsRecording(null))
        {
            Debug.LogWarning("Microphone is not recording.");
            return;
        }

        if (Microphone.GetPosition(null) >= clip.samples)
        {
            Debug.LogWarning("Clip is not ready.");
            return;
        }

        if (clip == null)
        {
            Debug.LogError("Audio clip is null.");
            return;
        }

        var position = Microphone.GetPosition(null);
        Microphone.End(null);

        var samples = new float[position * clip.channels];
        if (clip.GetData(samples, 0) == false)
        {
            Debug.LogError("Failed to get data from AudioClip.");
            return;
        }

        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        if (bytes == null)
        {
            Debug.LogError("Failed to encode data as WAV.");
            return;
        }

        recording = false;
        File.WriteAllBytes(Application.dataPath + $"/record/{Filename}.wav", bytes);
        Debug.Log("file saved");
        Debug.Log(Application.dataPath + $"/record/{Filename}.wav");
        UploadFile();

    }

    public void RecordFor3Seconds()
    {
        StartRecording(); // Start recording
        Debug.Log("waiting");
        // Record for 3 seconds
        StartCoroutine(StopRecordingAfterDelay(3f));
    }

    private IEnumerator StopRecordingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopRecording(); // Stop recording after the delay
    }
    
    public void UploadFile()
    {
        Debug.Log("masuk upload file");
        string filePath = Application.dataPath + $"/record/{Filename}.wav";
        string uploadUrl = "https://lizard-alive-suitably.ngrok-free.app/upload"; // Ganti dengan URL server Anda

        StartCoroutine(UploadFileRequest(uploadUrl, filePath));
    }
    private IEnumerator UploadFileRequest(string url, string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        WWWForm form = new();
        form.AddBinaryData("file", fileData, fileName, "audio/wav");

        using UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gagal mengunggah file: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log("File berhasil diunggah ke server. Respons: " + jsonResponse);

            // Memproses respons JSON
            ProcessJsonResponse(jsonResponse);
        }
    }
    private void ProcessJsonResponse(string jsonResponse)
    {
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

        string emotion = responseData.emotion;
        float percentage = responseData.percentage;

        switch (emotion)
        {
            case "angry":
                emotion = "Angry";
                break;
            case "disgust":
                emotion = "Disgust";
                break;
            case "fear":
                emotion = "Fear";
                break;
            case "happy":
                emotion = "Joy";
                break;
            case "neutral":
                emotion = "Neutral";
                break;
            case "sad":
                emotion = "Sad";
                break;
            case "surprise":
                emotion = "Surprise";
                break;
        }


        powerUpManager.predictedEmotion = emotion;
        powerUpManager.percentage = percentage;

        Debug.Log(powerUpManager.predictedEmotion);
        powerUpManager.ProcessResults();
        Debug.Log("Emosi: " + emotion + ", Persentase: " + percentage);
    }

    [System.Serializable]
    private class ResponseData
    {
        public string emotion;
        public float percentage;
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
