using UnityEngine;
using System.Collections;

public class HedgieGrid : MonoBehaviour {

	public int dimensions;//grid size ex. 10 makes a 10x10 grid
	public float windUpTime, longSquish, shortSquish, movSpeed, turnTime, rotationTime, scaleFactor;
	public GridControls gc;
	private int ballCount;//number of balls in the center
	private Vector2 tile;//width and height of each rectangle in the grid
	private Vector2[,] grid;//the centers of each tile of the grid
	private Hedgie[,] h;//the hedgie in each tile of the grid
    private HedgieSprites hsprites;
    private Vector3[] correctedScales;

	public void SetUp (int dimensions, GameObject HedgieObject, Camera cam, HedgieSprites hsprites){
        this.hsprites = hsprites;
		ballCount = 0;
		this.dimensions = dimensions;
		grid = new Vector2[dimensions, dimensions];
		h = new Hedgie[dimensions, dimensions];

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        tile.x = width / dimensions;
        tile.y = height / dimensions;

        grid = findGridCenters();

        for(int x = 0; x < dimensions; x++){
			for(int y = 0; y < dimensions; y++){
				GameObject go = (GameObject)Instantiate(HedgieObject, new Vector3(grid[x,y].x, grid[x,y].y, 0), Quaternion.identity);
				h[x,y] = new Hedgie(go, hsprites.getSprite(0, 0), -1, -1, -1);
			}
		}

        float newWidth = 0;
        float newHeight = 0;
        Sprite spr = null;

        correctedScales = new Vector3[hsprites.getLength()];
        for(int k = 0; k < hsprites.getLength(); k++){
        	spr = hsprites.getSprite(k, 0);
        	newWidth = tile.x / (spr.rect.width / 100);
        	newHeight = tile.y / (spr.rect.height / 100);
			correctedScales[k] = new Vector3(newWidth, newHeight, 1f);
        }

        EntropyTree.SetDimensions(dimensions);
	}

	private Vector2[,] findGridCenters(){
		Vector2[,] g = new Vector2[dimensions, dimensions];
		for (int c = 0; c < dimensions; c++){
			for (int r = 0; r < dimensions; r++){
				g[r,c] = new Vector2((tile.x / 2) + (r * tile.x), (tile.y / 2) + (c * tile.y));
			}
		}
		return g;
	}

	public void MoveHedgehog(Coords start, Coords end){
		Hedgie hog = getHedgie(start.x, start.y);
		StartCoroutine(MoveRoutine(hog, start, end));
	}

	private IEnumerator MoveRoutine(Hedgie hog, Coords start, Coords end){
		float elapsed = 0f;
		float percentComplete = 0f;
        bool movingHorizontally = (start.x == 0 || start.x == dimensions - 1);

		while(elapsed < windUpTime){
			elapsed += Time.deltaTime;
			percentComplete = elapsed / windUpTime;
			Vector3 squishVector = new Vector3();
			if(movingHorizontally){
				float horizontalSquish = Mathf.Lerp(1f, longSquish, Mathf.Pow(percentComplete, 2f));
                float verticalSquish = Mathf.Lerp(1f, shortSquish, Mathf.Pow(percentComplete, 2f));
                squishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
			}else{
                float horizontalSquish = Mathf.Lerp(1f, shortSquish, Mathf.Pow(percentComplete, 2f));
                float verticalSquish = Mathf.Lerp(1f, longSquish, Mathf.Pow(percentComplete, 2f));
                squishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
			}
			hog.getObject().transform.localScale = Vector3.Scale(squishVector, correctedScales[hog.getType()]);
			
			yield return null;
		}

		elapsed = 0f;
		percentComplete = 0f;
        Vector2 startPos = getGrid(start.x,start.y);
        Vector2 endPos = getGrid(end.x, end.y);
		float fracJourney = 0f;
        while(fracJourney < 1f){
			elapsed += Time.deltaTime;
            float distCovered = elapsed * movSpeed;
            fracJourney = distCovered / Vector2.Distance(startPos, endPos);
			hog.getObject().transform.position = Vector2.Lerp(startPos, endPos, fracJourney);

			yield return null;
		}

		elapsed = 0f;
        while (elapsed < windUpTime) {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / windUpTime;
            Vector3 squishVector = new Vector3();
            if (movingHorizontally) {
                float horizontalSquish = Mathf.Lerp(longSquish, 1f, Mathf.Pow(percentComplete, 2f));
                float verticalSquish = Mathf.Lerp(shortSquish, 1f, Mathf.Pow(percentComplete, 2f));
                squishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
            } else {
                float horizontalSquish = Mathf.Lerp(shortSquish, 1f, Mathf.Pow(percentComplete, 2f));
                float verticalSquish = Mathf.Lerp(longSquish, 1f, Mathf.Pow(percentComplete, 2f));
                squishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
            }
            hog.getObject().transform.localScale = Vector3.Scale(squishVector, correctedScales[hog.getType()]);

            yield return null;
        }

        ballIncrement();
        transmogrify(end.x, end.y, hog);
        hog.getObject().transform.position = getGrid(start.x, start.y);

		gc.FinishedMovingHedgie(start, end);
	}

