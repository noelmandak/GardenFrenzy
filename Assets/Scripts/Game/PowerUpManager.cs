using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    
    public PlayerController playerController;
    private List<PowerUpClass> activePowerUps = new();
    public GameManager gameManager;

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
        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            if (!gameManager.IsPaused()) // Hanya kurangi waktu jika permainan tidak dijeda
            {
                activePowerUps[i].UpdateDuration(Time.deltaTime);
                Debug.Log($"Powerup {activePowerUps[i].powerUpType} {activePowerUps[i].GetDuration()}");
            }

            if (activePowerUps[i].IsExpired())
            {
                // Hapus power up yang telah berakhir
                activePowerUps.RemoveAt(i);
            }
        }
    }

    private float GetDuration(int star)
    {
        return star switch
        {
            1 => 3.0f,
            2 => 6.0f,
            3 => 10.0f,
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
            float speedMultiplier = 2f;
            playerController.SetPlayerSpeed(!isPlayerRed, playerController.GetPlayerSpeed(!isPlayerRed) / speedMultiplier);
            Debug.Log(playerController.GetPlayerSpeed(!playerController.isRed));
        }
        // Implementasikan efek-efek power up lainnya sesuai kebutuhan Anda
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
            playerController.SetPlayerSpeed(!isPlayerRed, playerController.GetPlayerSpeed(!isPlayerRed) * 2f);
            Debug.Log(playerController.GetPlayerSpeed(playerController.isRed));
        }
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