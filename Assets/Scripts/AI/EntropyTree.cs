using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntropyTree : MonoBehaviour {

	public static EntropyTree instance;

	public static int dimensions;
	public Dictionary<Coords, Hedgehog> currentBoardState = new Dictionary<Coords, Hedgehog>();
	public static int[] leftSide, rightSide, topSide, bottomSide;
	private int colors = 6;
	// private AIPops aip;

	// Use this for initialization
	void Awake () {
		if(instance != null){
			GameObject.Destroy(instance);
		}
		instance = this;
	}

	//used to keep boardstate current
	public void AddHedgehog(int x, int y, int color){
		// Debug.Log((x - 1) + ", " + (y - 1));
		// if(!currentBoardState.ContainsKey(new Coords(x, y))){
			currentBoardState.Add(new Coords(x - 1, y - 1), new Hedgehog(color));
		// }
	}

	//used to keep boardstate current
	public void RemoveHedgehog(int x , int y){
		// Debug.Log((x - 1) + ", " + (y - 1));
		currentBoardState.Remove(new Coords(x - 1, y - 1));
	}

	public void SetOuterHedgehog(int x, int y, int color){
		if(x == 0){//left side
			leftSide[y - 1] = color;
		}else if(x == dimensions+1){//right side
			rightSide[dimensions - y] = color;
		}else if(y == 0){//bottom side
			bottomSide[dimensions - x] = color;
		}else{//top side
			topSide[x-1] = color;
		}
	}

	//need to know dimension for grid calculations
	public static void SetDimensions(int number){
		dimensions = number - 2;
		leftSide = new int[dimensions];
		rightSide = new int[dimensions];
		topSide = new int[dimensions];
		bottomSide = new int[dimensions];
	}

	public Move FindNextMove(){
		return FindNextMove(currentBoardState);
	}

	public Move FindNextMove(Dictionary<Coords, Hedgehog> boardState){
		Node bestNode = new Node();
		foreach(Move move in FindAllMoves(boardState)){
			//change this later when you add depth
			Node newNode = new Node(boardState, move, leftSide, rightSide, topSide, bottomSide);
			// Debug.Log(newNode.ToString());
			if(newNode.entropy < bestNode.entropy){
				bestNode = newNode;
				// Debug.Log(bestNode.ToString());
			}
		}
		// Debug.Log("Best: " + bestNode.ToString());
		// Debug.Log(bestNode.ToString());
		return bestNode.move;
	}

	public List<Move> FindAllMoves(Dictionary<Coords, Hedgehog> boardState){
		List<Move> allMoves = new List<Move>();
		foreach(Coords checkHog in boardState.Keys){
			bool hasMoveLeft = true;
			bool hasMoveRight = true;
			bool hasMoveUp = true;
			bool hasMoveDown = true;
			foreach(Coords hog in boardState.Keys){
				//on the left side, or there's a hog with a smaller x
				if(checkHog.x == 0 || (hog.y == checkHog.y && hog.x < checkHog.x)){
					hasMoveLeft = false;
				}
				//on the right side, or there's a hog with a larger x
				if(checkHog.x == (dimensions - 1) || (hog.y == checkHog.y && hog.x > checkHog.x)){
					hasMoveRight = false;
				}
				//on the top side, or there's a hog with a larger y
				if(checkHog.y == (dimensions - 1) || (hog.x == checkHog.x && hog.y > checkHog.y)){
					hasMoveUp = false;
				}
				//on the bottom side, or there's a hog with a smaller y
				if(checkHog.y == 0 || (hog.x == checkHog.x && hog.y < checkHog.y)){
					hasMoveDown = false;
				}
			}
			if(dimensions % 2 == 0){
				if(hasMoveLeft){
					allMoves.AddRange(new List<Move>() {new Move(3, 0, checkHog.y),
														new Move(3, 1, checkHog.y),
														new Move(3, -1, checkHog.y),
														new Move(3, 2, checkHog.y)});
					// allMoves.Add(new Move(3, 0, checkHog.y));
				}
				if(hasMoveRight){
					allMoves.AddRange(new List<Move>() {new Move(1, 0, dimensions - checkHog.y - 1),
														new Move(1, 1, dimensions - checkHog.y - 1),
														new Move(1, -1, dimensions - checkHog.y - 1),
														new Move(1, 2, dimensions - checkHog.y - 1)});
					// allMoves.Add(new Move(1, 0, dimensions - checkHog.y - 1));
				}
				if(hasMoveUp){
					allMoves.AddRange(new List<Move>() {new Move(0, 0, checkHog.x),
														new Move(0, 1, checkHog.x),
														new Move(0, -1, checkHog.x),
														new Move(0, 2, checkHog.x)});
					// allMoves.Add(new Move(0, 0, checkHog.x));
				}
				if(hasMoveDown){
					allMoves.AddRange(new List<Move>() {new Move(2, 0, dimensions - checkHog.x - 1),
														new Move(2, 1, dimensions - checkHog.x - 1),
														new Move(2, -1, dimensions - checkHog.x - 1),
														new Move(2, 2, dimensions - checkHog.x - 1)});	
					// allMoves.Add(new Move(2, 0, dimensions - checkHog.x - 1));
				}
			}
		}
		// foreach(Move move in allMoves){
		// 	Debug.Log(move.ToString());
		// }
		return allMoves;
	}

	public void Debugging(){
		// for(int k = 0; k < dimensions; k++){
		// 	Debug.Log(k + ": " + NumToColor(topSide[k]));
		// }
		
		Node newNode = new Node(currentBoardState, new Move(-1,-1,-1), leftSide, rightSide, topSide, bottomSide);
		newNode.DetermineDegreesOfFreedom();
	}

	public void DisplayHedgehogs(){
		foreach(KeyValuePair<Coords, Hedgehog> kvp in currentBoardState){
			Debug.Log(kvp.Key + " " + NumToColor(kvp.Value.color));
		}
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
