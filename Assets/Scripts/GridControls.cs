using UnityEngine;
using System.Collections;

[RequireComponent (typeof (HedgieSprites))]
public class GridControls : MonoBehaviour {

	public struct Coords{//lets me refer to various things as x,y coordinates
		public int x, y;

		public Coords(int x, int y){
			this.x = x;
			this.y = y;
		}
	}

	public Camera cam;//the main camera
    public GameObject HedgieObject;
	public int dimensions, innerBalls;
	public float rotationTime, movSpeed;//time for rotation and speed of hedgie travel
	private bool clockwise, counterclockwise, moving;//are true when an action is occuring
	private float clockStart;//holds when the timer starts
	private Coords movStart, movEnd;//coordinates within the grid dimensions where you start and end
	private Vector2 movStartPos, movEndPos;//position within the gamespace where you start and end

	private HedgieGrid hg;//check the HedgieGrid class; this holds all the sweet grid juice
    private HedgieSprites hsprites;
	void Start(){
		hg = new HedgieGrid(dimensions, innerBalls, cam);
        hsprites = GetComponent<HedgieSprites>() as HedgieSprites;
        InstantiateHedgies();
        SpawnOuterBalls();
        SpawnInnerBalls(innerBalls);
	}

    private void InstantiateHedgies(){
        //change this shit
        Vector2 g;
        for(int x = 0; x < dimensions; x++){
            for(int y = 0; y < dimensions; y++){
                g = hg.getGrid(x, y);
                GameObject go = (GameObject)Instantiate(HedgieObject, new Vector3(g.x, g.y, 0), Quaternion.identity);
                Hedgie defaultHedgie = new Hedgie(go, hsprites.getSprite(0,0), -1, -1);
                hg.setHedgie(x, y, defaultHedgie);
            }
        }
    }
    //switch this to after hedgies explode
    /*void Update(){
        if (hg.getBallCount() < 1){
            Restart();
        }
    }*/

    //change this if you want to lean towards certain hedgies
	private void SpawnBall(int x, int y)
    {
        int type = Random.Range(0, hsprites.getLength() - 1);
        int color = Random.Range(0, hsprites.getSheetLength(type));
        Vector2 g = hg.getGrid(x, y);
        Hedgie spawnHedgie = new Hedgie(hg.getHedgie(x,y).getObject(), hsprites.getSprite(type, color), color, type);
        hg.getHedgie(x, y).setHedgie(spawnHedgie);
        print("Type: " + type + " Color: " + color);
    }

    private void SpawnOuterBalls()
    {
        for (int d = dimensions-2; d > 0; d--)
        {
            //left side
            SpawnBall(0,d);
            //right side
            SpawnBall(dimensions-1, d);

            //bottom side
            SpawnBall(d, 0);
            //top side
            SpawnBall(d, dimensions - 1);
        }
    }

