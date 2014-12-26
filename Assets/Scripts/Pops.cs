using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		dimensions = hg.getDimensions();
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
        if(checkConnect(x, y)){//don't do nuffin if there aint no connections

            List<Coords> hits = new List<Coords>();
            int derp = 0;
        	for (int k = -1; k <= 1; k += 2){//checks each direction for matching colors, and stores each match in a list of coordinates
                if ((x + k) >= 1 && (x + k) <= dimensions - 2){
                    if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x + k, y).getColor()){
                        hits.Add(new Coords(x + k, y));
                    }
                }
                if ((y + k) >= 1 && (y + k) <= dimensions - 2){
                    if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x, y + k).getColor()){
                        hits.Add(new Coords(x, y + k));
                    }
                }
            }

            if (hg.getType(x, y) == SPLITTER) {
                derp = hg.getHealth(x, y) - 1;
            }

            if (hg.getType(x, y) == NORMAL || hg.getType(x, y) == ACE || hg.getType(x, y) == ARMOR) {
                int max = 0;
                bool gotone = false;
                foreach (Coords hit in hits) {
                    if (hg.getType(hit.x, hit.y) == SPLITTER && hg.getColor(hit.x, hit.y) == hg.getColor(x, y)) {
                        gotone = true;
                        if(max < hg.getHealth(hit.x, hit.y)){
                            max = hg.getHealth(hit.x, hit.y);
                        }
                    }
                }
                if(gotone){
                    splittify(x, y, max);
                    derp = -1;
                }
            }



            if (hg.getType(x, y) == NORMAL || hg.getType(x, y) == ACE) {
                foreach (Coords hit in hits) {
                    if (hg.getType(hit.x, hit.y) == NORMAL) {//If a normal/ace hits a normal
                        loseHealth(hit.x, hit.y, -(hits.Count));
                    }
                    else if (hg.getType(hit.x, hit.y) == ARMOR) {//If a normal/ace hits an armor
                        loseHealth(hit.x, hit.y, -(hits.Count));
                    }
                    else if (hg.getType(hit.x, hit.y) == SPLITTER) {//If a normal/ace hits a splitter
                        split(hit.x, hit.y, hg.getColor(hit.x, hit.y), hg.getHealth(hit.x, hit.y) - 1);
                    }
                    else if (hg.getType(hit.x, hit.y) == FIREBALL) {//If a normal/ace hits a fireball
                        fireVector(hit.x, hit.y);
                    }
                    else if (hg.getType(hit.x, hit.y) == BOMB) {//If a normal/ace hits a bomb
                        bomb(hit);
                    }

                }
                if (hg.getType(x, y) != SPLITTER && hg.getType(x, y) != -1) {
                    loseHealth(x, y, -(hits.Count));
                }
            }
                
            else if (hg.getType(x, y) == ARMOR) {
                foreach (Coords hit in hits) {
                    if (hg.getType(hit.x, hit.y) == NORMAL) {//If an armor hits a normal
                        hg.pop(hit.x, hit.y);
                        loseHealth(x, y, -(hits.Count));
                    }
                    else if (hg.getType(hit.x, hit.y) == ARMOR) {//If an armor hits an armor
                        loseHealth(hit.x, hit.y, -(hits.Count));
                        loseHealth(x, y, -(hits.Count));
                    }
                    else if (hg.getType(hit.x, hit.y) == SPLITTER) {//If an armor hits a splitter
                        split(hit.x, hit.y, hg.getColor(hit.x, hit.y), hg.getHealth(hit.x, hit.y) - 1);
                    }
                    else if (hg.getType(hit.x, hit.y) == FIREBALL) {//If an armor hits a fireball

                    }
                    else if (hg.getType(hit.x, hit.y) == BOMB) {//If an armor hits a bomb

                    }

                }
            }

            else if (hg.getType(x, y) == SPLITTER) {
                bool doubledip = false;
                foreach (Coords hit in hits) {
                    if (hg.getType(hit.x, hit.y) == NORMAL) {//If a splitter hits a normal
                        splittify(hit.x, hit.y, hg.getHealth(x, y));
                        split(hit.x, hit.y, hg.getColor(hit.x, hit.y), hg.getHealth(hit.x, hit.y) - 1);
                    }
                    else if (hg.getType(hit.x, hit.y) == ARMOR) {//If a splitter hits an armor
                        splittify(hit.x, hit.y, hg.getHealth(x, y));
                        split(hit.x, hit.y, hg.getColor(hit.x, hit.y), hg.getHealth(hit.x, hit.y) - 1);
                    }
                    else if (hg.getType(hit.x, hit.y) == SPLITTER) {//If a splitter hits a splitter
                        if (!doubledip) {
                            split(x, y, hg.getColor(hit.x, hit.y), hg.getHealth(hit.x, hit.y) + derp);
                            doubledip = true;
                        }
                        
                    }
                    else if (hg.getType(hit.x, hit.y) == FIREBALL) {//If a splitter hits a fireball

                    }
                    else if (hg.getType(hit.x, hit.y) == BOMB) {//If a splitter hits a bomb

                    }

                }
            }

            else if (hg.getType(x, y) == FIREBALL) {
                foreach (Coords hit in hits) {
                    if (hg.getType(hit.x, hit.y) == NORMAL) {//If a fireball hits a normal

                    }
                    else if (hg.getType(hit.x, hit.y) == ARMOR) {//If a fireball hits an armor

                    }
                    else if (hg.getType(hit.x, hit.y) == SPLITTER) {//If a fireball hits a splitter

                    }
                    else if (hg.getType(hit.x, hit.y) == FIREBALL) {//If a fireball hits a fireball

                    }
                    else if (hg.getType(hit.x, hit.y) == BOMB) {//If a fireball hits a bomb

                    }

                }
            }

            else if (hg.getType(x, y) == BOMB) {
                foreach (Coords hit in hits) {
                    if (hg.getType(hit.x, hit.y) == NORMAL) {//If a bomb hits a normal

                    }
                    else if (hg.getType(hit.x, hit.y) == ARMOR) {//If a bomb hits an armor

                    }
                    else if (hg.getType(hit.x, hit.y) == SPLITTER) {//If a bomb hits a splitter

                    }
                    else if (hg.getType(hit.x, hit.y) == FIREBALL) {//If a bomb hits a fireball

                    }
                    else if (hg.getType(hit.x, hit.y) == BOMB) {//If a bomb hits a bomb

                    }

                }
            }
        }


    }

    private void loseHealth(int x, int y, int damage) {
        hg.loseHealth(x, y, damage);
    }

    /*private void split(int x, int y, int color, int newHealth) {
        for (int r = -1; r <= 1; r++) {
            for (int c = -1; c <= 1; c++) {
                if ((x + r) >= 1 && (x + r) <= (dimensions - 2) && (y + c) >= 1 && (y + c) <= (dimensions - 2)) {
                    if (hg.getType(x + r, y + c) == SPLITTER && hg.getColor(x + r, y + c) == color && hg.getHealth(x + r, y + c) != newHealth) {
                        hg.setHealth(x + r, y + c, newHealth);
                        split(x + r, y + c, color, newHealth);
                    }
                }
            }
        }
    }*/

    private void split(int x, int y, int color, int newHealth){
        if (hg.getColor(x, y) == color) {
            if (hg.getType(x, y) == SPLITTER){
                if (hg.getHealth(x, y) != newHealth) {
                    hg.setHealth(x, y, newHealth);

                    for (int k = -1; k <= 1; k += 2) {
                        if ((x + k) >= 1 && (x + k) <= (dimensions - 2)) {
                            split(x + k, y, color, newHealth);
                        }
                        if ((y + k) >= 1 && (y + k) <= (dimensions - 2)) {
                            split(x, y + k, color, newHealth);
                        }
                        }
                    }
                }
            else if (hg.getType(x, y) != -1) {
                newHealth--;
                if (newHealth <= 0) {
                    splittify(x, y, 1);
                }
                else {
                    splittify(x, y, newHealth);
                }
                split(x, y, color, newHealth);
            }
        }
    }
        
    

    private void splittify(int x, int y, int health) {
        hg.setType(x, y, SPLITTER);
        hg.setHealth(x, y, health);
    }

    private void fireVector(int x, int y) {

    }

    private void bomb(Coords pos) {
        int color = hg.getColor(pos.x, pos.y);
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if((pos.x + x) > 0 && (pos.x + x) < dimensions -1 && (pos.y + y) > 0 && (pos.y + y) < dimensions -1){
                    if (color == hg.getColor(pos.x + x, pos.y + y)) {
                        hg.loseHealth(pos.x + x, pos.y + y, -1);//you might need to change this
                    }
                }
            }
        }
    }


}

