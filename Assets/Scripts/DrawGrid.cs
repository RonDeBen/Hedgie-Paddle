using UnityEngine;
using System.Collections;

public class DrawGrid : MonoBehaviour {

    private RuntimePlatform platform = Application.platform;

    public GameObject[] types;

    public Camera cam;

    public int dimensions, innerBalls;

    private float height, width;

    private Vector2[,] grid;
    private Vector2 tile;

    private Hedgie[,] h;

    private int ballCount;

    private Vector2 touchStart, touchEnd;

    public float rotationTime, movSpeed;
    private bool clockwise, counterclockwise, moving;
    private float clockStart;

    private Vector2 movStart, movEnd; 
    private Vector2 movStartPos, movEndPos;
	// Use this for initialization
	void Start () {
        ballCount = innerBalls;
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
        
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;

        tile.x = width / dimensions;
        tile.y = height / dimensions;

        Draw();

        SpawnOuterBalls();
        SpawnInnerBalls(innerBalls);
	}

    void Update()
    {
        if(!clockwise && !counterclockwise && !moving){
            if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
            {
                Touch touch = Input.GetTouch(0);
                TouchPhase phase = touch.phase;

                if (Input.touchCount > 0)
                {
                    if (phase == TouchPhase.Began)
                    {
                        touchStart = touch.position;
                    }
                    if(phase == TouchPhase.Ended)
                    {
                        touchEnd = touch.position;
                        if(Vector2.Distance(touchStart,touchEnd) > 20){
                        //swipes
                        if(touchStart.y > Screen.height/2){//top half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                rotateClockwise();
                            }else{//going left
                                rotateCounterclockwise();
                            }
                        }else{//bottom half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                rotateCounterclockwise();
                            }
                            else{//going left
                                rotateClockwise();
                            }
                        }
                    }else{
                        //taps
                        checkTouch(touch.position);
                    }
                    }
                }
            }
        
