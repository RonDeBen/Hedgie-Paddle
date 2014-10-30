﻿using UnityEngine;
using System.Collections;

public class HedgieGrid {
	private int dimensions;//grid size ex. 10x10
	private int ballCount;//number of balls in the center
	private Vector2 tile;//width and height of each rectangle in the grid
	private Vector2[,] grid;//the centers of each tile of the grid
	private Hedgie[,] h;//the hedgie in each tile of the grid

	public HedgieGrid (int dimensions, int innerBalls, Camera cam){
		ballCount = innerBalls;
		this.dimensions = dimensions;
		grid = new Vector2[dimensions, dimensions];
		h = new Hedgie[dimensions, dimensions];
		h.Initialize();

		for (int k = 0; k < dimensions; k++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                h[k, j] = new Hedgie();
            }
        }

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

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
		h[x,y].pop();
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

	public void setDimensions(int value){
		dimensions = value;
	}

	public void setBallCount(int value){
		ballCount = value;
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
		h[x,y] = value;
	}

	public void setHedgie(Hedgie[,] values){
		h = values;
	}

}
