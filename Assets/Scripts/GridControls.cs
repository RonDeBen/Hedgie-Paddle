using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[RequireComponent (typeof (HedgieSprites))]
[RequireComponent (typeof (SpawnWorkflow))]
[RequireComponent (typeof (MusicMiddleware))]
public class GridControls : MonoBehaviour {

    public Camera cam;//the main camera
    public GameObject HedgieObject;
    public int dimensions, innerBalls;
    public float rotationTime, movSpeed, aiSlow;//time for rotation and speed of hedgie travel
    private bool clockwise, counterclockwise, moving, spinning, doubleClockwise, onceAround;//are true when an action is occuring
    private float clockStart, spinClockStart;//holds when the timer starts
    private Coords movStart, movEnd;//coordinates within the grid dimensions where you start and end
    private Vector2 movStartPos, movEndPos;//position within the gamespace where you start and end
    private Quaternion qStart, qEnd;
    public HedgieGrid hg;//check the HedgieGrid class; this holds all the sweet grid juice
    private Taps taps;
    private Pops pops;
    private HedgieSprites hsprites;
    private SpawnWorkflow sw;
    private bool hasStarted = false;
    private MusicMiddleware mm;

    public int numberOfMoves, numberOfGames, gamesToPlay = 0;

    public bool isAIOn = false;
    public bool canMakeAIMove = false;

    private List<Move> aiMoves = new List<Move>();

    private float start_entropy;

    public DifficultyController DiffCon;

    void Start() {
        sw = GetComponent<SpawnWorkflow>() as SpawnWorkflow;
        sw.SetTendencies();
        hsprites = GetComponent<HedgieSprites>() as HedgieSprites;
        mm = GetComponent<MusicMiddleware>() as MusicMiddleware;

        MakeGrid();
    }

    public void MakeGrid(){
        hg.SetUp(dimensions, HedgieObject, cam, hsprites);
        taps = new Taps(hg);
        pops = new Pops(hg);
        SpawnOuterBalls();
        SpawnInnerBalls(innerBalls);
        hasStarted = true;
        mm.loopSound("Very_Hedgie", true);
    }

    private void SpawnBall(int x, int y){
        int type = sw.PickHedgieTypeInner();
        int color = Random.Range(0, hsprites.getSheetLength(type));
        Hedgie spawnHedgie = new Hedgie(hg.getObject(x, y), hsprites.getSprite(type, color), color, type, sw.pickHedgieHealth(type));
        hg.transmogrify(x, y, spawnHedgie);
        hg.ballIncrement();

        EntropyTree.instance.AddHedgehog(x, y, spawnHedgie.color, spawnHedgie.health, spawnHedgie.type);
    }

    private void SpawnOuterBall(int x, int y){
        int type = sw.pickHedgieTypeOuter();
        int color = Random.Range(0, hsprites.getSheetLength(type));
        Hedgie spawnHedgie = new Hedgie(hg.getObject(x, y), hsprites.getSprite(type, color), color, type, sw.pickHedgieHealth(type));
        hg.transmogrify(x, y, spawnHedgie);
        EntropyTree.instance.SetOuterHedgehog(x, y, color);
    }