            else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    checkTouch(Input.mousePosition);
                }
            }
    		else if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
    		{
    			if (Input.GetMouseButtonDown(0))
    			{
    				touchStart = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
    			}
                if (Input.GetMouseButtonUp(0))
                {
                    print(Vector2.Distance(touchStart,touchEnd));
                    touchEnd = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
                    if(Vector2.Distance(touchStart,touchEnd) > 5){
                        //swipes
                        if(touchStart.y > Screen.height/2){//top half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                rotateClockwise();
                            }else{//going left
                                rotateCounterclockwise();
                            }
                        }else{//bottom half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                rotateCounterclockwise();
                            }
                            else{//going left
                                rotateClockwise();
                            }
                        }
                    }else{
                        //taps
                        checkTouch(Input.mousePosition);
                    }
                }
    		}
            if (ballCount < 1)
            {
                SpawnInnerBalls(innerBalls);
                ballCount = innerBalls;
            }
        }
    }

    void FixedUpdate(){
        if(counterclockwise){

            float timeSinceStarted = Time.time - clockStart;
            float complete = timeSinceStarted / rotationTime;

            Vector2 leftPos, rightPos, topPos, bottomPos;

            for (int k = 0; k < dimensions - 2; k++){
                bottomPos = grid[k + 1, 0];
                rightPos = grid[dimensions - 1, k + 1];
                topPos = grid[dimensions - k - 2,dimensions - 1];
                leftPos = grid[0, dimensions - 2 - k];

                //lerp bottom to right
                h[k + 1, 0].getObject().transform.position = Vector2.Lerp(grid[k + 1, 0], rightPos, complete);
                //lerp right to top
                h[dimensions - 1, k + 1].getObject().transform.position = Vector2.Lerp(grid[dimensions - 1, k + 1],topPos, complete);
                //lerp top to left
                h[dimensions - k - 2,dimensions - 1].getObject().transform.position = Vector2.Lerp(grid[dimensions - k - 2,dimensions - 1], leftPos, complete);
                //lerp left to bottom
                h[0, dimensions - 2 - k].getObject().transform.position = Vector2.Lerp(grid[0, dimensions - 2 - k], bottomPos, complete);
            }

            if(complete >= 1.0f){
                counterclockwise = false;
                for (int k = 0; k < dimensions - 2; k++){
                    Hedgie bottom = new Hedgie(h[k + 1, 0]);
                    Hedgie right = new Hedgie(h[dimensions - 1, k + 1]);
                    Hedgie top = new Hedgie(h[dimensions - k - 2, dimensions - 1]);
                    Hedgie left = new Hedgie(h[0, dimensions - k - 2]);

                    h[k + 1, 0].setHedgie(left);//bottom is given left
                    h[dimensions - 1, k + 1].setHedgie(bottom);//right is given bottom
                    h[dimensions - k - 2, dimensions - 1].setHedgie(right);//top is given right
                    h[0, dimensions - 2 - k].setHedgie(top);//left is given top
                }
            }
        }
        if(clockwise){
            float timeSinceStarted = Time.time - clockStart;
            float complete = timeSinceStarted / rotationTime;

            Vector2 leftPos, rightPos, topPos, bottomPos;

            for (int k = 0; k < dimensions - 2; k++){
                bottomPos = grid[k + 1, 0];
                rightPos = grid[dimensions - 1, k + 1];
                topPos = grid[dimensions - k - 2,dimensions - 1];
                leftPos = grid[0, dimensions - 2 - k];

                //lerp bottom to left
                h[k + 1, 0].getObject().transform.position = Vector2.Lerp(grid[k + 1, 0], leftPos, complete);
                //lerp right to bottom
                h[dimensions - 1, k + 1].getObject().transform.position = Vector2.Lerp(grid[dimensions - 1, k + 1],bottomPos, complete);
                //lerp top to right
                h[dimensions - k - 2,dimensions - 1].getObject().transform.position = Vector2.Lerp(grid[dimensions - k - 2,dimensions - 1], rightPos, complete);
                //lerp left to top
                h[0, dimensions - 2 - k].getObject().transform.position = Vector2.Lerp(grid[0, dimensions - 2 - k], topPos, complete);
            }

            if(complete >= 1.0f){
                clockwise = false;
                for (int k = 0; k < dimensions - 2; k++){
                    Hedgie bottom = new Hedgie(h[k + 1, 0]);
                    Hedgie right = new Hedgie(h[dimensions - 1, k + 1]);
                    Hedgie top = new Hedgie(h[dimensions - k - 2, dimensions - 1]);
                    Hedgie left = new Hedgie(h[0, dimensions - k - 2]);

                    h[k + 1, 0].setHedgie(right);//bottom is given right
                    h[dimensions - 1, k + 1].setHedgie(top);//right is given top
                    h[dimensions - k - 2, dimensions - 1].setHedgie(left);//top is given left
                    h[0, dimensions - 2 - k].setHedgie(bottom);//left is given bottom
                }
            }
        }
        if(moving){
            float distCovered = (Time.time - clockStart) * movSpeed;
            float fracJourney = distCovered / Vector2.Distance(movStartPos, movEndPos);

            h[(int)movStart.x, (int)movStart.y].getObject().transform.position = Vector2.Lerp(movStartPos, movEndPos, fracJourney);

            if(fracJourney >= 1.0f){
                ballCount++;
                h[(int)movEnd.x, (int)movEnd.y] = h[(int)movStart.x, (int)movStart.y];
                h[(int)movStart.x, (int)movStart.y] = new Hedgie();
                SpawnBall((int)movStart.x, (int)movStart.y);

                if (checkConnect((int)movEnd.x,(int)movEnd.y))
                    explode((int)movEnd.x, (int)movEnd.y);

                moving = false;
            }
        }
    }

    void checkTouch(Vector3 pos){
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 click = new Vector2((int)(wp.x / tile.x), (int)(wp.y / tile.y));
        //print(click);
        if (click.x == 0 && click.y != 0 && click.y != dimensions - 1)//if the left corner is clicked
        {
            int check = checkRight((int)click.y);//checks to the right of where was clicked
            //print(check);
            if (check != -1)
                move(new Vector2(click.x, click.y), new Vector2(check, click.y), 1f);
        }

        if (click.x == dimensions - 1 && click.y != 0 && click.y != dimensions - 1)//if the right corner is clicked
        {
            int check = checkLeft((int)click.y);//checks to the left of where was clicked
            //print(check);
            if (check != -1)
                move(new Vector2(click.x, click.y), new Vector2(check, click.y),1f);
        }

        if (click.y == 0 && click.x != 0 && click.x != dimensions - 1)//if the bottom corner is clicked
        {
            int check = checkUp((int)click.x);//checks above where was clicked
            //print(check);
            if (check != -1)
                move(new Vector2(click.x, click.y), new Vector2(click.x, check),1f);
        }

        if (click.y == dimensions - 1 && click.x != 0 && click.x != dimensions - 1)//if the top corner is clicked
        {
            int check = checkDown((int)click.x);//checks below where was clicked
            //print(check);
            if (check != -1)
                move(new Vector2(click.x, click.y), new Vector2(click.x, check),1f);
        }
    }

    void rotateCounterclockwise()
    {
            clockStart = Time.time;
            counterclockwise = true;
    }

    void rotateClockwise()
    {
            clockStart = Time.time;
            clockwise = true;
    }

    void move(Vector2 start, Vector2 end, float time)
    {
        movStart = start;
        movEnd = end;
        movStartPos = grid[(int)start.x,(int)start.y];
        movEndPos = grid[(int)end.x, (int)end.y];
        moving = true;
        clockStart = Time.time;
    } 

    int checkRight(int y)
    {
        if (h[1, y].getType() != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = 2; k < dimensions - 1; k++)
            {
                if (h[k, y].getType() != -1) 
                    return k - 1;
            }
            return -1;
        }
    }

    int checkLeft(int y)
    {
        if (h[dimensions - 2, y].getType() != -1)//if there's a ball in front of the ball you're shooting
            return -1;
        else
        {
            for (int k = dimensions - 3; k > 0; k--)//checks sequentially 
            {
                if (h[k, y].getType() != -1)
                    return k + 1;
            }
            return -1;//no balls on that row
        }
    }

    int checkUp(int x)
    {
        if (h[x, 1].getType() != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = 2; k < dimensions - 1; k++)//checks sequentially above where was clicked
            {
                if (h[x, k].getType() != -1)
                    return k - 1;//returns the y value before the first ball encountered 
            }
            return -1;
        }
    }

    int checkDown(int x)
    {
        if (h[x, dimensions - 2].getType() != -1)//if there's a ball in front of the ball
            return -1;
        else
        {
            for (int k = dimensions - 3; k > 0; k--)//checks sequentially below where was clicked 
            {
                if (h[x, k].getType() != -1)
                    return k + 1;//returns the y value before the first ball encountered
            }
            return -1;
        }
    }

    void explode(int x, int y)
    {
        for (int k = -1; k <= 1; k += 2)
        {
            if ((x + k) >= 1 && (x + k) <= dimensions - 2)
            {
                if (h[x, y].getType() == h[x + k, y].getType())
                {
                    pop(ref h[x + k, y]);
                    ballCount--;
                }
            }
            if ((y + k) >= 1 && (y + k) <= dimensions - 2)
            {
                if (h[x, y].getType() == h[x, y + k].getType())
                {
                    pop(ref h[x, y + k]);
                    ballCount--;
                }
            }
        }
        pop(ref h[x, y]);
        ballCount--;
    }

    void Draw()
    {
        for (int c = 0; c < dimensions; c++)
        {
            for (int r = 0; r < dimensions; r++)
            {
                grid[r, c] = new Vector2((tile.x / 2) + (r * tile.x), (tile.y / 2) + (c * tile.y));
            }
        }
    }

    void SpawnBall(int x, int y)
    {
        int ball = Random.Range(0, types.Length);
        Vector2 g = grid[x, y];
        GameObject go = (GameObject)Instantiate(types[ball], new Vector3(g.x, g.y, 0), Quaternion.identity);
        h[x, y].setObject(go);
        h[x, y].setType(ball);
    }

    void SpawnOuterBalls()
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

    void SpawnInnerBalls(int balls)
    {
        int counter, x, y;
        counter = 0;
        while (counter < balls)
        {
            x = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
            y = Random.Range(0, dimensions / 2) + Random.Range(0, dimensions / 2) + 1;
            if (h[x, y].getType() == -1)
            {
                counter++;
                SpawnBall(x, y);

                if (checkConnect(x, y))
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        if ((x+k) >= 1 && (x+k) <= dimensions - 2)
                        {
                            if (h[x, y].getType() == h[x + k, y].getType())
                            {
                                pop(ref h[x + k, y]);
                                counter--;
                            }
                        }
                        if ((y + k) >= 1 && (y + k) <= dimensions - 2)
                        {
                            if (h[x, y].getType() == h[x, y + k].getType())
                            {
                                pop(ref h[x, y + k]);
                                counter--;
                            }
                        }

                    }
                    
                    pop(ref h[x, y]);
                    counter--;
                }
            }
        }
    }

    bool checkConnect(int x, int y)
    {
        for (int k = -1; k <= 1; k += 2)
        {
            if ((x + k) >= 1 && (x + k) <= dimensions - 2)
            {
                if (h[x, y].getType() == h[x + k, y].getType())
                {
                    return true;
                }
            }
            if ((y + k) >= 1 && (y + k) <= dimensions - 2)
            {
                if (h[x, y].getType() == h[x, y + k].getType())
                {
                    return true;
                }
            }
            
        }

        return false;
    }

    void pop(ref Hedgie hedge)
    {
        Destroy(hedge.getObject());
        hedge = new Hedgie();
    }

    void GridLines()
    {
        /*float w = width / column;
        float h = height / row;

        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        lr.material = gridMat;
        lr.SetColors(gridColor, gridColor);
        lr.SetWidth(gridWidth, gridWidth);
        int verts = (column + 2) + (row + 2);
        lr.SetVertexCount(verts);
        Vector3 p = new Vector3(0,0,0);
        int pos = 0;
        lr.SetPosition(0, p);
        //vertical lines
        for (int k = 0; k < column; k++)
        {
            p = new Vector3(w * k, height * (k % 2), 0);
            lr.SetPosition(pos++, p);
            p = new Vector3(w * (k + 1), height * (k % 2), 0);
            lr.SetPosition(pos++, p);
        }*/


    }
}
