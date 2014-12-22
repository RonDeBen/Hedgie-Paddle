using UnityEngine;
using System.Collections;

public class Taps{

	public struct Coords{
        public int x, y;

        public Coords(int x, int y){
            this.x = x;
            this.y = y;
        }
    }

	private const int NORMAL = 0;
	private const int ARMOR = 1;
	private const int SPLITTER = 2;
	private const int ACE = 3;
	private const int CHAIN = 4;
	private const int FIREBALL = 5;
	private const int BOMB = 6;

	private HedgieGrid hg;
	private int dimensions;

	public Taps(HedgieGrid hg){
		this.hg = hg;
		dimensions = hg.getLength();
	}


	public Vector2 Here(int x, int y){
		if(hg.getType(x, y) != ACE){
			if (x == 0 && y != 0 && y != dimensions - 1){//if the left corner is clicked
	        	int check = checkRight(y);
	        	if (check != -1)
	            	return new Vector2(check, y);
	    	}

		    if (x == dimensions - 1 && y != 0 && y != dimensions - 1){//if the right corner is clicked
		        int check = checkLeft(y);
		        if (check != -1)
		           return new Vector2(check, y);
		    }

		    if (y == 0 && x != 0 && x != dimensions - 1){//if the bottom corner is clicked
		        int check = checkUp(x);
		        if (check != -1)
		            return new Vector2(x, check);
		    }

		    if (y == dimensions - 1 && x != 0 && x != dimensions - 1){//if the top corner is clicked
		        int check = checkDown(x);
		        if (check != -1)
		            return new Vector2(x, check);
		    }

		 }else{
		 	//DO SOME PROTOCOL FOR ACE
		 }

		 return new Vector2(-1,-1);
	}

	private int checkRight(int y)
    {
        if (hg.getColor(1, y) != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = 2; k < dimensions - 1; k++)
            {
                if (hg.getColor(k, y) != -1) 
                    return k - 1;
            }
            return -1;
        }
    }

    private int checkLeft(int y)
    {
        if (hg.getColor(dimensions - 2, y) != -1)//if there's a ball in front of the ball you're shooting
            return -1;
        else
        {
            for (int k = dimensions - 3; k > 0; k--)//checks sequentially 
            {
                if (hg.getColor(k, y) != -1)
                    return k + 1;
            }
            return -1;//no balls on that row
        }
    }

    private int checkUp(int x)
    {
        if (hg.getColor(x, 1) != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = 2; k < dimensions - 1; k++)//checks sequentially above where was clicked
            {
                if (hg.getColor(x, k) != -1)
                    return k - 1;//returns the y value before the first ball encountered 
            }
            return -1;
        }
    }

    private int checkDown(int x)
    {
        if (hg.getColor(x, dimensions - 2) != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = dimensions - 3; k > 0; k--)//checks sequentially below where was clicked 
            {
                if (hg.getColor(x, k) != -1)
                    return k + 1;//returns the y value before the first ball encountered
            }
            return -1;
        }
    }

}
