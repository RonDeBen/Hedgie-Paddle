using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coords{
	public int x, y;

    public Coords(int x, int y){
        this.x = x;
        this.y = y;
    }

    public override string ToString(){
		return ("x: " + x + ", y: " + y);
	}

	public override bool Equals(object obj){
        Coords other = obj as Coords;
        return (other != null && other.x == this.x && other.y == this.y);
    }

    public override int GetHashCode(){
        return x.GetHashCode() ^ y.GetHashCode();
    }
}
