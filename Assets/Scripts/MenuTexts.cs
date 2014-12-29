using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEngine.EventSystems;
public class MenuTexts : MonoBehaviour {

    private int dimensions, innerHedgies, normalTend, armorTend, splitterTend, armorMin, splitterMin, armorMax, splitterMax;
    private GridControls gc;
    public GameObject go;


    void Start() {
        gc = go.GetComponent<GridControls>() as GridControls;
    }

    public void onClick() {
        InputField[] fields = gameObject.GetComponentsInChildren<InputField>();
        
        int.TryParse(fields[0].text, out dimensions);
        int.TryParse(fields[1].text, out innerHedgies);
        int.TryParse(fields[2].text, out normalTend);
        int.TryParse(fields[3].text, out armorTend);
        int.TryParse(fields[4].text, out splitterTend);
        int.TryParse(fields[5].text, out armorMin);
        int.TryParse(fields[6].text, out splitterMin);
        int.TryParse(fields[7].text, out armorMax);
        int.TryParse(fields[8].text, out splitterMax);

        gc.setParams(dimensions, innerHedgies, normalTend, armorTend, splitterTend, armorMin, splitterMin, armorMax, splitterMax);
        gc.MakeGrid();
        gameObject.SetActive(false);
    }

    public void remenu() {
        gameObject.SetActive(true);
    }
}