    private void SpawnInnerBalls(int balls)
    {
        int counter, x, y;
        counter = 0;
        while (counter < balls)
        {
            x = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
            y = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
            if (hg.getHedgie(x, y).getColor() == -1)
            {
                counter++;
                SpawnBall(x, y);

                if (checkConnect(x, y))
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        if ((x+k) >= 1 && (x+k) <= dimensions - 2)
                        {
                            if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x + k, y).getColor())
                            {
                                hg.pop(x + k, y);
                                counter--;
                            }
                        }
                        if ((y + k) >= 1 && (y + k) <= dimensions - 2)
                        {
                            if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x, y + k).getColor())
                            {
                                hg.pop(x, y + k);
                                counter--;
                            }
                        }

                    }
                    
                    hg.pop(x, y);
                    counter--;
                }
            }
        }
    }


    public bool InMotion(){
        return (moving || counterclockwise || clockwise);
    }

    public void Restart(){
        SpawnInnerBalls(innerBalls);
        hg.setBallCount(innerBalls);
    }
	void FixedUpdate(){
        if(counterclockwise){

            float timeSinceStarted = Time.time - clockStart;
            float complete = timeSinceStarted / rotationTime;

            Vector2 leftPos, rightPos, topPos, bottomPos;

            for (int k = 0; k < dimensions - 2; k++){
                bottomPos = hg.getGrid(k + 1, 0);
                rightPos = hg.getGrid(dimensions - 1, k + 1);
                topPos = hg.getGrid(dimensions - k - 2,dimensions - 1);
                leftPos = hg.getGrid(0, dimensions - 2 - k);

                //lerp bottom to right
                hg.getHedgie(k + 1, 0).getObject().transform.position = Vector2.Lerp(hg.getGrid(k + 1, 0), rightPos, complete);
                //lerp right to top
                hg.getHedgie(dimensions - 1, k + 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - 1, k + 1),topPos, complete);
                //lerp top to left
                hg.getHedgie(dimensions - k - 2,dimensions - 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - k - 2,dimensions - 1), leftPos, complete);
                //lerp left to bottom
                hg.getHedgie(0, dimensions - 2 - k).getObject().transform.position = Vector2.Lerp(hg.getGrid(0, dimensions - 2 - k), bottomPos, complete);
            }

            if(complete >= 1.0f){
                counterclockwise = false;
                for (int k = 0; k < dimensions - 2; k++){
                    Hedgie bottom = new Hedgie(hg.getHedgie(k + 1, 0));
                    Hedgie right = new Hedgie(hg.getHedgie(dimensions - 1, k + 1));
                    Hedgie top = new Hedgie(hg.getHedgie(dimensions - k - 2, dimensions - 1));
                    Hedgie left = new Hedgie(hg.getHedgie(0, dimensions - k - 2));

                    hg.getHedgie(k + 1, 0).setHedgie(left);//bottom is given left
                    hg.getHedgie(dimensions - 1, k + 1).setHedgie(bottom);//right is given bottom
                    hg.getHedgie(dimensions - k - 2, dimensions - 1).setHedgie(right);//top is given right
                    hg.getHedgie(0, dimensions - 2 - k).setHedgie(top);//left is given top
                }
            }
        }
        if(clockwise){
            float timeSinceStarted = Time.time - clockStart;
            float complete = timeSinceStarted / rotationTime;

            Vector2 leftPos, rightPos, topPos, bottomPos;

            for (int k = 0; k < dimensions - 2; k++){
                bottomPos = hg.getGrid(k + 1, 0);
                rightPos = hg.getGrid(dimensions - 1, k + 1);
                topPos = hg.getGrid(dimensions - k - 2,dimensions - 1);
                leftPos = hg.getGrid(0, dimensions - 2 - k);

                //lerp bottom to left
                hg.getHedgie(k + 1, 0).getObject().transform.position = Vector2.Lerp(hg.getGrid(k + 1, 0), leftPos, complete);
                //lerp right to bottom
                hg.getHedgie(dimensions - 1, k + 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - 1, k + 1),bottomPos, complete);
                //lerp top to right
                hg.getHedgie(dimensions - k - 2,dimensions - 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - k - 2,dimensions - 1), rightPos, complete);
                //lerp left to top
                hg.getHedgie(0, dimensions - 2 - k).getObject().transform.position = Vector2.Lerp(hg.getGrid(0, dimensions - 2 - k), topPos, complete);
            }

            if(complete >= 1.0f){
                clockwise = false;
                for (int k = 0; k < dimensions - 2; k++){
                    Hedgie bottom = new Hedgie(hg.getHedgie(k + 1, 0));
                    Hedgie right = new Hedgie(hg.getHedgie(dimensions - 1, k + 1));
                    Hedgie top = new Hedgie(hg.getHedgie(dimensions - k - 2, dimensions - 1));
                    Hedgie left = new Hedgie(hg.getHedgie(0, dimensions - k - 2));

                    hg.getHedgie(k + 1, 0).setHedgie(right);//bottom is given right
                    hg.getHedgie(dimensions - 1, k + 1).setHedgie(top);//right is given top
                    hg.getHedgie(dimensions - k - 2, dimensions - 1).setHedgie(left);//top is given left
                    hg.getHedgie(0, dimensions - 2 - k).setHedgie(bottom);//left is given bottom
                }
            }
        }
        if(moving){
            float distCovered = (Time.time - clockStart) * movSpeed;
            float fracJourney = distCovered / Vector2.Distance(movStartPos, movEndPos);

            hg.getHedgie(movStart.x, movStart.y).getObject().transform.position = Vector2.Lerp(movStartPos, movEndPos, fracJourney);

            if(fracJourney >= 1.0f){
                hg.ballIncrement();
                hg.setHedgie(movEnd.x, movEnd.y, new Hedgie(hg.getHedgie(movStart.x, movStart.y)));
                hg.setHedgie(movStart.x, movStart.y, new Hedgie());
                SpawnBall(movStart.x, movStart.y);

                if (checkConnect(movEnd.x,movEnd.y))
                    explode(movEnd.x, movEnd.y);

                moving = false;
            }
        }
    }

    public void checkTouch(Vector3 pos){
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Coords click = new Coords((int)(wp.x / hg.getTile().x), (int)(wp.y / hg.getTile().y));
        //print(click);
        if (click.x == 0 && click.y != 0 && click.y != dimensions - 1)//if the left corner is clicked
        {
        	int check = checkRight(click.y);
            if (check != -1)
                move(new Coords(click.x, click.y), new Coords(check, click.y), 1f);
        }

        if (click.x == dimensions - 1 && click.y != 0 && click.y != dimensions - 1)//if the right corner is clicked
        {
        	int check = checkLeft(click.y);
            if (check != -1)
                move(new Coords(click.x, click.y), new Coords(check, click.y),1f);
        }

        if (click.y == 0 && click.x != 0 && click.x != dimensions - 1)//if the bottom corner is clicked
        {
        	int check = checkUp(click.x);
            if (check != -1)
                move(new Coords(click.x, click.y), new Coords(click.x, check),1f);
        }

        if (click.y == dimensions - 1 && click.x != 0 && click.x != dimensions - 1)//if the top corner is clicked
        {
        	int check = checkDown(click.x);
            if (check != -1)
                move(new Coords(click.x, click.y), new Coords(click.x, check),1f);
        }
    }

    public void rotateCounterclockwise()
    {
            clockStart = Time.time;
            counterclockwise = true;
    }

    public void rotateClockwise()
    {
            clockStart = Time.time;
            clockwise = true;
    }

    private void move(Coords start, Coords end, float time)
    {
        movStart = start;
        movEnd = end;
        movStartPos = hg.getGrid(start.x,start.y);
        movEndPos = hg.getGrid(end.x, end.y);
        moving = true;
        clockStart = Time.time;
    } 

    private int checkRight(int y)
    {
        if (hg.getHedgie(1, y).getColor() != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = 2; k < dimensions - 1; k++)
            {
                if (hg.getHedgie(k, y).getColor() != -1) 
                    return k - 1;
            }
            return -1;
        }
    }

    private int checkLeft(int y)
    {
        if (hg.getHedgie(dimensions - 2, y).getColor() != -1)//if there's a ball in front of the ball you're shooting
            return -1;
        else
        {
            for (int k = dimensions - 3; k > 0; k--)//checks sequentially 
            {
                if (hg.getHedgie(k, y).getColor() != -1)
                    return k + 1;
            }
            return -1;//no balls on that row
        }
    }

    private int checkUp(int x)
    {
        if (hg.getHedgie(x, 1).getColor() != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = 2; k < dimensions - 1; k++)//checks sequentially above where was clicked
            {
                if (hg.getHedgie(x, k).getColor() != -1)
                    return k - 1;//returns the y value before the first ball encountered 
            }
            return -1;
        }
    }

    private int checkDown(int x)
    {
        if (hg.getHedgie(x, dimensions - 2).getColor() != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = dimensions - 3; k > 0; k--)//checks sequentially below where was clicked 
            {
                if (hg.getHedgie(x, k).getColor() != -1)
                    return k + 1;//returns the y value before the first ball encountered
            }
            return -1;
        }
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@~~~~Probably change this for splitters~~~~@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    private void explode(int x, int y)
    {
        for (int k = -1; k <= 1; k += 2)
        {
            if ((x + k) >= 1 && (x + k) <= dimensions - 2)
            {
                if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x + k, y).getColor())
                {
                    hg.pop(x + k, y);
                    hg.ballDecrement();
                }
            }
            if ((y + k) >= 1 && (y + k) <= dimensions - 2)
            {
                if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x, y + k).getColor())
                {
                    hg.pop(x, y + k);
                    hg.ballDecrement();
                }
            }
        }
        hg.pop(x, y);
        hg.ballDecrement();
    }

    

    private bool checkConnect(int x, int y)
    {
        for (int k = -1; k <= 1; k += 2)
        {
            if ((x + k) >= 1 && (x + k) <= dimensions - 2)
            {
                if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x + k, y).getColor())
                {
                    return true;
                }
            }
            if ((y + k) >= 1 && (y + k) <= dimensions - 2)
            {
                if (hg.getHedgie(x, y).getColor() == hg.getHedgie(x, y + k).getColor())
                {
                    return true;
                }
            }
            
        }

        return false;
    }
}
