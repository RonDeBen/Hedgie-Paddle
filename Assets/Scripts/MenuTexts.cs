using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
public class MenuTexts : MonoBehaviour {

    public static MenuTexts instance;

    private int dimensions, innerHedgies, normalTend, armorTend, splitterTend, armorMin, splitterMin, armorMax, splitterMax;
    private GridControls gc;
    public bool isActive = true;


    void Awake() {
        gc = gameObject.GetComponent<GridControls>() as GridControls;
        if(instance != null){
            GameObject.Destroy(instance);
        }
        instance = this;
    }

    public void onClick() {
        Debug.Log("what");
        InputField[] fields = gameObject.GetComponentsInChildren<InputField>();
        
        int.TryParse(fields[0].text, out dimensions);
        if(dimensions == 0)
            dimensions = 11;
        int.TryParse(fields[1].text, out innerHedgies);
        if(innerHedgies == 0)
            innerHedgies = 15;
        int.TryParse(fields[2].text, out normalTend);
        if(normalTend == 0)
            normalTend = 70;
        int.TryParse(fields[3].text, out armorTend);
        if(armorTend == 0)
            armorTend = 20;
        int.TryParse(fields[4].text, out splitterTend);
        if(splitterTend == 0)
            splitterTend = 30;
        int.TryParse(fields[5].text, out armorMin);
        if(armorMin == 0)
            armorMin = 2;
        int.TryParse(fields[6].text, out splitterMin);
        if(splitterMin == 0)
            splitterMin = 2;
        int.TryParse(fields[7].text, out armorMax);
        if(armorMax == 0)
            armorMax = 3;
        int.TryParse(fields[8].text, out splitterMax);
        if(splitterMax == 0)
            splitterMax = 3;

        // gc.setParams(dimensions, innerHedgies, normalTend, armorTend, splitterTend, armorMin, splitterMin, armorMax, splitterMax);
        gc.MakeGrid();
        isActive = false;
        gameObject.SetActive(false);
    }

    public void remenu() {
        isActive = false;
        gameObject.SetActive(true);
    }
}
