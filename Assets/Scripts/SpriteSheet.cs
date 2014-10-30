using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpriteSheet{

	public string sheetName;
	public Sprite[] sheet;

	public Sprite getSprite(int index){
		return sheet[index];
	}
	public Sprite[] getSheet(){
		return sheet;
	}

	public string getSheetName(){
		return sheetName;
	}

	public int getLength(){
		return sheet.Length;
	}

	public void setSheet(Sprite[] sheet){
		this.sheet = sheet;
	}

	public void setSheetName(string sheetName){
		this.sheetName = sheetName;
	}
}