    private void SpawnOuterBalls(){
        for (int d = dimensions-2; d > 0; d--){
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

    private void SpawnInnerBalls(int balls){
        int x, y;
        do{
            hg.ClearInnerHedgehogs();
            while (hg.getBallCount() < balls){
                // x = Random.Range(1, dimensions - 1);
                // y = Random.Range(1, dimensions - 1);
                if(dimensions % 2 == 0){
                    x = Random.Range(0, dimensions / 2) + Random.Range(1, dimensions / 2);
                    y = Random.Range(0, dimensions / 2) + Random.Range(1, dimensions / 2);
                }else{
                    x = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
                    y = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
                }
                if (hg.getHedgie(x, y).getColor() == -1){
                    SpawnBall(x,y);
                    pops.Advent(x,y);
                }
            }
        }while (EntropyTree.instance.GetNumPossibleMoves() == 0);
        start_entropy = EntropyTree.instance.GetCurrentEntropy();
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

        // if(counterclockwise){

        //     float timeSinceStarted = Time.time - clockStart;
        //     float complete = timeSinceStarted / rotationTime;

        //     Vector2 leftPos, rightPos, topPos, bottomPos;

        //     for (int k = 0; k < dimensions - 2; k++){
        //         bottomPos = hg.getGrid(k + 1, 0);
        //         rightPos = hg.getGrid(dimensions - 1, k + 1);
        //         topPos = hg.getGrid(dimensions - k - 2,dimensions - 1);
        //         leftPos = hg.getGrid(0, dimensions - 2 - k);

        //         //lerp bottom to right
        //         hg.getHedgie(k + 1, 0).getObject().transform.position = Vector2.Lerp(hg.getGrid(k + 1, 0), rightPos, complete);
        //         //lerp right to top
        //         hg.getHedgie(dimensions - 1, k + 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - 1, k + 1),topPos, complete);
        //         //lerp top to left
        //         hg.getHedgie(dimensions - k - 2,dimensions - 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - k - 2,dimensions - 1), leftPos, complete);
        //         //lerp left to bottom
        //         hg.getHedgie(0, dimensions - 2 - k).getObject().transform.position = Vector2.Lerp(hg.getGrid(0, dimensions - 2 - k), bottomPos, complete);
        //     }

        //     if(complete >= 1.0f){
        //         counterclockwise = false;
        //         for (int k = 0; k < dimensions - 2; k++){
        //             Hedgie bottom = new Hedgie(hg.getHedgie(k + 1, 0));
        //             Hedgie right = new Hedgie(hg.getHedgie(dimensions - 1, k + 1));
        //             Hedgie top = new Hedgie(hg.getHedgie(dimensions - k - 2, dimensions - 1));
        //             Hedgie left = new Hedgie(hg.getHedgie(0, dimensions - k - 2));

        //             hg.setHedgie(k + 1, 0, left);//bottom is given left
        //             hg.setHedgie(dimensions - 1, k + 1, bottom);//right is given bottom
        //             hg.setHedgie(dimensions - k - 2, dimensions - 1, right);//top is given right
        //             hg.setHedgie(0, dimensions - 2 - k, top);//left is given top

                    // EntropyTree.instance.SetOuterHedgehog(k + 1, 0, left.getColor());//bottom is given left
                    // EntropyTree.instance.SetOuterHedgehog(dimensions - 1, k + 1, bottom.getColor());//right is given bottom
                    // EntropyTree.instance.SetOuterHedgehog(dimensions - k - 2, dimensions - 1, right.getColor());//top is given right
                    // EntropyTree.instance.SetOuterHedgehog(0, dimensions - 2 - k, top.getColor());//left is given top
        //         }
        //     }
        // }
        // if(clockwise || doubleClockwise){
        //     float timeSinceStarted = Time.time - clockStart;

        //     float complete = 0f;

        //     if(doubleClockwise || onceAround){
        //         complete = 2f*(timeSinceStarted / rotationTime);
        //     }else{
        //         complete = timeSinceStarted / rotationTime;
        //     }

        //     Vector2 leftPos, rightPos, topPos, bottomPos;

        //     for (int k = 0; k < dimensions - 2; k++){
        //         bottomPos = hg.getGrid(k + 1, 0);
        //         rightPos = hg.getGrid(dimensions - 1, k + 1);
        //         topPos = hg.getGrid(dimensions - k - 2,dimensions - 1);
        //         leftPos = hg.getGrid(0, dimensions - 2 - k);

        //         //lerp bottom to left
        //         hg.getHedgie(k + 1, 0).getObject().transform.position = Vector2.Lerp(hg.getGrid(k + 1, 0), leftPos, complete);
        //         //lerp right to bottom
        //         hg.getHedgie(dimensions - 1, k + 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - 1, k + 1),bottomPos, complete);
        //         //lerp top to right
        //         hg.getHedgie(dimensions - k - 2,dimensions - 1).getObject().transform.position = Vector2.Lerp(hg.getGrid(dimensions - k - 2,dimensions - 1), rightPos, complete);
        //         //lerp left to top
        //         hg.getHedgie(0, dimensions - 2 - k).getObject().transform.position = Vector2.Lerp(hg.getGrid(0, dimensions - 2 - k), topPos, complete);
        //     }

        //     if(complete >= 1.0f){
        //         if(doubleClockwise){
        //             if(onceAround){
        //                 doubleClockwise = false;
        //             }else{
        //                 onceAround = true;
        //                 complete = 0;
        //                 clockStart = Time.time;
        //             }
        //         }
        //         clockwise = false;
        //         for (int k = 0; k < dimensions - 2; k++){
        //             Hedgie bottom = new Hedgie(hg.getHedgie(k + 1, 0));
        //             Hedgie right = new Hedgie(hg.getHedgie(dimensions - 1, k + 1));
        //             Hedgie top = new Hedgie(hg.getHedgie(dimensions - k - 2, dimensions - 1));
        //             Hedgie left = new Hedgie(hg.getHedgie(0, dimensions - k - 2));

        //             hg.setHedgie(k + 1, 0, right);//bottom is given right
        //             hg.setHedgie(dimensions - 1, k + 1, top);//right is given top
        //             hg.setHedgie(dimensions - k - 2, dimensions - 1, left);//top is given left
        //             hg.setHedgie(0, dimensions - 2 - k, bottom);//left is given bottom

                    // EntropyTree.instance.SetOuterHedgehog(k + 1, 0, right.getColor());//bottom is given right
                    // EntropyTree.instance.SetOuterHedgehog(dimensions - 1, k + 1, top.getColor());//right is given top
                    // EntropyTree.instance.SetOuterHedgehog(dimensions - k - 2, dimensions - 1, left.getColor());//top is given left
                    // EntropyTree.instance.SetOuterHedgehog(0, dimensions - 2 - k, bottom.getColor());//left is given bottom
        //         }
        //     }
        // }
        // if(spinning){
        //     float timeSinceRotation = Time.time - spinClockStart;
        //     float complete = timeSinceRotation / rotationTime;
        //     for(int x = 0; x < dimensions; x++){
        //         for(int y = 0; y < dimensions; y++){
        //             hg.getHedgie(x,y).getObject().transform.rotation = Quaternion.Slerp(qStart, qEnd, complete);
        //         }
        //     }
        // }
    }

    public void rotateCounterclockwise(){
        hg.RotateCounterclockwise();
    }

    public void rotateClockwise(){
        hg.RotateClockwise();
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

    private void Move(Coords start, Coords end){
        hg.MoveHedgehog(start, end);
        // movStart = start;
        // movEnd = end;
        // movStartPos = hg.getGrid(start.x,start.y);
        // movEndPos = hg.getGrid(end.x, end.y);
        moving = true;
        // clockStart = Time.time;
    } 

    public void FinishedMovingHedgie(Coords movStart, Coords movEnd){
        SpawnOuterBall(movStart.x, movStart.y);
        pops.Advent(movEnd.x, movEnd.y);
        checkRestart(innerBalls);
        moving = false;
    }

    public void FinishedRotation(){
        clockwise = false;
        counterclockwise = false;
        doubleClockwise = false;
    }

    public void checkRestart(int newBalls){
        if(hg.getBallCount() <= 0){
            Debug.Log("Won in " + numberOfMoves + " moves");
            RequestManager.instance.PostPoint(true, dimensions - 2, innerBalls, numberOfMoves, start_entropy);
            NormRestart();
            // SpawnInnerBalls(newBalls); 
            hg.DestroyAll();
            MakeGrid();
        }
    }

    private void AIRestart(){
        numberOfMoves = 0;
        numberOfGames++;
        if(numberOfGames >= gamesToPlay){
            innerBalls++;
            if(innerBalls >= ((dimensions - 2)*(dimensions - 3))){//simplification of (dim-2)*(dim-2)-(dim-2)
                canMakeAIMove = false;
                isAIOn = false;
            }else
            numberOfGames = 0;
        }
    }

    private void NormRestart(){
        DiffCon.NextLevel();
        DiffCon.CalculateTendencies();
        innerBalls = DiffCon.CalculateInnerHedgehogs();
        dimensions = DiffCon.CalculateDimensions(innerBalls, dimensions);
    }

    public HedgieSprites getHedgieSprites(){
        return hsprites;
    }

    public void MakeAIMove(){
        if (aiMoves == null || aiMoves.Count == 0) {
            aiMoves = EntropyTree.instance.FindMoves(hg.GetHedgieGrid());
        }
        if(aiMoves != null){
            Move nextMove = aiMoves[0];
            aiMoves.RemoveAt(0);
            StartCoroutine(AIMove(nextMove));
        }else{
            numberOfGames++;
            RequestManager.instance.PostPoint(false, dimensions - 2, innerBalls, numberOfMoves, start_entropy);
            Debug.Log("Lost in " + numberOfMoves + " moves");
            numberOfMoves = 0;
            SpawnOuterBalls();
            SpawnInnerBalls(innerBalls); 
            if(numberOfGames >= gamesToPlay){
                canMakeAIMove = false;
                isAIOn = false;
            }else{
                MakeAIMove();
            }
        }
    }

    IEnumerator WaitToMakeMove(Move move){
        StartCoroutine(AIMove(move));
        yield return new WaitForSeconds(rotationTime + aiSlow);
    }

    IEnumerator WaitToMakeMoves(List<Move> moves){
        for(int k = 0; k < moves.Count; k++){
            StartCoroutine(AIMove(moves[k]));
            yield return new WaitForSeconds(rotationTime + aiSlow * 2);
        }
    }

    public void TurnOnAIMode(){
        if(!isAIOn){
            numberOfMoves = 0;
            canMakeAIMove = true;
            isAIOn = true;
        }else{
            numberOfMoves = 0;
            canMakeAIMove = false;
            isAIOn = false;
        }
        // MakeAIMove();
    }

    IEnumerator AIMove(Move move){
        numberOfMoves++;

        Coords click = new Coords(0,0);
        Vector2 end = new Vector2();

        switch(move.rotation){
            case 0:
                break;
            case 1:
                rotateClockwise();
                break;
            case 3://was -1
                rotateCounterclockwise();
                break;
            case 2:
                rotateDoubleClockwise();
                break;
            default:
               Debug.Log("weird");
                break;
        }

        yield return new WaitForSeconds(rotationTime + aiSlow);

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

    public void SetBoardState(){
        EntropyTree.instance.SetBoardState(hg.GetHedgieGrid());
    }
}
