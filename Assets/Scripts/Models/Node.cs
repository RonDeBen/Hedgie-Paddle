using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node{
	public float entropy;
	public Dictionary<Coords, Hedgehog> boardState;
	public int[] leftSide, rightSide, topSide, bottomSide;
	public int depth, moveColor;
	public Coords newPos;
	public Move move;

	public Node(Dictionary<Coords, Hedgehog> qboardState, Move move, int[] leftSide, int[] rightSide, int[] topSide, int[] bottomSide){
		depth = 1;//to change to actual depth later
		this.move = move;
		this.leftSide = CloneArray(leftSide);
		this.rightSide = CloneArray(rightSide);
		this.topSide = CloneArray(topSide);
		this.bottomSide = CloneArray(bottomSide);

		if(this.move.Equals(new Move(-1,-1,-1))){
			boardState = CloneDictionary(qboardState);
			DetermineDegreesOfFreedom();
			entropy = Entropy();
			Debugging();
		}else{
			boardState = CloneDictionary(qboardState);
			GetResultingBoardState();
			DetermineDegreesOfFreedom();
			entropy = Entropy();
		}
	}

	private Dictionary<Coords, Hedgehog> CloneDictionary(Dictionary<Coords, Hedgehog> startDict){
		Dictionary<Coords, Hedgehog> newDict = new Dictionary<Coords, Hedgehog>();
		foreach(KeyValuePair<Coords, Hedgehog> kvp in startDict){
			newDict.Add(new Coords(kvp.Key.x, kvp.Key.y), new Hedgehog(kvp.Value.color));
		}
		return newDict;
	}

	private int[] CloneArray(int[] startArray){
		int[] newArray = new int[startArray.Length];
		for(int k = 0; k < startArray.Length; k++){
			newArray[k] = startArray[k];
		}
		return newArray;
	}

	public override string ToString(){
		return ("Entropy: " + entropy + ", Move: " + move.ToString() + " color: " + NumToColor(moveColor) + " Count: " + boardState.Count);
	} 

	public Node(){
		entropy = float.MaxValue;
		// entropy = 0;
	}

	private float Prob(int d){
		return (1 - Mathf.Pow(0.83f, d));
	}

	private float Entropy(){
		float sum = 0;
		foreach(KeyValuePair<Coords, Hedgehog> element in boardState){
			if(element.Value.d == 0){
				sum += 1; 
			}else{
				float p = Prob(element.Value.d);
				sum -= Mathf.Log(p, 10);
			}
		}
		return sum;
	}

	//used to calculate hedgie entropy
	public void DetermineDegreesOfFreedom(){
		int dimensions = EntropyTree.dimensions;
		foreach(Coords checkHog in boardState.Keys){
			bool hasMoveLeft = true;
			bool hasMoveRight = true;
			bool hasMoveUp = true;
			bool hasMoveDown = true;
			foreach(Coords hog in boardState.Keys){
				if((checkHog.x == 0) || ((hog.y == checkHog.y) && (hog.x < checkHog.x))){
					hasMoveLeft = false;
				}
				if((checkHog.x == dimensions - 1) || ((hog.y == checkHog.y) && hog.x > checkHog.x)){
					hasMoveRight = false;
				}
				if((checkHog.y == dimensions - 1) || ((hog.x == checkHog.x) && (hog.y > checkHog.y))){
					hasMoveUp = false;
				}
				if((checkHog.y == 0) || ((hog.x == checkHog.x) && (hog.y < checkHog.y))){
					hasMoveDown = false;
				}
			}
			if(hasMoveLeft){
				AddDegreesToAdjacentHedgehogs(new Coords(checkHog.x - 1, checkHog.y));
			}
			if(hasMoveRight){
				AddDegreesToAdjacentHedgehogs(new Coords(checkHog.x + 1, checkHog.y));
			}
			if(hasMoveUp){
				AddDegreesToAdjacentHedgehogs(new Coords(checkHog.x, checkHog.y + 1));
			}
			if(hasMoveDown){
				AddDegreesToAdjacentHedgehogs(new Coords(checkHog.x, checkHog.y - 1));		
			}
		}
	}

	//for each move there's a higher probability there will be a connection
	private void AddDegreesToAdjacentHedgehogs(Coords pos){
		if(boardState.ContainsKey(new Coords(pos.x - 1, pos.y))){
			boardState[new Coords(pos.x - 1, pos.y)].AddDegree();
		}
		if(boardState.ContainsKey(new Coords(pos.x + 1, pos.y))){
			boardState[new Coords(pos.x + 1, pos.y)].AddDegree();
		}
		if(boardState.ContainsKey(new Coords(pos.x, pos.y - 1))){
			boardState[new Coords(pos.x, pos.y - 1)].AddDegree();
		}
		if(boardState.ContainsKey(new Coords(pos.x, pos.y + 1))){
			boardState[new Coords(pos.x, pos.y + 1)].AddDegree();
		}
	}

	int mod(int x, int m) {
	    return (x%m + m)%m;
	}

	private void GetResultingBoardState(){
		// Dictionary<Coords, Hedgehog> startingBoardState = CloneDictionary(newBoardState);
		int dimensions = EntropyTree.dimensions;
		// Debug.Log(move.ToString());
		switch(mod((move.side - move.rotation), 4)){ //applys rotation so our switch statement goes back to side numbering convention
			case 0:{ // top
				moveColor = topSide[move.index];
				topSide[move.index] = -depth;
				int max = 0;
				foreach(Coords pos in boardState.Keys){
					if(pos.x == move.index && pos.y > max){
						max = pos.y;
					}
				}
				newPos = new Coords(move.index, max + 1);
				// Debug.Log(newPos);
				boardState.Add(newPos, new Hedgehog(moveColor));
				RemoveMatchedColors(newPos);
				break;
			}
			case 1:{ // right
				int adjustedIndex = dimensions - move.index - 1;//it's wonky for rotation reasons
				moveColor = rightSide[move.index];
				rightSide[move.index] = -depth;
				int max = 0;

				foreach(Coords pos in boardState.Keys){
					if(pos.y == adjustedIndex && pos.x > max){
						max = pos.x;
					}
				}
				newPos = new Coords(max + 1, adjustedIndex);
				// Debug.Log(newPos);
				boardState.Add(newPos, new Hedgehog(moveColor));
				RemoveMatchedColors(newPos);
				break;
			}
			case 2: {// bottom
				int adjustedIndex = dimensions - move.index - 1;//it's wonky for rotation reasons
				moveColor = bottomSide[move.index];
				bottomSide[move.index] = -depth;
				int min = dimensions;
				foreach(Coords pos in boardState.Keys){
					if(pos.x == adjustedIndex && pos.y < min){
						min = pos.y;
					}
				}
				newPos = new Coords(adjustedIndex, min - 1);
				// Debug.Log(newPos);
				boardState.Add(newPos, new Hedgehog(moveColor));
				RemoveMatchedColors(newPos);
				break;
			}
			case 3: {// left
				moveColor = leftSide[move.index];
				leftSide[move.index] = -depth;
				int min = dimensions;
				foreach(Coords pos in boardState.Keys){
					if(pos.y == move.index && pos.x < min){
						min = pos.x;
					}
				}
				newPos = new Coords(min - 1, move.index);
				// Debug.Log(newPos)
				boardState.Add(newPos, new Hedgehog(moveColor));
				RemoveMatchedColors(newPos);
				break;
			}
		}
	}

	//takes the dictionary with new moves, and removes colors, if there was a match
	public void RemoveMatchedColors(Coords newPos){

		int colorToMatch = boardState[newPos].color;
		bool gotAMatch = false;

        if(boardState.ContainsKey(new Coords(newPos.x - 1, newPos.y))){
			if(boardState[new Coords(newPos.x - 1, newPos.y)].color == colorToMatch){
				gotAMatch = true;
				boardState.Remove(new Coords(newPos.x - 1, newPos.y));
			}
		}
		if(boardState.ContainsKey(new Coords(newPos.x + 1, newPos.y))){
			if(boardState[new Coords(newPos.x + 1, newPos.y)].color == colorToMatch){
				gotAMatch = true;
				boardState.Remove(new Coords(newPos.x + 1, newPos.y));
			}
		}
		if(boardState.ContainsKey(new Coords(newPos.x, newPos.y - 1))){
			if(boardState[new Coords(newPos.x, newPos.y - 1)].color == colorToMatch){
				gotAMatch = true;
				boardState.Remove(new Coords(newPos.x, newPos.y - 1));
			}
		}
		if(boardState.ContainsKey(new Coords(newPos.x, newPos.y + 1))){
			if(boardState[new Coords(newPos.x, newPos.y + 1)].color == colorToMatch){
				gotAMatch = true;
				boardState.Remove(new Coords(newPos.x, newPos.y - 1));
			}
		}

        if(gotAMatch){
        	boardState.Remove(newPos);
        }
    }

    public void Debugging(){
		// foreach(KeyValuePair<Coords, Hedgehog> kvp in boardState){
		// 	Debug.Log(kvp.Value.d);
		// }
		// foreach(KeyValuePair<Coords, Hedgehog> kvp in boardState){
		// 	Debug.Log(kvp.Key);
		// }
		Debug.Log(entropy);
		// for(int k = 0; k < EntropyTree.dimensions; k++){
		// 	Debug.Log(k + ": " + NumToColor(bottomSide[k]));
		// }
    }

    public string NumToColor(int num){
		switch(num){
			case 0:
				return "red";
			case 1:
				return "orange";
			case 2:
				return "yellow";
			case 3:
				return "green";
			case 4:
				return "blue";
			case 5:
				return "purple";
			default:
				return num.ToString();

		}
	}
}

