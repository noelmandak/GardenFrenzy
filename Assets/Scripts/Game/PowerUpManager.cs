using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    
    public PlayerController playerController;
    public PowerUpUI powerUpUI;
    private List<PowerUpClass> activePowerUps = new();
    public GameManager gameManager;
    public GameObject[] RedPUEffect;
    public GameObject[] BluePUEffect;

    public void ActivatePower(bool isPlayerRed, int powerUpType, int star)
    {
        float duration = GetDuration(star);

        PowerUpClass powerUp = new(powerUpType, duration, isPlayerRed);
        activePowerUps.Add(powerUp);
        Debug.Log($"powerup activated {powerUpType} stars {star}");

        // Terapkan efek dari power up
        ApplyPowerUpEffect(isPlayerRed, powerUpType);
        // Pastikan power up dihapus dari player ketika durasinya berakhir
        StartCoroutine(RemovePowerUpAfterDuration(powerUp));
    }
    private IEnumerator RemovePowerUpAfterDuration(PowerUpClass powerUp)
    {
        while (!gameManager.IsPaused()) // Hanya lakukan penghitungan waktu jika permainan tidak dijeda
        {
            yield return new WaitForSeconds(powerUp.duration);

            // Hapus power up dari player setelah durasinya berakhir
            RemovePowerUpFromPlayer(powerUp.isRed, powerUp.powerUpType);
            break;
        }
    }

    private void Update()
    {
        List<int> RedPowerUpType = new List<int>();
        List<float> RedPowerUpDuration = new List<float>();
        List<int> BluePowerUpType = new List<int>();
        List<float> BluePowerUpDuration  = new List<float>();

        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            if (!gameManager.IsPaused()) // Hanya kurangi waktu jika permainan tidak dijeda
            {
                activePowerUps[i].UpdateDuration(Time.deltaTime);
                if (activePowerUps[i].isRed) 
                {
                    RedPowerUpType.Add(activePowerUps[i].powerUpType);
                    RedPowerUpDuration.Add((int)activePowerUps[i].GetDuration());
                } 
                else 
                {
                    BluePowerUpType.Add(activePowerUps[i].powerUpType);
                    BluePowerUpDuration.Add((int)activePowerUps[i].GetDuration());
                }
            }

            if (activePowerUps[i].IsExpired())
            {
                // Hapus power up yang telah berakhir
                activePowerUps.RemoveAt(i);
            }

        }
        powerUpUI.UpdateMessage(true, RedPowerUpType.ToArray(), RedPowerUpDuration.ToArray());
        powerUpUI.UpdateMessage(false, BluePowerUpType.ToArray(), BluePowerUpDuration.ToArray());
    }

    private float GetDuration(int star)
    {
        return star switch
        {
            1 => 8.0f,
            2 => 15.0f,
            3 => 20.0f,
            _ => 0.0f,
        };
    }

    private void ApplyPowerUpEffect(bool isPlayerRed, int powerUpType)
    {
        // Terapkan efek power up sesuai dengan deskripsi yang diberikan
        // Contoh: jika powerUpType adalah 1 (Angry), tingkatkan kecepatan player merah
        if (powerUpType == 1)
        {
            float speedMultiplier = 2f;
            playerController.SetPlayerSpeed(isPlayerRed, playerController.GetPlayerSpeed(isPlayerRed) * speedMultiplier);
        }else if (powerUpType == 2)
        {

            Debug.Log(playerController.GetPlayerSpeed(!playerController.isRed));
            float speedMultiplier = 5f;
            playerController.SetPlayerSpeed(!isPlayerRed, playerController.GetPlayerSpeed(!isPlayerRed) / speedMultiplier);
            Debug.Log(playerController.GetPlayerSpeed(!playerController.isRed));
        }else if (powerUpType == 3)
        {
            playerController.SetFearField(isPlayerRed, true);

        }
        else if (powerUpType == 4)
        {
            Debug.Log($"activate red{playerController.isPlayerRedDoublePoin} ~ blue{playerController.isPlayerBlueDoublePoin}");
            playerController.SetDoublePoints(isPlayerRed,true);
            Debug.Log($"activate red{playerController.isPlayerRedDoublePoin} ~ blue{playerController.isPlayerBlueDoublePoin}");
        }
        // Implementasikan efek-efek power up lainnya sesuai kebutuhan Anda
        SetPUEffect(isPlayerRed,powerUpType,true);
    }
    private void RemovePowerUpFromPlayer(bool isPlayerRed, int powerUpType)
    {
        // Implementasikan penghapusan power up dari player sesuai dengan kebutuhan Anda
        // Contoh: jika powerUpType adalah 1 (Angry), kurangi kecepatan player merah kembali ke nilai semula
        Debug.Log($"powerup deactivated {powerUpType}");
        if (powerUpType == 1)
        {
            Debug.Log(playerController.GetPlayerSpeed(playerController.isRed));
            playerController.SetPlayerSpeed(isPlayerRed, playerController.GetPlayerSpeed(isPlayerRed) / 2f);
            Debug.Log(playerController.GetPlayerSpeed(playerController.isRed));
        } 
        // Implementasikan penghapusan power up lainnya sesuai kebutuhan Anda
        else if (powerUpType == 2)
        {
            Debug.Log(playerController.GetPlayerSpeed(playerController.isRed));
            playerController.SetPlayerSpeed(!isPlayerRed, playerController.GetPlayerSpeed(!isPlayerRed) * 5f);
            Debug.Log(playerController.GetPlayerSpeed(playerController.isRed));
        }
        else if (powerUpType == 3)
        {
            playerController.SetFearField(isPlayerRed,false);
        }
        else if (powerUpType == 4)
        {
            Debug.Log($"deactive red{playerController.isPlayerRedDoublePoin} ~ blue{playerController.isPlayerBlueDoublePoin}");
            playerController.SetDoublePoints(isPlayerRed,false);
            Debug.Log($"deactive red{playerController.isPlayerRedDoublePoin} ~ blue{playerController.isPlayerBlueDoublePoin}");
        }
        SetPUEffect(isPlayerRed, powerUpType, false);
    }

    public void SetPUEffect(bool isPlayerRed, int powerUpType, bool active)
    {
        if (isPlayerRed) RedPUEffect[powerUpType - 1].SetActive(active);
        else BluePUEffect[powerUpType - 1].SetActive(active);
    }
}


public class PowerUpClass
{
    public bool isRed;
    public int powerUpType;
    public float duration;

    public PowerUpClass(int type, float duration, bool isRed)
    {
        this.powerUpType = type;
        this.duration = duration;
        this.isRed = isRed;
    }

    public void UpdateDuration(float deltaTime)
    {
        duration -= deltaTime;
    }

    public float GetDuration()
    {
        return duration;
    }
    public bool IsExpired()
    {
        return duration <= 0;
    }
}