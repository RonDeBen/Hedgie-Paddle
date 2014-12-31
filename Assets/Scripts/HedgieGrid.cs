using UnityEngine;
using System.Collections;

public class HedgieGrid : MonoBehaviour {
	private int dimensions;//grid size ex. 10 makes a 10x10 grid
	private int ballCount;//number of balls in the center
	private Vector2 tile;//width and height of each rectangle in the grid
	private Vector2[,] grid;//the centers of each tile of the grid
	private Hedgie[,] h;//the hedgie in each tile of the grid
    private HedgieSprites hsprites;
    private Vector3[] correctedScales;

	public HedgieGrid (int dimensions, GameObject HedgieObject, Camera cam, HedgieSprites hsprites){
        this.hsprites = hsprites;
		ballCount = 0;
		this.dimensions = dimensions;
		grid = new Vector2[dimensions, dimensions];
		h = new Hedgie[dimensions, dimensions];

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        tile.x = width / dimensions;
        tile.y = height / dimensions;

        grid = findGridCenters();

        for(int x = 0; x < dimensions; x++){
			for(int y = 0; y < dimensions; y++){
				GameObject go = (GameObject)Instantiate(HedgieObject, new Vector3(grid[x,y].x, grid[x,y].y, 0), Quaternion.identity);
				h[x,y] = new Hedgie(go, hsprites.getSprite(0, 0), -1, -1, -1);
			}
		}

        float newWidth = 0;
        float newHeight = 0;
        Sprite spr = new Sprite();

        correctedScales = new Vector3[hsprites.getLength()];
        for(int k = 0; k < hsprites.getLength(); k++){
        	spr = hsprites.getSprite(k, 0);
        	newWidth = tile.x / (spr.rect.width / 100);
        	newHeight = tile.y / (spr.rect.height / 100);
			correctedScales[k] = new Vector3(newWidth, newHeight, 1f);
        }

	}

	private Vector2[,] findGridCenters(){
		Vector2[,] g = new Vector2[dimensions, dimensions];
		for (int c = 0; c < dimensions; c++){
			for (int r = 0; r < dimensions; r++){
				g[r,c] = new Vector2((tile.x / 2) + (r * tile.x), (tile.y / 2) + (c * tile.y));
			}
		}
		return g;
	}

	public void pop(int x, int y){
		ballCount += h[x,y].pop();
	}

	public int getDimensions(){
		return dimensions;
	} 

	public int getBallCount(){
		return ballCount;
	}

	public int CountBalls(){
		return 0;
	}

	public void ballIncrement(){
		ballCount++;
	}

	public void ballDecrement(){
		ballCount--;
	}

    public GameObject getObject(int x, int y) {
        return h[x, y].getObject();
    }
	public Vector2 getTile(){
		return tile;
	} 

	public Vector2[,] getGrid(){
		return grid;
	}

	public Vector2 getGrid(int x, int y){
		return grid[x,y];
	}

	public Hedgie[,] getHedgie(){
		return h;
	}

	public Hedgie getHedgie(int x, int y){
		return h[x,y];
	}

	public int getColor(int x, int y){
		return h[x,y].getColor();
	}

	public int getType(int x, int y){
		return h[x,y].getType();
	}

    public int getHealth(int x, int y) {
        return h[x, y].getHealth();
    }

	public void setDimensions(int value){
		dimensions = value;
	}

	public void setBallCount(int value){
		ballCount = value;
	}

    public void setSprite(int x, int y, Sprite s) {
        h[x, y].setSprite(s);
    }
	public void setTile(Vector2 value){
		value = tile;
	} 

	public void setGrid(Vector2[,] values){
		grid = values;
	}
	public void setGrid(int x, int y, Vector2 value){
		grid[x,y] = value;
	}

	public void setHedgie(int x, int y, Hedgie value){
		h[x,y] = new Hedgie(value);
		changeLocalScale(x,y,value.getType());
	}

	public void setHedgie(Hedgie[,] values){
		h = values;
	}

    public void setHealth(int x, int y, int newHealth) {
        ballCount += h[x, y].setHealth(newHealth);
    }

    public void setText(int x, int y, string text) {
        h[x, y].setText(text);
    }

    public void setType(int x, int y, int type) {
        h[x, y].setType(type);
        h[x, y].setSprite(hsprites.getSprite(type, h[x, y].getColor()));
        changeLocalScale(x, y, type);
    }

    public void loseHealth(int x, int y, int damage) {
        ballCount += h[x, y].loseHealh(damage);
        if (h[x, y].getHealth() == 1 && h[x, y].getType() == 1) {
            h[x, y].setSprite(hsprites.getSprite(0, h[x, y].getColor()));
            changeLocalScale(x, y, 0);
        }
    }

	public void transmogrify (int x, int y, Hedgie hedgie){
		h[x,y].transmogrify(hedgie);
		changeLocalScale(x,y,hedgie.getType());
	}

	public void transmogrify(int x, int y, Sprite s, int color, int type, int health){
		h[x,y].transmogrify(s, color, type, health);
		changeLocalScale(x,y,type);
	}

	public void changeLocalScale(int x, int y, int type){
		h[x,y].getObject().transform.localScale = correctedScales[type];
	}

    public void DestroyAll(){
        for (int x = 0; x < dimensions; x++) {
                for (int y = 0; y < dimensions; y++) {
                    Destroy(h[x,y].getObject());
                }
            }
    }
}
