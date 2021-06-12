using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnWorkflow : MonoBehaviour {

	private const int NORMAL = 0;
	private const int ARMOR = 1;
	private const int SPLITTER = 2;
	private const int ACE = 3;
	private const int CHAIN = 4;
	private const int FIREBALL = 5;
	private const int BOMB = 6;

	public List<int> innerTendencies;
	public List<int> outerTendencies;

	private int innerTotal, outerTotal, level;
    public int armorMin, armorMax, splitterMin, splitterMax;

	public void sumTotal(){
        innerTotal = 0;
		outerTotal = 0;
		for(int k = 0; k < innerTendencies.Count; k++){
			innerTotal += innerTendencies[k];
		}

		for(int k = 0; k < outerTendencies.Count; k++){
			outerTotal += outerTendencies[k];
		}
	}

	public int pickHedgieTypeOuter(){
		int rand = Random.Range(0, outerTotal);
		int max = outerTendencies[0];
		int k = 0;
		if(max > rand)
			return k;
		while(rand >= max){
			k++;
			max += outerTendencies[k];
		}
		return k;
	}

	public int PickHedgieTypeInner(){
		int rand = Random.Range(0, innerTotal);
		int max = innerTendencies[0];
		int k = 0;
		if(max > rand)
			return k;
		while(rand >= max){
			k++;
			max += innerTendencies[k];
		}
		return k;
	}

	public int pickHedgieHealth(int type){
		if(type == ARMOR){
			return Random.Range(armorMin, armorMax + 1);
        }
        else if (type == SPLITTER) {
            return Random.Range(splitterMin, splitterMax + 1);
        }
        else{
			return 1;
		}
	}

	public void SetTendencies(){
		sumTotal();
	}

	public void SetPercent(float percent){
		int half_percent = (int)(Mathf.Clamp(percent / 2f, 0f, 100f));
		innerTendencies[1] = half_percent;
		innerTendencies[2] = half_percent;
		innerTendencies[0] = 100 - (half_percent + half_percent);
		sumTotal();
	}

}
