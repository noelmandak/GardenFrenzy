using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionComponent : MonoBehaviour
{
    public GameObject[] EmotionLogos; // 1 = angry, 2 = sad, 3 = fear, 4 = joy
    public GameObject[] Stars;
    public GameObject[] NonActiveStars;

    public void SetEmotionLogo(int emotionType)
    {
        EmotionLogos[emotionType-1].SetActive(true);
    }

    public void SetStars(int star)
    {
        if (star > 0) Stars[0].SetActive(true);
        if (star > 1) Stars[1].SetActive(true);
        if (star > 2) Stars[2].SetActive(true);
        if (star > 0) NonActiveStars[0].SetActive(false);
        if (star > 1) NonActiveStars[1].SetActive(false);
        if (star > 2) NonActiveStars[2].SetActive(false);
    }
}
