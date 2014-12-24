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

	public void sumTotal(){
		for(int k = 0; k < tendencies.Count; k++){
			total += tendencies[k];
		}
	}

	public int pickHedgieType(){
		int rand = Random.Range(0, total);
		int max = tendencies[0];
		int k = 0;
		if(max > rand)
			return k;
		while(rand >= max){
			k++;
			max += tendencies[k];
		}
		return k;
	}

	public int pickHedgieHealth(int type){
		if(type == ARMOR || type == SPLITTER){
			return Random.Range(2,4);
		}else{
			return 1;
		}
	}

}
