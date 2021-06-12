using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIPops: MonoBehaviour {

    public static AIPops instance;

    void Awake() {
        if (instance != null) {
            GameObject.Destroy(instance);
        }
        instance = this;
    }

	private const int NORMAL = 0;
	private const int ARMOR = 1;
	private const int SPLITTER = 2;
	private const int ACE = 3;
	private const int CHAIN = 4;
	private const int FIREBALL = 5;
	private const int BOMB = 6;

	// private HedgieGrid hg;
    public Hedgehog[,] hg;
	private int dimensions;
    // public List<int> split_ids = new List<int>();
    public int id_counter = 0;

    public Dictionary<Coords, Hedgehog> GetResultingBoardState(Dictionary<Coords, Hedgehog> startState, Coords newPos){
        id_counter = 0;
        dimensions = EntropyTree.dimensions;
        hg = new Hedgehog[dimensions, dimensions];
        for(int i = 0; i < dimensions; i++){
            for(int j = 0; j < dimensions; j++){
                hg[i, j] = new Hedgehog(-1, -1, -1);
            }
        }
        foreach(Coords key in startState.Keys){
            Hedgehog newHeg = new Hedgehog(startState[key]);
            if(newHeg.getType() == SPLITTER && newHeg.getSplitId() == -1){
                newHeg.setSplitId(id_counter);
                id_counter++;
            }
            hg[key.x, key.y] = newHeg;
        }
        Advent(newPos.x, newPos.y);
        Dictionary<Coords, Hedgehog> newState = new Dictionary<Coords, Hedgehog>();
        for (int x = 0; x < dimensions; x++) {
            for (int y = 0; y < dimensions; y++) {
                if (hg[x, y].color != -1) {
                    newState.Add(new Coords(x, y), new Hedgehog(hg[x,y]));
                }
            }
        }
        return newState;
    }

    public Dictionary<Coords, Hedgehog> Dumb(Dictionary<Coords, Hedgehog> startState){
        id_counter = 0;
        dimensions = EntropyTree.dimensions;
        hg = new Hedgehog[dimensions, dimensions];
        for (int i = 0; i < dimensions; i++) {
            for (int j = 0; j < dimensions; j++) {
                hg[i, j] = new Hedgehog(-1, -1, -1);
            }
        }
        foreach (Coords key in startState.Keys) {
            Hedgehog newHeg = new Hedgehog(startState[key]);
            if (newHeg.getType() == SPLITTER && newHeg.getSplitId() == -1) {
                newHeg.setSplitId(id_counter);
                id_counter++;
            }
            hg[key.x, key.y] = newHeg;
        }
        // Advent(newPos.x, newPos.y);
        Dictionary<Coords, Hedgehog> newState = new Dictionary<Coords, Hedgehog>();
        for (int x = 0; x < dimensions; x++) {
            for (int y = 0; y < dimensions; y++) {
                if (hg[x, y].color != -1) {
                    newState.Add(new Coords(x, y), new Hedgehog(hg[x, y]));
                }
            }
        }
        return newState;
    }

	public AIPops(Hedgehog[,] hg, int dimensions){
		this.hg = hg;
		this.dimensions = dimensions;
	}

	public bool checkConnect(int x, int y){
		for (int k = -1; k <= 1; k += 2){
            if ((x + k) >= 0 && (x + k) <= dimensions - 1){
                if (hg[x,y].getColor() == hg[x + k, y].getColor()){
                    return true;
                }
            }if ((y + k) >= 0 && (y + k) <= dimensions - 1){
                if (hg[x, y].getColor() == hg[x, y + k].getColor()){
                    return true;
                }
            }
        }
        return false;
    }

    public void Advent(int x, int y){
        if(checkConnect(x, y)){//don't do anything, if there are no connections

            List<Coords> hits = new List<Coords>();
            int derp = 0;
        	for (int k = -1; k <= 1; k += 2){//checks each direction for matching colors, and stores each match in a list of coordinates
                if ((x + k) >= 0 && (x + k) <= dimensions - 1){
                    if (hg[x, y].getColor() == hg[x + k, y].getColor()){
                        hits.Add(new Coords(x + k, y));
                    }
                }
                if ((y + k) >= 0 && (y + k) <= dimensions - 1){
                    if (hg[x, y].getColor() == hg[x, y + k].getColor()){
                        hits.Add(new Coords(x, y + k));
                    }
                }
            }
            if (hg[x, y].getType() == SPLITTER) {
                derp = hg[x, y - 1].getHealth();
            }else if (hg[x, y].getType() == NORMAL || hg[x, y].getType() == ACE || hg[x, y].getType() == ARMOR) {
                int max = 0;
                bool gotone = false;
                foreach (Coords hit in hits) {
                    if (hg[hit.x, hit.y].getType() == SPLITTER && hg[hit.x, hit.y].getColor() == hg[x, y].getColor()) {
                        gotone = true;
                        if(max < hg[hit.x, hit.y].getHealth()){
                            max = hg[hit.x, hit.y].getHealth();
                        }
                    }
                }
                if(gotone){
                    splittify(x, y, max);
                    derp = -1;
                }
            }

            if (hg[x, y].getType() == NORMAL || hg[x, y].getType() == ACE) {
                foreach (Coords hit in hits) {
                    if (hg[hit.x, hit.y].getType() == NORMAL) {//If a normal/ace hits a normal
                        loseHealth(hit.x, hit.y, -(hits.Count));
                    }else if (hg[hit.x, hit.y].getType() == ARMOR) {//If a normal/ace hits an armor
                        loseHealth(hit.x, hit.y, -(hits.Count));
                    }else if (hg[hit.x, hit.y].getType() == SPLITTER) {//If a normal/ace hits a splitter
                        split(hit.x, hit.y, hg[hit.x, hit.y].getColor(), hg[hit.x, hit.y].getHealth() - 1, hg[hit.x, hit.y].getSplitId());
                    }else if (hg[hit.x, hit.y].getType() == FIREBALL) {//If a normal/ace hits a fireball
                        fireVector(hit.x, hit.y);
                    }else if (hg[hit.x, hit.y].getType() == BOMB) {//If a normal/ace hits a bomb
                        bomb(hit);
                    }

                }
                if (hg[x, y].getType() != SPLITTER && hg[x, y].getType() != -1) {
                    loseHealth(x, y, -(hits.Count));
                }
            }else if (hg[x, y].getType() == ARMOR) {
                foreach (Coords hit in hits) {
                    if (hg[hit.x, hit.y].getType() == NORMAL) {//If an armor hits a normal
                        // hg.pop(hit.x, hit.y);
                        hg[hit.x, hit.y].pop();
                        loseHealth(x, y, -(hits.Count));
                    }else if (hg[hit.x, hit.y].getType() == ARMOR) {//If an armor hits an armor
                        loseHealth(hit.x, hit.y, -(hits.Count));
                        loseHealth(x, y, -(hits.Count));
                    }else if (hg[hit.x, hit.y].getType() == SPLITTER) {//If an armor hits a splitter
                        split(hit.x, hit.y, hg[hit.x, hit.y].getColor(), hg[hit.x, hit.y].getHealth() - 1, hg[hit.x, hit.y].getSplitId());
                    }else if (hg[hit.x, hit.y].getType() == FIREBALL) {//If an armor hits a fireball
                        //TODO
                    }else if (hg[hit.x, hit.y].getType() == BOMB) {//If an armor hits a bomb
                        //TODO
                    }

                }
            }else if (hg[x, y].getType() == SPLITTER) {
                bool doubledip = false;
                foreach (Coords hit in hits) {
                    if (hg[hit.x, hit.y].getType() == NORMAL) {//If a splitter hits a normal
                        splittify(hit.x, hit.y, hg[x, y].getHealth());
                        split(hit.x, hit.y, hg[hit.x, hit.y].getColor(), hg[hit.x, hit.y].getHealth() - 1, hg[x, y].getSplitId());
                    }else if (hg[hit.x, hit.y].getType() == ARMOR) {//If a splitter hits an armor
                        splittify(hit.x, hit.y, hg[x, y].getHealth());
                        split(hit.x, hit.y, hg[hit.x, hit.y].getColor(), hg[hit.x, hit.y].getHealth() - 1, hg[x, y].getSplitId());
                    }else if (hg[hit.x, hit.y].getType() == SPLITTER) {//If a splitter hits a splitter
                        if (!doubledip) {
                            if(hg[x, y].getSplitId() == -1){
                                hg[x, y].setSplitId(id_counter);
                                id_counter++;
                            }
                            split(hit.x, hit.y, hg[hit.x, hit.y].getColor(), hg[hit.x, hit.y].getHealth() + derp, hg[hit.x, hit.y].getSplitId());
                            doubledip = true;
                        }
                    }else if (hg[hit.x, hit.y].getType() == FIREBALL) {//If a splitter hits a fireball
                        //TODO
                    }else if (hg[hit.x, hit.y].getType() == BOMB) {//If a splitter hits a bomb
                        //TODO
                    }
                }
            }else if (hg[x, y].getType() == FIREBALL) {
                foreach (Coords hit in hits) {
                    if (hg[hit.x, hit.y].getType() == NORMAL) {//If a fireball hits a normal

                    }else if (hg[hit.x, hit.y].getType() == ARMOR) {//If a fireball hits an armor

                    }else if (hg[hit.x, hit.y].getType() == SPLITTER) {//If a fireball hits a splitter

                    }else if (hg[hit.x, hit.y].getType() == FIREBALL) {//If a fireball hits a fireball

                    }else if (hg[hit.x, hit.y].getType() == BOMB) {//If a fireball hits a bomb

                    }

                }
            }else if (hg[x, y].getType() == BOMB) {
                foreach (Coords hit in hits) {
                    if (hg[hit.x, hit.y].getType() == NORMAL) {//If a bomb hits a normal

                    }else if (hg[hit.x, hit.y].getType() == ARMOR) {//If a bomb hits an armor

                    }else if (hg[hit.x, hit.y].getType() == SPLITTER) {//If a bomb hits a splitter

                    }else if (hg[hit.x, hit.y].getType() == FIREBALL) {//If a bomb hits a fireball

                    }else if (hg[hit.x, hit.y].getType() == BOMB) {//If a bomb hits a bomb

                    }
                }
            }
        }
    }

    void loseHealth(int x, int y, int damage) {
        // EntropyTree.instance.RemoveHedgehog(x, y);
        hg[x, y].loseHealth(damage);
    }

    void split(int x, int y, int color, int newHealth, int split_id){
        if (hg[x, y].getColor() == color) {
            hg[x, y].setSplitId(split_id);
            if (hg[x, y].getType() == SPLITTER){
                if (hg[x, y].getHealth() != newHealth) {
                    hg[x, y].setHealth(newHealth);

                    for (int k = -1; k <= 1; k += 2) {
                        if ((x + k) >= 0 && (x + k) <= (dimensions - 1)) {
                            split(x + k, y, color, newHealth, split_id);
                        }
                        if ((y + k) >= 0 && (y + k) <= (dimensions - 1)) {
                            split(x, y + k, color, newHealth, split_id);
                        }
                    }
                }
            }else if (hg[x, y].getType() != -1) {
                newHealth--;
                if (newHealth <= 0) {
                    splittify(x, y, 1);
                }else {
                    splittify(x, y, newHealth);
                }
                split(x, y, color, newHealth, split_id);
            }
        }
    }

    private void splittify(int x, int y, int health) {
        hg[x, y].setType(SPLITTER);
        hg[x, y].setHealth(health);
    }

    private void fireVector(int x, int y) {

    }

    private void bomb(Coords pos) {
        int color = hg[pos.x, pos.y].getColor();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if((pos.x + x) > 0 && (pos.x + x) < dimensions -1 && (pos.y + y) > 0 && (pos.y + y) < dimensions -1){
                    if (color == hg[pos.x + x, pos.y + y].getColor()) {
                        hg[pos.x + x, pos.y + y].loseHealth(-1);//you might need to change this
                    }
                }
            }
        }
    }


}

