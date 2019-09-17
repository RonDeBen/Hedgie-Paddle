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

	public List<int> tendencies;

	private int total, level;
    private int armorMin, armorMax, splitterMin, splitterMax;
	public void sumTotal(){
        total = 0;
		for(int k = 0; k < tendencies.Count; k++){
			total += tendencies[k];
		}
	}

	public int pickHedgieType(){
		int rand = Random.Range(0, total);
		int max = tendencies[0];
		int k = 0;
		if(max >= rand)
			return k;
		while(rand >= max){
			k++;
			max += tendencies[k];
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

    public void setRange(int armorMin, int armorMax, int splitterMin, int splitterMax) {
        this.armorMin = armorMin;
        this.armorMax = armorMax;
        this.splitterMin = splitterMin;
        this.splitterMax = splitterMax;
    }

    public void setTendencies(int normalTend, int armorTend, int splitterTend) {
        tendencies[0] = normalTend;
        // tendencies[1] = armorTend;
        // tendencies[2] = splitterTend;
        sumTotal();
    }

}
