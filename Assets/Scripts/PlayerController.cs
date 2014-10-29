using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GridControls))]
public class PlayerController : MonoBehaviour {

	public float tapDeviation = 20;
	private RuntimePlatform platform = Application.platform;
	private GridControls gc;
	private HedgieGrid hg;
	private Vector2 touchStart, touchEnd;
	void Start () {
		gc = GetComponent<GridControls>() as GridControls;
	}
	
	void Update()
    {
        if(!gc.InMotion()){
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
                        if(Vector2.Distance(touchStart,touchEnd) > tapDeviation){
                        //swipes
                        if(touchStart.y > Screen.height/2){//top half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                gc.rotateClockwise();
                            }else{//going left
                                gc.rotateCounterclockwise();
                            }
                        }else{//bottom half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                gc.rotateCounterclockwise();
                            }
                            else{//going left
                                gc.rotateClockwise();
                            }
                        }
                    }else{
                        //taps
                        gc.checkTouch(touch.position);
                    }
                    }
                }
            }
        
            else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    gc.checkTouch(Input.mousePosition);
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
                    touchEnd = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
                    if(Vector2.Distance(touchStart,touchEnd) > tapDeviation){
                        //swipes
                        if(touchStart.y > Screen.height/2){//top half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                gc.rotateClockwise();
                            }else{//going left
                                gc.rotateCounterclockwise();
                            }
                        }else{//bottom half of the screen
                            if(touchEnd.x > touchStart.x){//going right
                                gc.rotateCounterclockwise();
                            }
                            else{//going left
                                gc.rotateClockwise();
                            }
                        }
                    }else{
                        //taps
                        gc.checkTouch(Input.mousePosition);
                    }
                }
    		}
        }
    }
}
