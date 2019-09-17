using UnityEngine;
using System.Collections;

[RequireComponent (typeof (HedgieSprites))]
[RequireComponent (typeof (SpawnWorkflow))]
[RequireComponent (typeof (MusicMiddleware))]
public class GridControls : MonoBehaviour {

    public Camera cam;//the main camera
    public GameObject HedgieObject;
    public int dimensions, innerBalls;
    public float rotationTime, movSpeed;//time for rotation and speed of hedgie travel
    private bool clockwise, counterclockwise, moving, spinning, doubleClockwise, onceAround;//are true when an action is occuring
    private float clockStart, spinClockStart;//holds when the timer starts
    private Coords movStart, movEnd;//coordinates within the grid dimensions where you start and end
    private Vector2 movStartPos, movEndPos;//position within the gamespace where you start and end
    private Quaternion qStart, qEnd;
    private HedgieGrid hg;//check the HedgieGrid class; this holds all the sweet grid juice
    private Taps taps;
    private Pops pops;
    private HedgieSprites hsprites;
    private SpawnWorkflow sw;
    private bool hasStarted = false;
    private MusicMiddleware mm;

    public int numberOfMoves = 0;

    public bool isAIOn = false;
    public bool canMakeAIMove = false;

    void Start() {
        sw = GetComponent<SpawnWorkflow>() as SpawnWorkflow;
        hsprites = GetComponent<HedgieSprites>() as HedgieSprites;
        mm = GetComponent<MusicMiddleware>() as MusicMiddleware;

        MakeGrid();
    }

    public void MakeGrid(){
        if(hasStarted)
            hg.DestroyAll();
        
        hg = new HedgieGrid(dimensions, HedgieObject, cam, hsprites);
        taps = new Taps(hg);
        pops = new Pops(hg);
        //InstantiateHedgies();
        SpawnOuterBalls();
        SpawnInnerBalls(innerBalls);
        hasStarted = true;
        mm.loopSound("Very_Hedgie", true);
    }

    public void setParams(int dimensions, int innerHedgies, int normalTend, int armorTend, int splitterTend, int armorMin, int splitterMin, int armorMax, int splitterMax) {
        this.dimensions = dimensions;
        this.innerBalls = innerHedgies;
        sw.setTendencies(normalTend, armorTend, splitterTend);
        sw.setRange(armorMin, armorMax, splitterMin, splitterMax);
    }

    private void SpawnBall(int x, int y)
    {
        int type = sw.pickHedgieType();
        int color = Random.Range(0, hsprites.getSheetLength(type));
        Hedgie spawnHedgie = new Hedgie(hg.getObject(x, y), hsprites.getSprite(type, color), color, type, sw.pickHedgieHealth(type));
        hg.transmogrify(x, y, spawnHedgie);
        hg.ballIncrement();

        EntropyTree.instance.AddHedgehog(x, y, color);
    }

    private void SpawnOuterBall(int x, int y){
        int type = sw.pickHedgieType();
        int color = Random.Range(0, hsprites.getSheetLength(type));
        Hedgie spawnHedgie = new Hedgie(hg.getObject(x, y), hsprites.getSprite(type, color), color, type, sw.pickHedgieHealth(type));
        hg.transmogrify(x, y, spawnHedgie);
        EntropyTree.instance.SetOuterHedgehog(x, y, color);
    }

    private void SpawnOuterBalls()
    {
        for (int d = dimensions-2; d > 0; d--)
        {
            //left side
            SpawnOuterBall(0,d);
            //right side
            SpawnOuterBall(dimensions-1, d);
            //bottom side
            SpawnOuterBall(d, 0);
            //top side
            SpawnOuterBall(d, dimensions - 1);
        }
    }

    private void SpawnInnerBalls(int balls)
    {
        int x, y;
        while (hg.getBallCount() < balls)
        {
            x = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
            y = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
            if (hg.getHedgie(x, y).getColor() == -1)
            {
                SpawnBall(x,y);
                pops.Advent(x,y);
            }
        }
    }


    public bool InMotion(){
        return (moving || counterclockwise || clockwise || !hasStarted);
    }

