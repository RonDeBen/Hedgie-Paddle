using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node{
	public float entropy;
	public Dictionary<Coords, Hedgehog> boardState;
	// public int[] leftSide, rightSide, topSide, bottomSide;
	public int[][] allSides;
	public int depth, maxDepth, moveColor, previousRotation;
	public Coords newPos;
	public Move move;
	public Node parentNode;

	public Node(){
		entropy = float.MaxValue;
		depth = -1;
	}

	public Node(Node parentNode, 
				int depth, 
				int maxDepth,
				Dictionary<Coords, Hedgehog> boardState, 
				Move move,
                int[] topSide,
				int[] rightSide, 
				int[] bottomSide,
				int[] leftSide){
		this.parentNode = parentNode;
		this.depth = depth;
		this.maxDepth = maxDepth;
		this.boardState = boardState;
		this.move = move;

		allSides = new int[][] {topSide, rightSide, bottomSide, leftSide};


		//if the node is flagged set it up for debugging stuff
		if(this.move.Equals(new Move(-1,-1,-1))){
			this.boardState = AIPops.instance.Dumb(boardState);
			DetermineDegreesOfFreedom();
			entropy = Entropy();
		}else{
			GetResultingBoardState();
			DetermineDegreesOfFreedom();
			entropy = Entropy();
		}
	}

	public Move[] GetResultantMoves(){
		Move[] moves = new Move[maxDepth];
		Node previousNode = this;
		while(previousNode.depth != -1){
			moves[previousNode.depth - 1] = previousNode.move;
			previousNode = previousNode.parentNode;
		}
		// Debug.Log(moves[0].ToString());
		// Debug.Log(moves[1].ToString());
		return moves;
	}


	//make sure that this gives the entropy
	public Node FinalEntropyNode(){
		if(depth == maxDepth){
			return this;
		}else{
			Node bestChildNode = new Node();
			Node finalEntropyNode = new Node();

			int[] topSide, rightSide, bottomSide, leftSide;
			topSide = allSides[mod(0 - move.rotation, 4)];
            rightSide = allSides[mod(1 - move.rotation, 4)];
            bottomSide = allSides[mod(2 - move.rotation, 4)];
            leftSide = allSides[mod(3 - move.rotation, 4)];

			foreach(Move newMove in EntropyTree.FindAllMoves(boardState)){
				Node newChildNode = new Node(this, (depth + 1), maxDepth,CloneDictionary(boardState), newMove, CloneArray(topSide), CloneArray(rightSide), CloneArray(bottomSide), CloneArray(leftSide));
				finalEntropyNode = newChildNode.FinalEntropyNode();
				if(finalEntropyNode.entropy < bestChildNode.entropy){
					bestChildNode = finalEntropyNode;
				}
			}
			return bestChildNode;
		}
	}

	public float FinalEntropy(){
		if(depth == maxDepth){
			return entropy;
		}else{
			float bestFutureEntropy = float.MaxValue;
			float tempEntropy = float.MaxValue;
			
			int[] topSide, rightSide, bottomSide, leftSide;
            topSide = allSides[mod(0 - move.rotation, 4)];
            rightSide = allSides[mod(1 - move.rotation, 4)];
            bottomSide = allSides[mod(2 - move.rotation, 4)];
            leftSide = allSides[mod(3 - move.rotation, 4)];

			foreach(Move newMove in EntropyTree.FindAllMoves(boardState)){
				Node newChildNode = new Node(this, (depth + 1), maxDepth, CloneDictionary(boardState), newMove, CloneArray(topSide), CloneArray(rightSide), CloneArray(bottomSide), CloneArray(leftSide));
				tempEntropy = newChildNode.FinalEntropy();
				if(tempEntropy == 0){
					return 0;
				}
				if(tempEntropy < bestFutureEntropy){
					bestFutureEntropy = tempEntropy;
				}
			}
			return bestFutureEntropy;
		}
	}

	private Dictionary<Coords, Hedgehog> CloneDictionary(Dictionary<Coords, Hedgehog> startDict){
		Dictionary<Coords, Hedgehog> newDict = new Dictionary<Coords, Hedgehog>();
		foreach(KeyValuePair<Coords, Hedgehog> kvp in startDict){
			newDict.Add(new Coords(kvp.Key.x, kvp.Key.y), new Hedgehog(kvp.Value.color, kvp.Value.health, kvp.Value.type));
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

	private Move CloneMove(Move move){
		return new Move(move.side, move.rotation, move.index);
	}

	public override string ToString(){
		return ("Entropy: " + entropy + ", Move: " + move.ToString() + " color: " + NumToColor(moveColor) + " Count: " + boardState.Count);
	} 

	private float Prob(Hedgehog heg){
		//1 - (5/6)^#num_rolls
		return Prob(heg.d, heg.health);
	}

	private float Prob(int d, int health){
		return Mathf.Pow((1 - Mathf.Pow(0.83f, d)), health);
	}

	private float Entropy(){
		float sum = 0;
		Dictionary<int, List<Hedgehog>> split_dict = new Dictionary<int, List<Hedgehog>>();
		foreach(KeyValuePair<Coords, Hedgehog> element in boardState){
			if(element.Value.split_id == -1){
				if(element.Value.d == 0){
					sum += 1; 
				}else{
					float p = Prob(element.Value);
					sum -= Mathf.Log(p, 10);
				}
			}else{
				try{
                    split_dict[element.Value.split_id].Add(element.Value);
                }catch (KeyNotFoundException){
                    List<Hedgehog> newList = new List<Hedgehog>() {element.Value};
                    split_dict.Add(element.Value.split_id, newList);
                }
			}
		}
		foreach(KeyValuePair<int, List<Hedgehog>> entry in split_dict){
			int split_d = 0;
			foreach(Hedgehog heg in entry.Value){
				split_d += heg.d;
			}
			if(split_d == 0){
				sum += entry.Value.Count;
			}else{
				float p = Prob(split_d, entry.Value[0].health);
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

	public int HedgieCount(){
		return boardState.Count();
	}

	int mod(int x, int m) {
	    return (x%m + m)%m;
	}

	public void GetResultingBoardState(){
        int dimensions = EntropyTree.dimensions;
		int trueRotation = mod((move.side - move.rotation), 4);//applies rotation so our switch statement goes back to side numbering convention
        moveColor = allSides[trueRotation][move.index];
        switch(move.side){ 
			case 0:{ // top
				if(moveColor > -1){
					allSides[trueRotation][move.index] = -depth;
					int max = 0;
					foreach(Coords pos in boardState.Keys){
						if(pos.x == move.index && pos.y > max){
							max = pos.y;
						}
					}
					newPos = new Coords(move.index, max + 1);
					boardState.Add(newPos, new Hedgehog(moveColor, 1, 0));//change if outer hedgies can be special
					RemoveMatchedColors(newPos);
				}
				break;
			}
			case 1:{ // right
				int adjustedIndex = dimensions - move.index - 1;//it's wonky for rotation reasons
				if(moveColor > -1){
					allSides[trueRotation][move.index] = -depth;
					int max = 0;
					foreach(Coords pos in boardState.Keys){
						if(pos.y == adjustedIndex && pos.x > max){
							max = pos.x;
						}
					}
					newPos = new Coords(max + 1, adjustedIndex);
					boardState.Add(newPos, new Hedgehog(moveColor, 1, 0));//change if outer hedgies can be special
					RemoveMatchedColors(newPos);
				}
				break;
			}
			case 2: {// bottom
				int adjustedIndex = dimensions - move.index - 1;//it's wonky for rotation reasons
				if(moveColor > -1){
					allSides[trueRotation][move.index] = -depth;
					int min = dimensions;
					foreach(Coords pos in boardState.Keys){
						if(pos.x == adjustedIndex && pos.y < min){
							min = pos.y;
						}
					}
					newPos = new Coords(adjustedIndex, min - 1);
					boardState.Add(newPos, new Hedgehog(moveColor, 1, 0));//change if outer hedgies can be special
					RemoveMatchedColors(newPos);
				}
				break;
			}
			case 3: {// left
				if(moveColor > -1){
					allSides[trueRotation][move.index] = -depth;
					int min = dimensions;
					foreach(Coords pos in boardState.Keys){
						if(pos.y == move.index && pos.x < min){
							min = pos.x;
						}
					}
					newPos = new Coords(min - 1, move.index);
					boardState.Add(newPos, new Hedgehog(moveColor, 1, 0));//change if outer hedgies can be special
					RemoveMatchedColors(newPos);
				}
				break;
			}
		}
	}

	public void RemoveMatchedColors(Coords newPos){
		boardState = AIPops.instance.GetResultingBoardState(boardState, newPos);
	}

	//takes the dictionary with new moves, and removes colors, if there was a match
	// public void RemoveMatchedColors(Coords newPos){
	// 	if(EntropyTree.instance.UseAIPops){
	// 		boardState = AIPops.instance.GetResultingBoardState(boardState, newPos);
	// 	}else{
	// 		int colorToMatch = boardState[newPos].color;
	// 		bool gotAMatch = false;
	// 		Dictionary<Coords, Hedgehog> boardCopy = CloneDictionary(boardState);

	// 		if(boardState.ContainsKey(new Coords(newPos.x - 1, newPos.y))){
	// 			if(boardState[new Coords(newPos.x - 1, newPos.y)].color == colorToMatch){
	// 				gotAMatch = true;
	// 				if(boardState[new Coords(newPos.x - 1, newPos.y)].health > 1){
	// 					boardState[new Coords(newPos.x - 1, newPos.y)].TakeDamage();
	// 				}else{
	// 					boardState.Remove(new Coords(newPos.x - 1, newPos.y));
	// 				}
	// 			}
	// 		}
	// 		if(boardState.ContainsKey(new Coords(newPos.x + 1, newPos.y))){
	// 			if(boardState[new Coords(newPos.x + 1, newPos.y)].color == colorToMatch){
	// 				gotAMatch = true;
	// 				if(boardState[new Coords(newPos.x + 1, newPos.y)].health > 1){
	// 					boardState[new Coords(newPos.x + 1, newPos.y)].TakeDamage();
	// 				}else{
	// 					boardState.Remove(new Coords(newPos.x + 1, newPos.y));
	// 				}
	// 			}
	// 		}
	// 		if(boardState.ContainsKey(new Coords(newPos.x, newPos.y - 1))){
	// 			if(boardState[new Coords(newPos.x, newPos.y - 1)].color == colorToMatch){
	// 				gotAMatch = true;
	// 				if(boardState[new Coords(newPos.x, newPos.y - 1)].health > 1){
	// 					boardState[new Coords(newPos.x, newPos.y - 1)].TakeDamage();
	// 				}else{
	// 					boardState.Remove(new Coords(newPos.x, newPos.y - 1));
	// 				}
	// 			}
	// 		}
	// 		if(boardState.ContainsKey(new Coords(newPos.x, newPos.y + 1))){
	// 			if(boardState[new Coords(newPos.x, newPos.y + 1)].color == colorToMatch){
	// 				gotAMatch = true;
	// 				if(boardState[new Coords(newPos.x, newPos.y + 1)].health > 1){
	// 					boardState[new Coords(newPos.x, newPos.y + 1)].TakeDamage();
	// 				}else{
	// 					boardState.Remove(new Coords(newPos.x, newPos.y + 1));
	// 				}
	// 			}
	// 		}

	// 		if(gotAMatch){
	// 			if(boardState[newPos].health > 1){
	// 				boardState[newPos].TakeDamage();
	// 			}else{
	// 				boardState.Remove(newPos);
	// 			}
	// 		}
	// 	}
    // }

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

	public int GetNumPossibleMoves(){
		return EntropyTree.FindAllMoves(boardState).Count;
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

