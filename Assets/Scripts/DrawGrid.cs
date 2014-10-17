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
        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkTouch(Input.GetTouch(0).position);
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
        if (ballCount < 1)
        {
            SpawnInnerBalls(innerBalls);
            ballCount = innerBalls;
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

        if (click.x == 0f && click.y == 0f)
        {
            rotateClockwise();
        }

        if (click.x == dimensions - 1 && click.y == 0f)
        {
            rotateCounterclockwise();
        }
    }

    void rotateCounterclockwise()
    {
        Hedgie left, right, top, bottom;
        Vector2 leftPos, rightPos, topPos, bottomPos;
        for (int k = 0; k < dimensions - 2; k++)
        {
            bottom = new Hedgie(h[k + 1, 0]);
            bottomPos = grid[k + 1, 0];

            right = new Hedgie(h[dimensions - 1, k + 1]);
            rightPos = grid[dimensions - 1, k + 1];

            top = new Hedgie(h[dimensions - k - 2, dimensions - 1]);
            topPos = grid[dimensions - k - 2, dimensions - 1];

            left = new Hedgie(h[0, dimensions - 2 - k]);
            leftPos = grid[0, dimensions - 2 - k];

            h[dimensions - 1, k + 1] = new Hedgie(bottom);//moves bottom to right
            h[dimensions - 1, k + 1].getObject().transform.position = rightPos;

            h[dimensions - k - 2, dimensions - 1] = new Hedgie(right);//moves right to top
            h[dimensions - k - 2, dimensions - 1].getObject().transform.position = topPos;

            h[0, dimensions - 2 - k] = new Hedgie(top); //moves top to left
            h[0, dimensions - 2 - k].getObject().transform.position = leftPos;

            h[k + 1, 0] = new Hedgie(left);//moves left to bottom
            h[k + 1, 0].getObject().transform.position = bottomPos;
        }
    }

    void rotateClockwise()
    {
        Hedgie left, right, top, bottom;
        Vector2 leftPos, rightPos, topPos, bottomPos;
        for (int k = 0; k < dimensions - 2; k++)
        {
            bottom = new Hedgie(h[k + 1, 0]);
            bottomPos = grid[k + 1, 0];

            right = new Hedgie(h[dimensions - 1, k + 1]);
            rightPos = grid[dimensions - 1, k + 1];

            top = new Hedgie(h[dimensions - k - 2, dimensions - 1]);
            topPos = grid[dimensions - k - 2, dimensions - 1];

            left = new Hedgie(h[0, dimensions - 2 - k]);
            leftPos = grid[0, dimensions - 2 - k];

            h[dimensions - 1, k + 1] = new Hedgie(top);//moves top to right
            h[dimensions - 1, k + 1].getObject().transform.position = rightPos;

            h[dimensions - k - 2, dimensions - 1] = new Hedgie(left);//moves left to top
            h[dimensions - k - 2, dimensions - 1].getObject().transform.position = topPos;

            h[0, dimensions - 2 - k] = new Hedgie(bottom); //moves bottom to left
            h[0, dimensions - 2 - k].getObject().transform.position = leftPos;

            h[k + 1, 0] = new Hedgie(right);//moves right to bottom
            h[k + 1, 0].getObject().transform.position = bottomPos;


        }
    }

    void move(Vector2 start, Vector2 end, float time)
    {
        ballCount++;
        //print(start + " " + end);
        Vector2 startPos = grid[(int)start.x,(int)start.y];
        Vector2 endPos = grid[(int)end.x, (int)end.y];
        h[(int)start.x, (int)start.y].getObject().transform.position = Vector3.Lerp(startPos, endPos, 1f);
        h[(int)end.x, (int)end.y] = h[(int)start.x, (int)start.y];
        h[(int)start.x, (int)start.y] = new Hedgie();
        SpawnBall((int)start.x, (int)start.y);
        if (checkConnect((int)end.x,(int)end.y))
            explode((int)end.x, (int)end.y);
    }

    void travel(Vector2 startPos, Vector2 endPos, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            
        }
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
            for (int k = dimensions - 3; k > 1; k--)//checks sequentially 
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
            for (int k = dimensions - 3; k > 1; k--)//checks sequentially below where was clicked 
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
