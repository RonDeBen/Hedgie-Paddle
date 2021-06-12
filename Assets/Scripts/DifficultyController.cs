using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour{

    public SpawnWorkflow sw;
    public int level = 1;


    public void CalculateTendencies() {
        float special_percent = GetSpecialPercent();
        sw.SetPercent(special_percent);
    }

    public int CalculateDimensions(int inner_hedgies, int current_dim){
        int threshold = (current_dim - 2) * (current_dim - 3);
        if(inner_hedgies > threshold){
            return (current_dim + 1);
        }
        return current_dim;
    }

    private float GetSpecialPercent(){
        return Mathf.Pow(0.6f * level, 1.5f);
    }

    private float AverageNumberOfMoves(){
        return (level)+5;
    }

    public int CalculateInnerHedgehogs(){
        float num_norm_hedgies = 0.72f*AverageNumberOfMoves() - 2.38f;
        float special_multiplier = 0.76f * Mathf.Clamp(GetSpecialPercent() / 100f, 0f, 1f);
        float num_hedgies = (num_norm_hedgies * (1f + special_multiplier));
        Debug.Log(num_hedgies);
        return (int)num_hedgies;
    }

    public void NextLevel(){
        level++;
    }
}
