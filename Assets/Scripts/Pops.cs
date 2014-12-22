using UnityEngine;
using System.Collections;

public class Pops{

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

	public Pops(HedgieGrid hg){
		this.hg = hg;
		dimensions = hg.getLength();
	}

	public bool checkConnect(int x, int y){
		for (int k = -1; k <= 1; k += 2)
        {
            if ((x + k) >= 1 && (x + k) <= dimensions - 2)
            {
                if (hg.getColor(x, y) == hg.getColor(x + k, y))
                {
                    return true;
                }
            }
            if ((y + k) >= 1 && (y + k) <= dimensions - 2)
            {
                if (hg.getColor(x, y) == hg.getColor(x, y + k))
                {
                    return true;
                }
            }
            
        }

        return false;
    }

    public void Advent(int x, int y){
        if(checkConnect(x,y)){
        	if(hg.getType(x,y) == NORMAL ||  hg.getType(x,y) == ACE){
        		for (int k = -1; k <= 1; k += 2){
                    if ((x + k) >= 1 && (x + k) <= dimensions - 2){
                        if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x + k, y).getColor()){
                            hg.pop(x + k, y);                
                        }
                    }
                    if ((y + k) >= 1 && (y + k) <= dimensions - 2){
                        if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x, y + k).getColor()){
                            hg.pop(x, y + k);
                        }
                    }
                }
            hg.pop(x, y);
            }

            else if (hg.getType(x,y) == ARMOR){

            }

            else if(hg.getType(x,y) == SPLITTER){

            }

            else if(hg.getType(x,y) == CHAIN){

            }

            else if(hg.getType(x,y) == FIREBALL){

            }

            else if(hg.getType(x,y) == BOMB){

            }
        	
        }
    }

}

