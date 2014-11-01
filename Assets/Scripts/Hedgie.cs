using UnityEngine;
using System.Collections;

public class Hedgie 
{
    private Sprite s;
    private GameObject go;//gameObject associated with Hedgie
    private int color, type;//color of the ball, and type of the ball
    private SpriteRenderer sprender;//use this to turn off hedgies not in use

    public Hedgie()
    {
        go = new GameObject();
        sprender = go.AddComponent<SpriteRenderer>();
        color = -1;
        type = -1;
        s = new Sprite();
        sprender.enabled = true;
    }

    public Hedgie(GameObject go, Sprite s, int color, int type)
    {
        if(go == null){
            go = new GameObject();
        }
        this.go = go;
        this.s = s;
        this.color = color;
        this.type = type;
        sprender = go.GetComponent<SpriteRenderer>();
        sprender.sprite = s;
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public Hedgie(Hedgie h)
    {
        go = h.getObject();
        s = h.getSprite();
        color = h.getColor();
        type = h.getType();
        sprender = go.GetComponent<SpriteRenderer>();
        sprender.sprite = s;
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public GameObject getObject(){
        return go;
    }

    public Sprite getSprite(){
        return s;
    }

    public int getColor(){
        return color;
    }

    public int getType(){
        return type;
    }

    public void setObject(GameObject go){
        this.go = go;
    }

    public void setSprite(Sprite s){
        this.s = s;
    }

    public void setColor(int color){
        this.color = color;
    }

    public void setType(int type){
        this.type = type;
    }

    public void setHedgie(Hedgie h){
        go = h.getObject();
        s = h.getSprite();
        color = h.getColor();
        type = h.getType();
        sprender = go.GetComponent<SpriteRenderer>();
        sprender.sprite = s;
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public void transmogrify(Hedgie h){
        s = h.getSprite();
        color = h.getColor();
        type = h.getType();
        sprender.sprite = s;
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public void transmogrify(Sprite s, int color, int type){
        this.s = s;
        this.color = color;
        this.type = type;
        sprender.sprite = s;
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public void pop(){
        type = -1;
        color = -1;
        sprender.enabled = false;
    }
}