	public void RotateCounterclockwise(){
		StartCoroutine(RotateCounterclockwiseRoutine());
	}

	private IEnumerator RotateCounterclockwiseRoutine(){
		float elapsed = 0f;
		float percentComplete = 0f;
        Vector3 leftPos, rightPos, topPos, bottomPos;
		int offset = dimensions - 2;
		float[] gaussArr = new float[offset * 4];
		for(int k = 0; k < offset * 4; k++){
			gaussArr[k] = Mathy.NextGaussianFloat();
		}

        while (elapsed < turnTime) {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / turnTime;

            Quaternion bottomRot, rightRot, topRot, leftRot;
            Vector3 botToRightDir, rightToTopDir, topToLeftDir, leftToBottomDir;

            bottomPos = getGrid(1, 0);
            rightPos = getGrid(dimensions - 1, 1);
            topPos = getGrid(dimensions - 2, dimensions - 1);
            leftPos = getGrid(0, dimensions - 2);

            botToRightDir = (rightPos - getHedgie(1, 0).getObject().transform.position).normalized;
            rightToTopDir = (topPos - getHedgie(dimensions - 1, 1).getObject().transform.position).normalized;
            topToLeftDir = (leftPos - getHedgie(dimensions - 2, dimensions - 1).getObject().transform.position).normalized;
            leftToBottomDir = (bottomPos - getHedgie(0, dimensions - 2).getObject().transform.position).normalized;

            bottomRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: botToRightDir);
            rightRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: rightToTopDir);
            topRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: topToLeftDir);
            leftRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: leftToBottomDir);

            Vector3 verticalSquishVector, horizontalSquishVector;
            float horizontalSquish = Mathf.Lerp(1f, longSquish, Mathf.Pow(percentComplete, 2f));
            float verticalSquish = Mathf.Lerp(1f, shortSquish, Mathf.Pow(percentComplete, 2f));
            verticalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            horizontalSquish = Mathf.Lerp(1f, shortSquish, Mathf.Pow(percentComplete, 2f));
            verticalSquish = Mathf.Lerp(1f, longSquish, Mathf.Pow(percentComplete, 2f));
            horizontalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
            for (int k = 0; k < dimensions - 2; k++) {
                //slerp bottom to right
                GameObject bottomHog = getHedgie(k + 1, 0).getObject();
                bottomHog.transform.rotation = Quaternion.Slerp(bottomHog.transform.rotation, bottomRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k], 0.1f, 3f)));
                bottomHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(k + 1, 0).getType()]);
                //slerp right to top
                GameObject rightHog = getHedgie(dimensions - 1, k + 1).getObject();
                rightHog.transform.rotation = Quaternion.Slerp(rightHog.transform.rotation, rightRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset], 0.1f, 3f)));
                rightHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(dimensions - 1, k + 1).getType()]);
                //slerp top to left
                GameObject topHog = getHedgie(dimensions - k - 2, dimensions - 1).getObject();
                topHog.transform.rotation = Quaternion.Slerp(topHog.transform.rotation, topRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 2], 0.1f, 3f)));
                topHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(dimensions - k - 2, dimensions - 1).getType()]);
                //slerp left to bottom
                GameObject leftHog = getHedgie(0, dimensions - 2 - k).getObject();
                leftHog.transform.rotation = Quaternion.Slerp(leftHog.transform.rotation, leftRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 3], 0.1f, 3f)));
                leftHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
            }
            yield return null;
        }

        elapsed = 0f;
        
		while(elapsed < rotationTime){
			elapsed += Time.deltaTime;
			percentComplete = elapsed / rotationTime;

			Vector3 verticalSquishVector, horizontalSquishVector;
            float horizontalSquish = Mathf.Lerp(longSquish, shortSquish, MoveFunc(percentComplete));
            float verticalSquish = Mathf.Lerp(shortSquish, longSquish, MoveFunc(percentComplete));
            verticalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            horizontalSquish = Mathf.Lerp(shortSquish, longSquish, MoveFunc(percentComplete));
            verticalSquish = Mathf.Lerp(longSquish, shortSquish, MoveFunc(percentComplete));
            horizontalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

			for (int k = 0; k < dimensions - 2; k++) {
				bottomPos = getGrid(k + 1, 0);
				rightPos = getGrid(dimensions - 1, k + 1);
				topPos = getGrid(dimensions - k - 2, dimensions - 1);
				leftPos = getGrid(0, dimensions - 2 - k);

                //lerp bottom to right
                getHedgie(k + 1, 0).getObject().transform.position = Vector2.Lerp(getGrid(k + 1, 0), rightPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k]* scaleFactor, 0.1f, 3f)));
                getHedgie(k + 1, 0).getObject().transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
				//lerp right to top
                getHedgie(dimensions - 1, k + 1).getObject().transform.position = Vector2.Lerp(getGrid(dimensions - 1, k + 1), topPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k+offset]* scaleFactor, 0.1f, 3f)));
                getHedgie(dimensions - 1, k + 1).getObject().transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
			    //lerp top to left
                getHedgie(dimensions - k - 2, dimensions - 1).getObject().transform.position = Vector2.Lerp(getGrid(dimensions - k - 2, dimensions - 1), leftPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k+offset*2]* scaleFactor, 0.1f, 3f)));
                getHedgie(dimensions - k - 2, dimensions - 1).getObject().transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
				//lerp left to bottom
                getHedgie(0, dimensions - 2 - k).getObject().transform.position = Vector2.Lerp(getGrid(0, dimensions - 2 - k), bottomPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k+offset*3]* scaleFactor, 0.1f, 3f)));
                getHedgie(0, dimensions - 2 - k).getObject().transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
			}
            yield return null;
		}

		// counterclockwise = false;
		for (int k = 0; k < dimensions - 2; k++) {
			Hedgie bottom = new Hedgie(getHedgie(k + 1, 0));
			Hedgie right = new Hedgie(getHedgie(dimensions - 1, k + 1));
			Hedgie top = new Hedgie(getHedgie(dimensions - k - 2, dimensions - 1));
			Hedgie left = new Hedgie(getHedgie(0, dimensions - k - 2));

			setHedgie(k + 1, 0, left);//bottom is given left
			setHedgie(dimensions - 1, k + 1, bottom);//right is given bottom
			setHedgie(dimensions - k - 2, dimensions - 1, right);//top is given right
			setHedgie(0, dimensions - 2 - k, top);//left is given top

            EntropyTree.instance.SetOuterHedgehog(k + 1, 0, left.getColor());//bottom is given left
            EntropyTree.instance.SetOuterHedgehog(dimensions - 1, k + 1, bottom.getColor());//right is given bottom
            EntropyTree.instance.SetOuterHedgehog(dimensions - k - 2, dimensions - 1, right.getColor());//top is given right
            EntropyTree.instance.SetOuterHedgehog(0, dimensions - 2 - k, top.getColor());//left is given top
        }

		elapsed = 0f;

        while (elapsed < turnTime) {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / turnTime;

            Vector3 verticalSquishVector, horizontalSquishVector;
            float horizontalSquish = Mathf.Lerp(longSquish, 1f, Mathf.Pow(percentComplete, 2f));
            float verticalSquish = Mathf.Lerp(shortSquish, 1f, Mathf.Pow(percentComplete, 2f));
            verticalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            horizontalSquish = Mathf.Lerp(shortSquish, 1f, Mathf.Pow(percentComplete, 2f));
            verticalSquish = Mathf.Lerp(longSquish, 1f, Mathf.Pow(percentComplete, 2f));
            horizontalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
            for (int k = 0; k < dimensions - 2; k++) {
                //slerp bottom to right
                GameObject bottomHog = getHedgie(k + 1, 0).getObject();
                bottomHog.transform.rotation = Quaternion.Slerp(bottomHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k], 0.1f, 3f)));
                bottomHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(k + 1, 0).getType()]);
                //slerp right to top
                GameObject rightHog = getHedgie(dimensions - 1, k + 1).getObject();
                rightHog.transform.rotation = Quaternion.Slerp(rightHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset], 0.1f, 3f)));
                rightHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(dimensions - 1, k + 1).getType()]);
                //slerp top to left
                GameObject topHog = getHedgie(dimensions - k - 2, dimensions - 1).getObject();
                topHog.transform.rotation = Quaternion.Slerp(topHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 2], 0.1f, 3f)));
                topHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(dimensions - k - 2, dimensions - 1).getType()]);
                //slerp left to bottom
                GameObject leftHog = getHedgie(0, dimensions - 2 - k).getObject();
                leftHog.transform.rotation = Quaternion.Slerp(leftHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 3], 0.1f, 3f)));
                leftHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
            }
            yield return null;
        }

		gc.FinishedRotation();
    }

	public void RotateClockwise(){
		StartCoroutine(RotateClockwiseRoutine(rotationTime, true));
	}

	private float MoveFunc(float x){
        return -(x - 1f) * x;
	}

	private IEnumerator RotateClockwiseRoutine(float spinTime, bool derp){
        float elapsed = 0f;
        float percentComplete = 0f;
        Vector3 leftPos, rightPos, topPos, bottomPos;
        int offset = dimensions - 2;
        float[] gaussArr = new float[offset * 4];
        for (int k = 0; k < offset * 4; k++) {
            gaussArr[k] = Mathy.NextGaussianFloat();
        }

		while(elapsed < turnTime){
			elapsed += Time.deltaTime;
			percentComplete = elapsed / turnTime;

			Quaternion bottomRot, rightRot, topRot, leftRot;
			Vector3 botToLeftDir, rightToBotDir, topToRightDir, leftToTopDir;

            bottomPos = getGrid(1, 0);
            rightPos = getGrid(dimensions - 1,1);
            topPos = getGrid(dimensions - 2, dimensions - 1);
            leftPos = getGrid(0, dimensions - 2);

			botToLeftDir = (leftPos - getHedgie(1, 0).getObject().transform.position).normalized;
			rightToBotDir = (bottomPos - getHedgie(dimensions - 1, 1).getObject().transform.position).normalized;
			topToRightDir = (rightPos - getHedgie(dimensions - 2, dimensions - 1).getObject().transform.position).normalized;
			leftToTopDir = (topPos - getHedgie(0, dimensions - 2).getObject().transform.position).normalized;
			
			bottomRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: botToLeftDir);
            rightRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: rightToBotDir);
            topRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: topToRightDir);
            leftRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: leftToTopDir);

            Vector3 verticalSquishVector, horizontalSquishVector;
			float horizontalSquish = Mathf.Lerp(1f, longSquish, Mathf.Pow(percentComplete, 2f));
			float verticalSquish = Mathf.Lerp(1f, shortSquish, Mathf.Pow(percentComplete, 2f));
			verticalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            verticalSquish = Mathf.Lerp(1f, shortSquish, Mathf.Pow(percentComplete, 2f));
            horizontalSquish = Mathf.Lerp(1f, longSquish, Mathf.Pow(percentComplete, 2f));
			horizontalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
            for (int k = 0; k < dimensions - 2; k++) {
                //lerp bottom to left
				GameObject bottomHog = getHedgie(k + 1, 0).getObject();
                bottomHog.transform.rotation = Quaternion.Slerp(bottomHog.transform.rotation, bottomRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k], 0.1f, 3f)));
                bottomHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(k + 1, 0).getType()]);
                //lerp right to bottom
                GameObject rightHog = getHedgie(dimensions - 1, k + 1).getObject();
                rightHog.transform.rotation = Quaternion.Slerp(rightHog.transform.rotation, rightRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset], 0.1f, 3f)));
                rightHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(dimensions - 1, k + 1).getType()]);
                //lerp top to right
                GameObject topHog = getHedgie(dimensions - k - 2, dimensions - 1).getObject();
                topHog.transform.rotation = Quaternion.Slerp(topHog.transform.rotation, topRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 2], 0.1f, 3f)));
                topHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(dimensions - k - 2, dimensions - 1).getType()]);
                //lerp left to top
                GameObject leftHog = getHedgie(0, dimensions - 2 - k).getObject();
                leftHog.transform.rotation = Quaternion.Slerp(leftHog.transform.rotation, leftRot, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 3], 0.1f, 3f)));
                leftHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
			}
			yield return null;
		}

		elapsed = 0f;

        while (elapsed < spinTime) {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / spinTime;

            Vector3 verticalSquishVector, horizontalSquishVector;
            float horizontalSquish = Mathf.Lerp(longSquish, shortSquish, MoveFunc(percentComplete));
            float verticalSquish = Mathf.Lerp(shortSquish, longSquish, MoveFunc(percentComplete));
            verticalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            horizontalSquish = Mathf.Lerp(shortSquish, longSquish, MoveFunc(percentComplete));
            verticalSquish = Mathf.Lerp(longSquish, shortSquish, MoveFunc(percentComplete));
            horizontalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            for (int k = 0; k < dimensions - 2; k++) {
                bottomPos = getGrid(k + 1, 0);
                rightPos = getGrid(dimensions - 1, k + 1);
                topPos = getGrid(dimensions - k - 2, dimensions - 1);
                leftPos = getGrid(0, dimensions - 2 - k);

                //lerp bottom to left
                getHedgie(k + 1, 0).getObject().transform.position = Vector2.Lerp(getGrid(k + 1, 0), leftPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k] * scaleFactor, 0.1f, 3f)));
                getHedgie(k + 1, 0).getObject().transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
				//lerp right to bottom
                getHedgie(dimensions - 1, k + 1).getObject().transform.position = Vector2.Lerp(getGrid(dimensions - 1, k + 1), bottomPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset] * scaleFactor, 0.1f, 3f)));
                getHedgie(dimensions - 1, k + 1).getObject().transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
				//lerp top to right
                getHedgie(dimensions - k - 2, dimensions - 1).getObject().transform.position = Vector2.Lerp(getGrid(dimensions - k - 2, dimensions - 1), rightPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 2] * scaleFactor, 0.1f, 3f)));
                getHedgie(dimensions - k - 2, dimensions - 1).getObject().transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
				//lerp left to top
                getHedgie(0, dimensions - 2 - k).getObject().transform.position = Vector2.Lerp(getGrid(0, dimensions - 2 - k), topPos, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 3] * scaleFactor, 0.1f, 3f)));
                getHedgie(0, dimensions - 2 - k).getObject().transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
            }
            yield return null;
        }
        for (int k = 0; k < dimensions - 2; k++) {
            Hedgie bottom = new Hedgie(getHedgie(k + 1, 0));
            Hedgie right = new Hedgie(getHedgie(dimensions - 1, k + 1));
            Hedgie top = new Hedgie(getHedgie(dimensions - k - 2, dimensions - 1));
            Hedgie left = new Hedgie(getHedgie(0, dimensions - k - 2));

            setHedgie(k + 1, 0, right);//bottom is given right
            setHedgie(dimensions - 1, k + 1, top);//right is given top
            setHedgie(dimensions - k - 2, dimensions - 1, left);//top is given left
            setHedgie(0, dimensions - 2 - k, bottom);//left is given bottom

            EntropyTree.instance.SetOuterHedgehog(k + 1, 0, right.getColor());//bottom is given right
            EntropyTree.instance.SetOuterHedgehog(dimensions - 1, k + 1, top.getColor());//right is given top
            EntropyTree.instance.SetOuterHedgehog(dimensions - k - 2, dimensions - 1, left.getColor());//top is given left
            EntropyTree.instance.SetOuterHedgehog(0, dimensions - 2 - k, bottom.getColor());//left is given bottom
        }

		elapsed = 0f;

        while (elapsed < turnTime) {
            elapsed += Time.deltaTime;
            percentComplete = elapsed / turnTime;

            Vector3 verticalSquishVector, horizontalSquishVector;
            float horizontalSquish = Mathf.Lerp(longSquish, 1f, Mathf.Pow(percentComplete, 2f));
            float verticalSquish = Mathf.Lerp(shortSquish, 1f, Mathf.Pow(percentComplete, 2f));
            verticalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);

            horizontalSquish = Mathf.Lerp(shortSquish, 1f, Mathf.Pow(percentComplete, 2f));
            verticalSquish = Mathf.Lerp(longSquish, 1f, Mathf.Pow(percentComplete, 2f));
            horizontalSquishVector = new Vector3(horizontalSquish, verticalSquish, 1f);
            for (int k = 0; k < dimensions - 2; k++) {
                //lerp bottom to left
                GameObject bottomHog = getHedgie(k + 1, 0).getObject();
                bottomHog.transform.rotation = Quaternion.Slerp(bottomHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k], 0.1f, 3f)));
                bottomHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(k + 1, 0).getType()]);
                //lerp right to bottom
                GameObject rightHog = getHedgie(dimensions - 1, k + 1).getObject();
                rightHog.transform.rotation = Quaternion.Slerp(rightHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset], 0.1f, 3f)));
                rightHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(dimensions - 1, k + 1).getType()]);
                //lerp top to right
                GameObject topHog = getHedgie(dimensions - k - 2, dimensions - 1).getObject();
                topHog.transform.rotation = Quaternion.Slerp(topHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 2], 0.1f, 3f)));
                topHog.transform.localScale = Vector3.Scale(verticalSquishVector, correctedScales[getHedgie(dimensions - k - 2, dimensions - 1).getType()]);
                //lerp left to top
                GameObject leftHog = getHedgie(0, dimensions - 2 - k).getObject();
                leftHog.transform.rotation = Quaternion.Slerp(leftHog.transform.rotation, Quaternion.identity, Mathf.Pow(percentComplete, Mathf.Clamp(1 + gaussArr[k + offset * 3], 0.1f, 3f)));
                leftHog.transform.localScale = Vector3.Scale(horizontalSquishVector, correctedScales[getHedgie(0, dimensions - 2 - k).getType()]);
            }
            yield return null;
        }

		if(derp)
			gc.FinishedRotation();
	}

	public void RotateDoubleClockwise(){
		StartCoroutine(RotateClockwiseRoutine(rotationTime / 2f, false));
        StartCoroutine(RotateClockwiseRoutine(rotationTime / 2f, true));
	}

	public void pop(int x, int y){
		ballCount += h[x,y].pop();
	}

	public int getDimensions(){
		return dimensions;
	} 

	public int getBallCount(){
		return ballCount;
	}

	public int CountBalls(){
		return 0;
	}

	public void ballIncrement(){
		ballCount++;
	}

	public void ballDecrement(){
		ballCount--;
	}

    public GameObject getObject(int x, int y) {
        return h[x, y].getObject();
    }
	public Vector2 getTile(){
		return tile;
	} 

	public Vector2[,] getGrid(){
		return grid;
	}

	public Vector2 getGrid(int x, int y){
		return grid[x,y];
	}

	public Hedgie[,] GetHedgieGrid(){
		return h;
	}

	public Hedgie getHedgie(int x, int y){
		return h[x,y];
	}

	public int getColor(int x, int y){
		return h[x,y].getColor();
	}

	public int getType(int x, int y){
		return h[x,y].getType();
	}

    public int getHealth(int x, int y) {
        return h[x, y].getHealth();
    }

	public void setDimensions(int value){
		dimensions = value;
	}

	public void setBallCount(int value){
		ballCount = value;
	}

    public void setSprite(int x, int y, Sprite s) {
        h[x, y].setSprite(s);
    }
	public void setTile(Vector2 value){
		value = tile;
	} 

	public void setGrid(Vector2[,] values){
		grid = values;
	}
	public void setGrid(int x, int y, Vector2 value){
		grid[x,y] = value;
	}

	public void setHedgie(int x, int y, Hedgie value){
		h[x,y] = new Hedgie(value);
		changeLocalScale(x,y,value.getType());
	}

	public void setHedgie(Hedgie[,] values){
		h = values;
	}

    public void setHealth(int x, int y, int newHealth) {
        ballCount += h[x, y].setHealth(newHealth);
    }

    public void setText(int x, int y, string text) {
        h[x, y].setText(text);
    }

    public void setType(int x, int y, int type) {
        h[x, y].setType(type);
        h[x, y].setSprite(hsprites.getSprite(type, h[x, y].getColor()));
        changeLocalScale(x, y, type);
    }

    public void loseHealth(int x, int y, int damage) {
        ballCount += h[x, y].loseHealth(damage);
        if (h[x, y].getHealth() == 1 && h[x, y].getType() == 1) {
            h[x, y].setSprite(hsprites.getSprite(0, h[x, y].getColor()));
            changeLocalScale(x, y, 0);
        }
    }

	public void transmogrify (int x, int y, Hedgie hedgie){
		h[x,y].transmogrify(hedgie);
		changeLocalScale(x,y,hedgie.getType());
	}

	public void transmogrify(int x, int y, Sprite s, int color, int type, int health){
		h[x,y].transmogrify(s, color, type, health);
		changeLocalScale(x,y,type);
	}

	public void changeLocalScale(int x, int y, int type){
		h[x,y].getObject().transform.localScale = correctedScales[type];
	}

    public void DestroyAll(){
		ballCount = 0;
        for (int x = 0; x < dimensions; x++) {
			for (int y = 0; y < dimensions; y++) {
				h[x, y].pop();
				Destroy(h[x,y].getObject());
			}
		}
    }

	public void ClearInnerHedgehogs(){
		ballCount = 0;
        for (int x = 1; x < dimensions - 1; x++) {
            for (int y = 1; y < dimensions - 1; y++) {
                h[x, y].pop();
                // Destroy(h[x, y].getObject());
            }
        }
	}
}
