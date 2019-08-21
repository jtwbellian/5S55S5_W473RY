using System.Collections;
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
    // Start is called before the first frame update
    void Start()
    {
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
        for(int i = 0; i < 2; i ++)
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
                finalScore.text = "Score: " + score.ToString();

                yield return new WaitForSeconds(1f);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