    void Update(){
        if(!InMotion() && isAIOn && canMakeAIMove){
            MakeAIMove();
            canMakeAIMove = false;
        }
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

                    hg.setHedgie(k + 1, 0, left);//bottom is given left
                    hg.setHedgie(dimensions - 1, k + 1, bottom);//right is given bottom
                    hg.setHedgie(dimensions - k - 2, dimensions - 1, right);//top is given right
                    hg.setHedgie(0, dimensions - 2 - k, top);//left is given top

                    EntropyTree.instance.SetOuterHedgehog(k + 1, 0, left.getColor());//bottom is given left
                    EntropyTree.instance.SetOuterHedgehog(dimensions - 1, k + 1, bottom.getColor());//right is given bottom
                    EntropyTree.instance.SetOuterHedgehog(dimensions - k - 2, dimensions - 1, right.getColor());//top is given right
                    EntropyTree.instance.SetOuterHedgehog(0, dimensions - 2 - k, top.getColor());//left is given top
                }
            }
        }
        if(clockwise || doubleClockwise){
            float timeSinceStarted = Time.time - clockStart;

            float complete = 0f;

            if(doubleClockwise || onceAround){
                complete = 2f*(timeSinceStarted / rotationTime);
            }else{
                complete = timeSinceStarted / rotationTime;
            }

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
                if(doubleClockwise){
                    if(onceAround){
                        doubleClockwise = false;
                    }else{
                        onceAround = true;
                        complete = 0;
                        clockStart = Time.time;
                    }
                }
                clockwise = false;
                for (int k = 0; k < dimensions - 2; k++){
                    Hedgie bottom = new Hedgie(hg.getHedgie(k + 1, 0));
                    Hedgie right = new Hedgie(hg.getHedgie(dimensions - 1, k + 1));
                    Hedgie top = new Hedgie(hg.getHedgie(dimensions - k - 2, dimensions - 1));
                    Hedgie left = new Hedgie(hg.getHedgie(0, dimensions - k - 2));

                    hg.setHedgie(k + 1, 0, right);//bottom is given right
                    hg.setHedgie(dimensions - 1, k + 1, top);//right is given top
                    hg.setHedgie(dimensions - k - 2, dimensions - 1, left);//top is given left
                    hg.setHedgie(0, dimensions - 2 - k, bottom);//left is given bottom

                    EntropyTree.instance.SetOuterHedgehog(k + 1, 0, right.getColor());//bottom is given right
                    EntropyTree.instance.SetOuterHedgehog(dimensions - 1, k + 1, top.getColor());//right is given top
                    EntropyTree.instance.SetOuterHedgehog(dimensions - k - 2, dimensions - 1, left.getColor());//top is given left
                    EntropyTree.instance.SetOuterHedgehog(0, dimensions - 2 - k, bottom.getColor());//left is given bottom
                }
            }
        }
        if(moving){
            float distCovered = (Time.time - clockStart) * movSpeed;
            float fracJourney = distCovered / Vector2.Distance(movStartPos, movEndPos);

            hg.getHedgie(movStart.x, movStart.y).getObject().transform.position = Vector2.Lerp(movStartPos, movEndPos, fracJourney);

            if(fracJourney >= 1.0f){
                hg.ballIncrement();
                hg.transmogrify(movEnd.x, movEnd.y, hg.getHedgie(movStart.x, movStart.y));
                hg.getHedgie(movStart.x, movStart.y).getObject().transform.position = hg.getGrid(movStart.x, movStart.y);
                SpawnOuterBall(movStart.x, movStart.y);
                
                EntropyTree.instance.AddHedgehog(movEnd.x, movEnd.y, hg.getHedgie(movEnd.x, movEnd.y).getColor());

                pops.Advent(movEnd.x, movEnd.y);
                checkRestart(innerBalls);

                moving = false;
            }
        }
        if(spinning){
            float timeSinceRotation = Time.time - spinClockStart;
            float complete = timeSinceRotation / rotationTime;
            for(int x = 0; x < dimensions; x++){
                for(int y = 0; y < dimensions; y++){
                    hg.getHedgie(x,y).getObject().transform.rotation = Quaternion.Slerp(qStart, qEnd, complete);
                }
            }
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

    public void rotateDoubleClockwise(){
        clockStart = Time.time;
        doubleClockwise = true;
        onceAround = false;
    }

    public void spinHedgie(Quaternion qStart, Quaternion qEnd){
        this.qStart = qStart;
        this.qEnd = qEnd;
        spinClockStart = Time.time;
        spinning = true;
    }

    public void checkTouch(Vector3 pos){
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Coords click = new Coords((int)(wp.x / hg.getTile().x), (int)(wp.y / hg.getTile().y));
        Vector2 end = taps.FindResultingVector(click.x, click.y);
        if(end != Vector2.zero){
            Move(new Coords(click.x, click.y), new Coords((int)end.x, (int)end.y));
        }
    }

    private void Move(Coords start, Coords end)
    {
        movStart = start;
        movEnd = end;
        movStartPos = hg.getGrid(start.x,start.y);
        movEndPos = hg.getGrid(end.x, end.y);
        moving = true;
        clockStart = Time.time;
    } 

    public void checkRestart(int newBalls){
        if(hg.getBallCount() <= 0)
            SpawnInnerBalls(newBalls);    
    }

    public HedgieSprites getHedgieSprites(){
        return hsprites;
    }

    public void MakeAIMove(){
        // Debug.Log(move.ToString());
        numberOfMoves++;
        StartCoroutine(AIMove(EntropyTree.instance.FindNextMove()));
    }

    public void TurnOnAIMode(){
        canMakeAIMove = true;
        isAIOn = true;
    }

    IEnumerator AIMove(Move move){

        Coords click = new Coords(0,0);
        Vector2 end = new Vector2();

        switch(move.rotation){
            case 0:
                break;
            case 1:
                rotateClockwise();
                break;
            case -1:
                rotateCounterclockwise();
                break;
            case 2:
                rotateDoubleClockwise();
                break;
            default:
               Debug.Log("weird");
                break;
        }

        yield return new WaitForSeconds(rotationTime + 0.1f);

        switch(move.side){
            case 0://top
                click = new Coords(move.index + 1, dimensions - 1);
                end = taps.FindResultingVector(click.x, click.y);
                if(end != Vector2.zero){
                    Move(new Coords(click.x, click.y), new Coords((int)end.x, (int)end.y));
                }
                break;
            case 1://right
                click = new Coords(dimensions - 1, dimensions - 2 - move.index);
                end = taps.FindResultingVector(click.x, click.y);
                if(end != Vector2.zero){
                    Move(new Coords(click.x, click.y), new Coords((int)end.x, (int)end.y));
                }
                break;
            case 2://bottom
                click = new Coords(dimensions - 2 - move.index, 0);
                end = taps.FindResultingVector(click.x, click.y);
                if(end != Vector2.zero){
                    Move(new Coords(click.x, click.y), new Coords((int)end.x, (int)end.y));
                }
                break;
            default://left
                click = new Coords(0, move.index + 1);
                end = taps.FindResultingVector(click.x, click.y);
                if(end != Vector2.zero){
                    Move(new Coords(click.x, click.y), new Coords((int)end.x, (int)end.y));
                }
                break;
        }
        canMakeAIMove = true;
    }
}
