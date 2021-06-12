using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {
	/*for side- top: 0, right: 1, bottom: 2, left: 3
	  for rotation- none: 0, cwise: 1, ccwise: -1, 2cwise: 2*/
	public int side, rotation, index;

	public Move(int side, int rotation, int index){
		this.side = side;
		this.rotation = rotation;
		this.index = index;
	}
	
	public override string ToString(){
		return ("side: " + SideToString(side) + ", rotation: " + rotation + ", index: " + index);
	} 

	public override bool Equals(object obj){
        Move other = obj as Move;
        return (other != null && other.side == this.side && other.rotation == this.rotation && other.index == this.index);
    }

    public override int GetHashCode(){
        return side.GetHashCode() ^ rotation.GetHashCode() ^ index.GetHashCode();
    } 

	public int GetRotation(){
		return rotation;
	}

	private string SideToString(int side){
		switch(side){
			case 0:
				return "top";
			case 1:
				return "right";
			case 2:
				return "bottom";
			default:
				return "left";
		}
	}
}
