using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hedgehog{
	public int d, color, health, type, split_id;

	public Hedgehog(int color, int health, int type){
		this.color = color;
		this.health = health;
		this.type = type;
		this.d = 0;
		this.split_id = -1;
	}

	public Hedgehog(Hedgehog heg){
		color = heg.color;
		health = heg.health;
		type = heg.type;
		// d = heg.d;
		d = 0;
		split_id = heg.split_id;
	}

	public void pop(){
		color = -1;
		health = -1;
		type = -1;
		d = 0;
	}

	public void AddDegree(){
		d += 1;
	}

	public override bool Equals(object obj){
        Hedgehog other = obj as Hedgehog;
        return (other != null && other.color == this.color);
    }

    public override int GetHashCode(){
        return color.GetHashCode();
    }

	public void TakeDamage(){
		health--;
	}

	public int getColor(){
		return color;
	}

	public int getType(){
		return type;
	}

	public int getHealth(){
		return health;
	}

	public void loseHealth(int damage){
		health += damage;
		if(health < 1){
			pop();
		}
	}

	public void setHealth(int health){
		this.health = health;
	}

	public void setType(int type){
		this.type = type;
	}

	public void setSplitId(int id){
		split_id = id;
	}

	public int getSplitId(){
		return split_id;
	}

}
