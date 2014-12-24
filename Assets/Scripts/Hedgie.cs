using UnityEngine;
using System.Collections;

public class Hedgie 
{
    private Sprite s;
    private GameObject go;//gameObject associated with Hedgie
    private int color, type, health;//color of the ball, and type of the ball
    private SpriteRenderer sprender;//use this to turn off hedgies not in use
    private TextMesh healthText;
    public Hedgie()
    {
        go = new GameObject();
        sprender = go.AddComponent<SpriteRenderer>();
        healthText = go.GetComponentInChildren<TextMesh>();
        color = -1;
        type = -1;
        s = new Sprite();
        sprender.enabled = true;
    }

    public Hedgie(GameObject go, Sprite s, int color, int type, int health)
    {
        if(go == null){
            go = new GameObject();
        }
        this.go = go;
        this.s = s;
        this.color = color;
        this.type = type;
        this.health = health;
        healthText = go.GetComponentInChildren<TextMesh>();
        if (health > 1) {
            healthText.text = health.ToString();
        }
        else {
            healthText.text = "";
        }
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
        health = h.getHealth();
        sprender = go.GetComponent<SpriteRenderer>();
        sprender.sprite = s;
        healthText = go.GetComponentInChildren<TextMesh>();
        if (health > 1) {
            healthText.text = health.ToString();
        }
        else {
            healthText.text = " ";
        }
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

    public int getHealth(){
        return health;
    }

    public string getText() {
        return healthText.text;
    }

    public void setObject(GameObject go){
        this.go = go;
    }

    public void setSprite(Sprite s){
        this.s = s;
        sprender.sprite = s;
    }

    public void setColor(int color){
        this.color = color;
    }

    public void setType(int type){
        this.type = type;
    }

    public void setHealth(int health){
        this.health = health;
        if (health > 1) {
            healthText.text = health.ToString();
        }
        else if (health <= 0) {
            healthText.text = "";
        }
    }

    public void setText(string text) {
        healthText.text = text;
    }

    public void transmogrify(Hedgie h){
        s = h.getSprite();
        color = h.getColor();
        type = h.getType();
        sprender.sprite = s;
        health = h.getHealth();
        healthText.text = h.getText();
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public void transmogrify(Sprite s, int color, int type, int health){
        this.s = s;
        this.color = color;
        this.type = type;
        this.health = health;
        sprender.sprite = s;
        healthText.text = health.ToString();
        if(color == -1){
            sprender.enabled = false;
        }else{
            sprender.enabled = true;
        }
    }

    public int loseHealh(int damage) {
        health += damage;
        if (health > 1) {
            healthText.text = health.ToString();
            return 0;
        }
        else if (health == 1) {
            healthText.text = "";
            return 0;
        }
        else {
            healthText.text = "";
            pop();
            return -1;
        }
    }

    public int pop(){
            type = -1;
            color = -1;
            sprender.enabled = false;
            return -1;
    }
}

