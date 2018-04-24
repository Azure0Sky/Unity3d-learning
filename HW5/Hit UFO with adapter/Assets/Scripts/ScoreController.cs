using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

    private int score;      // 当前分数

    private ScoreController() { }

    private void Start()
    {
        score = 0;
    }

    public void Record( DiskData disk )
    {
        score += disk.score;
    }

    public void Reset()
    {
        score = 0;
    }

    public int GetScore()
    {
        return score;
    }

}
