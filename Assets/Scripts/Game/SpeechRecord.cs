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
    public ActivatePowerUpUI activatePowerUpUI;
    string directoryPath;

    public class CustomCertificateHandler : CertificateHandler
    {
        // Override method to always accept the certificate
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Always return true, indicating that the certificate is valid
            return true;
        }
    }
    private void Start()
    {
        powerUpManager = GetComponent<PowerUpManager>();
        string savedUsername = PlayerPrefs.GetString("Username", "");
        Filename = "record_" + savedUsername;
        directoryPath = Path.Combine(Application.persistentDataPath, "record");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public void StartRecording()
    {
        Debug.Log("START HORE");
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No microphone found.");
            SSTools.ShowMessage("No microphone found.", SSTools.Position.bottom, SSTools.Time.twoSecond);

            return;
        }
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
            SSTools.ShowMessage("Microphone is not recording.", SSTools.Position.bottom, SSTools.Time.twoSecond);

            return;
        }

        if (Microphone.GetPosition(null) >= clip.samples)
        {
            Debug.LogWarning("Clip is not ready.");
            SSTools.ShowMessage("Clip is not ready.", SSTools.Position.bottom, SSTools.Time.twoSecond);
            return;
        }

        if (clip == null)
        {
            Debug.LogError("Audio clip is null.");
            SSTools.ShowMessage("Audio clip is null.", SSTools.Position.bottom, SSTools.Time.twoSecond);
            return;
        }

        var position = Microphone.GetPosition(null);
        Microphone.End(null);

        var samples = new float[position * clip.channels];
        if (clip.GetData(samples, 0) == false)
        {
            Debug.LogError("Failed to get data from AudioClip.");
            SSTools.ShowMessage("Failed to get data from AudioClip.", SSTools.Position.bottom, SSTools.Time.twoSecond);
            return;
        }

        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        if (bytes == null)
        {
            Debug.LogError("Failed to encode data as WAV.");
            SSTools.ShowMessage("Failed to encode data as WAV.", SSTools.Position.bottom, SSTools.Time.twoSecond);
            return;
        }
        

        recording = false;
        // Menyimpan file
        File.WriteAllBytes(Path.Combine(directoryPath, $"{Filename}.wav"), bytes);
        Debug.Log("file saved");
        Debug.Log(Path.Combine(directoryPath, $"{Filename}.wav"));
        UploadFile();

    }

    public void RecordFor3Seconds()
    {
        StartRecording(); // Start recording
        //SSTools.ShowMessage("waiting", SSTools.Position.bottom, SSTools.Time.twoSecond);
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
        string filePath = Path.Combine(directoryPath, $"{Filename}.wav");
        string uploadUrl = "https://lizard-alive-suitably.ngrok-free.app/upload"; // Ganti dengan URL server Anda

        StartCoroutine(UploadFileRequest(uploadUrl, filePath));
    }

    private IEnumerator UploadFileRequest(string url, string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        // Mendeklarasikan form sebelum penggunaan
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "audio/wav");

        // Menggunakan CustomCertificateHandler
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.certificateHandler = new CustomCertificateHandler();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gagal mengunggah file: " + www.error);
            SSTools.ShowMessage("Gagal mengunggah file: " + www.error, SSTools.Position.bottom, SSTools.Time.twoSecond);
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
