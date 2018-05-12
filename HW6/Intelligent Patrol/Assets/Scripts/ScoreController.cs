using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

    private int score;

	void Awake ()
    {
        score = 0;
        PatrolController.losePlayerEvent += Scoring;
	}

    public void Scoring()
    {
        ++score;
    }

    public int GetScore()
    {
        return score;
    }
}
