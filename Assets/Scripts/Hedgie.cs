using UnityEngine;
using System.Collections;

public class Hedgie 
{
        GameObject go;
        Vector2 location;
        int type;

        public Hedgie()
        {
            go = new GameObject();
            location = new Vector2();
            type = -1;
        }

        public Hedgie(GameObject go, Vector2 location, int type)
        {
            this.go = go;
            this.location = location;
            this.type = type;
        }

        public Hedgie(Hedgie h)
        {
            go = h.getObject();
            location = h.getLocation();
            type = h.getType();
        }

        public GameObject getObject()
        {
            return go;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        public int getType()
        {
            return type;
        }

        public void setObject(GameObject value)
        {
            go = value;
        }

        public void setLocation(Vector2 value)
        {
            location = value;
        }

        public void setType(int value)
        {
            type = value;
        }
}

