﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultsController : MonoBehaviour
{
    private readonly int [] MULTIPLIER = {2, 10, 5};
    RiverManager rm;
    public TextMeshProUGUI finalScore;
    public float score; 
    public GameObject multipliers;

    public GameObject p1Crown, p2Crown;
    public AudioSource audio;
    public AudioClip clipPoints;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rm = RiverManager.instance;
    }

    public void ShowMultipliers()
    {
        multipliers.SetActive(true);
        finalScore.gameObject.SetActive(true);
        StartCoroutine(AddUpScores());
    }

    private IEnumerator AddUpScores()
    {
        float totalP1Score = rm.GetScore(0);
        float totalP2Score = rm.GetScore(1);

        // Loop through all item categories 
        for(int i = 0; i < 3; i ++)
        {
            // While there are still points in category for either player
            while (rm.GetScore(0, i) + rm.GetScore(1, i) > 0)
            {   
                // Player 1 decrement
                if (rm.GetScore(0, i) > 0)
                    rm.AddToScoreLocal(0, i, -1);
                // Player 2 decrement
                if (rm.GetScore(1, i) > 0)
                    rm.AddToScoreLocal(1, i, -1);
                
                // Increment final score time multiplier for category i
                score += MULTIPLIER[i];
                // Update UI
                finalScore.text = "Total Score: " + score.ToString();
                audio.PlayOneShot(clipPoints);
                
                if (audio.pitch < 2f)
                    audio.pitch += 0.05f;

                yield return new WaitForSeconds(0.2f);
            }
        }
    
        // Give Crown when done
        if (totalP1Score > totalP2Score)
        {
            p1Crown.SetActive(true);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti2, p1Crown.transform.position, 10);
        }
        else if (totalP1Score < totalP2Score)
        {
            p2Crown.SetActive(true);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti2, p2Crown.transform.position, 10);
        }
        else
        {
            p1Crown.SetActive(true);
            p2Crown.SetActive(true);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti2, p1Crown.transform.position, 15);
            FXManager.GetInstance().Burst(FXManager.FX.Confetti2, p2Crown.transform.position, 15);
        }
        
        multipliers.SetActive(false);

        yield return null;
    }

    // Update is called once per frame
    void Update(){}
}
