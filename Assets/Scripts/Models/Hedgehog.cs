using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hedgehog{
	public int d, color;

	public Hedgehog(int color){
		this.color = color;
		this.d = 0;
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
}
