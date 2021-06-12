using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntropyTree : MonoBehaviour {

	public static EntropyTree instance;

	public static int dimensions;
	public Dictionary<Coords, Hedgehog> currentBoardState = new Dictionary<Coords, Hedgehog>();
	public static int[] leftSide, rightSide, topSide, bottomSide;
	private int colors = 6;
	public bool UseAIPops = false;
	// private AIPops aip;

	// Use this for initialization
	void Awake () {
		if(instance != null){
			GameObject.Destroy(instance);
		}
		instance = this;
	}

	//used to keep boardstate current
	public void AddHedgehog(int x, int y, int color, int health, int type){
		// Debug.Log((x - 1) + ", " + (y - 1));
		if(!currentBoardState.ContainsKey(new Coords(x - 1, y - 1))){
			currentBoardState.Add(new Coords(x - 1, y - 1), new Hedgehog(color, health, type));
		}
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

	public List<Move> FindMoves(Hedgie[,] hg){
		SetBoardState(hg);
		int possibleMoves = GetNumPossibleMoves();

		if(possibleMoves > 0){
			float currentEntropy = GetCurrentEntropy();
			Node bestNode = FindNextNodeWithDepth(1, currentEntropy);
			if(bestNode.entropy <= currentEntropy || bestNode.GetNumPossibleMoves() == 0){
				return new List<Move> { bestNode.move };
			}else{
				bestNode = FindNextNodeWithDepth(2, currentEntropy);
				if(bestNode.entropy <= currentEntropy || bestNode.GetNumPossibleMoves() == 0){
					return new List<Move> { bestNode.parentNode.move, bestNode.move };
				}else{
					bestNode = FindNextNodeWithDepth(3, currentEntropy);
					return new List<Move> {bestNode.parentNode.parentNode.move, bestNode.parentNode.move, bestNode.move};
				}
			}
		}
		return null;
	}

    public Node FindNextNodeWithDepth(int depth, float currentEntropy) {
        Node bestNode = new Node();
        foreach (Move move in FindAllMoves(currentBoardState)) {
            Node newNode = new Node(new Node(), 1, depth, CloneDictionary(currentBoardState), move, CloneArray(topSide), CloneArray(rightSide), CloneArray(bottomSide), CloneArray(leftSide));
			Node finalEntropyNode = newNode.FinalEntropyNode();
            if (bestNode.entropy > finalEntropyNode.entropy) {
                bestNode = finalEntropyNode;
            }
        }
        // Debug.Log(bestNode.move + " => " + bestNode.HedgieCount());
        return bestNode;
    }

	public static List<Move> FindAllMoves(Dictionary<Coords, Hedgehog> boardState){
		List<Move> allMoves = new List<Move>();
		foreach(Coords checkHog in boardState.Keys){
			// Debug.Log(checkHog.x + ", " + checkHog.y);
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
			// if(dimensions % 2 == 0){
				if(hasMoveLeft){
					allMoves.AddRange(new List<Move>() {new Move(3, 0, checkHog.y),
														new Move(3, 1, checkHog.y),
														new Move(3, 3, checkHog.y),
														new Move(3, 2, checkHog.y)});
					// allMoves.Add(new Move(3, 0, checkHog.y));
				}
				if(hasMoveRight){
					allMoves.AddRange(new List<Move>() {new Move(1, 0, dimensions - checkHog.y - 1),
														new Move(1, 1, dimensions - checkHog.y - 1),
														new Move(1, 3, dimensions - checkHog.y - 1),
														new Move(1, 2, dimensions - checkHog.y - 1)});
					// allMoves.Add(new Move(1, 0, dimensions - checkHog.y - 1));
				}
				if(hasMoveUp){
					allMoves.AddRange(new List<Move>() {new Move(0, 0, checkHog.x),
														new Move(0, 1, checkHog.x),
														new Move(0, 3, checkHog.x),
														new Move(0, 2, checkHog.x)});
					// allMoves.Add(new Move(0, 0, checkHog.x));
				}
				if(hasMoveDown){
					allMoves.AddRange(new List<Move>() {new Move(2, 0, dimensions - checkHog.x - 1),
														new Move(2, 1, dimensions - checkHog.x - 1),
														new Move(2, 3, dimensions - checkHog.x - 1),
														new Move(2, 2, dimensions - checkHog.x - 1)});	
					// allMoves.Add(new Move(2, 0, dimensions - checkHog.x - 1));
				}
			// }
		}
		// foreach(Move move in allMoves){
		// 	Debug.Log(move.ToString());
		// }
		// Debug.Log(allMoves.Count);
		return allMoves;
	}

	public int GetNumPossibleMoves(){
		// Debug.Log(FindAllMoves(currentBoardState).Count);
		return FindAllMoves(currentBoardState).Count;
	}

	public float GetCurrentEntropy(){
        Node tempParent = new Node();
        Node newNode = new Node(tempParent, 0, 0, currentBoardState, new Move(-1, -1, -1), topSide, rightSide, bottomSide, leftSide);
		return newNode.entropy;
	}

	public void Debugging(){
		// for(int k = 0; k < dimensions; k++){
		// 	Debug.Log(k + ": " + NumToColor(topSide[k]));
		// }
		// Debug.Log(GetNumPossibleMoves());
		// Debug.Log(currentBoardState.Count);
		Node tempParent = new Node();
		Node newNode = new Node(tempParent, 0, 0, currentBoardState, new Move(-1,-1,-1), topSide, rightSide, bottomSide, leftSide);
		newNode.Debugging();
		// newNode.DetermineDegreesOfFreedom();
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

    private Dictionary<Coords, Hedgehog> CloneDictionary(Dictionary<Coords, Hedgehog> startDict) {
        Dictionary<Coords, Hedgehog> newDict = new Dictionary<Coords, Hedgehog>();
        foreach (KeyValuePair<Coords, Hedgehog> kvp in startDict) {
            newDict.Add(new Coords(kvp.Key.x, kvp.Key.y), new Hedgehog(kvp.Value.color, kvp.Value.health, kvp.Value.type));
        }
        return newDict;
    }

    private int[] CloneArray(int[] startArray) {
        int[] newArray = new int[startArray.Length];
        for (int k = 0; k < startArray.Length; k++) {
            newArray[k] = startArray[k];
        }
        return newArray;
    }

	public int GetBallCount(){
		return currentBoardState.Count;
	}

	public void SetBoardState(Hedgie[,] h){
		currentBoardState.Clear();
		for(int x = 1; x < dimensions + 1; x++){
			for(int y = 1; y < dimensions + 1; y++){
				if(h[x,y].color != -1){
					currentBoardState.Add(new Coords(x - 1, y - 1), new Hedgehog(h[x,y].color, h[x,y].health, h[x,y].type));
				}
			}
		}
    }
}
