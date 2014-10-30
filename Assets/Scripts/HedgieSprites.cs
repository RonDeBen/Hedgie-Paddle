using UnityEngine;
using System.Collections;

public class HedgieSprites : MonoBehaviour {

	public SpriteSheet[] ss;

	public Sprite[] getsSheet(string sheet){
		bool looping = true;
		int k = 0;
		while(looping && k < ss.Length){
			looping = !(sheet.Equals(ss[k].getSheetName()));
			if(looping){
				return ss[k].getSheet();
			}
		}
		return null;
	}	

	public Sprite[] getSheet(int k){
		return ss[k].getSheet();
	}

	public Sprite getSprite(int sheet, int color){
		return ss[sheet].getSprite(color);
	}

	public int getLength(){
		return ss.Length;
	}

	public int getSheetLength(int sheet){
		return ss[sheet].getLength();
	}
}
