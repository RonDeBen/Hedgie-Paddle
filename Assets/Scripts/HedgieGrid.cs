using UnityEngine;
using System.Collections;

public class HedgieGrid {
	private int dimensions;//grid size ex. 10 makes a 10x10 grid
	private int ballCount;//number of balls in the center
	private Vector2 tile;//width and height of each rectangle in the grid
	private Vector2[,] grid;//the centers of each tile of the grid
	private Hedgie[,] h;//the hedgie in each tile of the grid
    private HedgieSprites hsprites;

	public HedgieGrid (int dimensions, int innerBalls, Camera cam, HedgieSprites hsprites){
        this.hsprites = hsprites;
		ballCount = 0;
		this.dimensions = dimensions;
		grid = new Vector2[dimensions, dimensions];
		h = new Hedgie[dimensions, dimensions];
		h.Initialize();

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        if(height > width){
        	float temp = height;
        	height = width;
        	width = temp;
        }

        tile.x = width / dimensions;
        tile.y = height / dimensions;

        grid = findGridCenters();
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

    public GameObject getGameObject(int x, int y) {
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
    }

    public void loseHealth(int x, int y, int damage) {
        ballCount += h[x, y].loseHealh(damage);
        if (h[x, y].getHealth() == 1 && h[x, y].getType() != 2) {
            h[x, y].setSprite(hsprites.getSprite(0, h[x, y].getColor()));
        }
    }

	public void transmogrify (int x, int y, Hedgie hedge){
		h[x,y].transmogrify(hedge);
	}

	public void transmogrify(int x, int y, Sprite s, int color, int type, int health){
		h[x,y].transmogrify(s, color, type, health);
	}
}
