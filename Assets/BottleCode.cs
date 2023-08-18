using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;


[System.Serializable] public enum Facing
{
    N = 0,
    NNE = 1,
    NE = 2,
    ENE = 3,
    E = 4,
    ESE = 5,
    SE = 6,
    SSE = 7,
    S = 8,
    SSW = 9,
    SW = 10,
    WSW = 11,
    W = 12,
    WNW = 13,
    NW = 14,
    NNW = 15
}

public class Buffer<T> : LinkedList<T>
{
	private readonly int capacity;

	public bool IsFull => Count >= capacity;

	public Buffer(int capacity)
	{
		this.capacity = capacity;
	}

	public void Enqueue(T item)
	{
		if (Count == capacity)
		{
			RemoveFirst();
		}
		AddLast(item);
	}
}

public class BottleCode : MonoBehaviour
{
    [SerializeField] int pointSkip = 4;

    [SerializeField] float defaultSecsPerPoint = 5f;
    [SerializeField] int speedSampleCount = 1024, timePerPointSampleCount = 32;

    [SerializeField] TMP_Text textRPS, textBestRPS;
	[SerializeField] TMP_Text[] textCompassPoints;

    [SerializeField] TMP_Dropdown dropdownGameMode;
    [SerializeField] GameObject menuThing;

	public float currentAngle = 0;
    public Facing currentFacing, nextFacing;
    public int chosenDirection = 0;

    private Buffer<float> speedSamples;// = new Buffer<float>(1024);
    private Buffer<float> timePerPointSamples;// = new Buffer<float>(32);

    private float rotNow, prevRot, rotThisFrame, rotPerMin, bestRotPerMin, aveRPM;

    private float wantedAngle;

    private float currentVel;
    private float currentSmoothTime;

    private float timeLastPointInc = -1;

    private bool gameRunning;

    void Start()
    {
		dropdownGameMode.onValueChanged.AddListener(delegate { uiSetGameMode(dropdownGameMode.value); });

		speedSamples = new Buffer<float>(speedSampleCount);
		timePerPointSamples = new Buffer<float>(timePerPointSampleCount);

		resetForNewTurn();
        //uiSetGameMode(0);
	}

    public void startGame()
    {
        gameRunning = true;
        syncMenuThing();
        syncCompassPointTexts();
    }

    public void stopGame()
    {
        gameRunning = false;
        syncMenuThing();
    }

    public void uiSetGameMode(Int32 gameMode)
    {
        switch (gameMode)
        {
            case 0: pointSkip = 4; break;
			case 1: pointSkip = 2; break;
			case 2: pointSkip = 1; break;
		}

        syncCompassPointTexts();
    }

    private void syncMenuThing()
    {
        menuThing.SetActive(!gameRunning);
    }

    public void resetForNewTurn()
    {
        stopGame();

		currentFacing = Facing.N;
		chosenDirection = 0;
		currentAngle = 0;

        wantedAngle = 0f;

        currentVel = 0f;
        currentSmoothTime = defaultSecsPerPoint;

		rotNow = 0;
		prevRot = 0;

		timePerPointSamples.Clear();
		timePerPointSamples.Enqueue(defaultSecsPerPoint);

		bestRotPerMin = 0f;
		syncBestText();

		syncCompassPointTexts();

        transform.rotation = Quaternion.identity;
	}

    public bool isNextFacing(Facing nf)
    {
        if (chosenDirection != 0)
        {
            return getNextFacing(chosenDirection) == nf;
        }
        else if (getNextFacing(-1) == nf)
        {
            setChosenDirection(-1);
            return true;
        }
		else if (getNextFacing(1) == nf)
		{
			setChosenDirection(1);
			return true;
		}
        return false;
	}

    public void setChosenDirection(int dir)
    {
        chosenDirection = dir;
    }

    public void increaseFacing()
    {
        if (!gameRunning) return;

		currentFacing = getNextFacing(chosenDirection);
        syncCompassPointTexts();

        wantedAngle += chosenDirection * (90f/4f) * pointSkip;

        if (timeLastPointInc != -1)
        {
            float thisTime = Time.time - timeLastPointInc;
            timePerPointSamples.Enqueue(thisTime);
            Debug.Log(timePerPointSamples.Average());
		}

		timeLastPointInc = Time.time;
	}

    void Update()
    {
        //debug
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            chosenDirection = 1;
            increaseFacing();
        }
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			chosenDirection = -1;
			increaseFacing();
		}

		//float wantedAngle = facingToAngle(currentFacing);
        if (chosenDirection > 0)
            while (wantedAngle < currentAngle) wantedAngle += 360f;
        else if (chosenDirection < 0)
			while (wantedAngle > currentAngle) wantedAngle -= 360f;

        currentSmoothTime = Mathf.Lerp(currentSmoothTime, timePerPointSamples.Average(), Time.deltaTime);
        currentAngle = Mathf.SmoothDamp(currentAngle, wantedAngle, ref currentVel, currentSmoothTime, Mathf.Infinity, Time.deltaTime);
        //currentAngle = Mathf.Lerp(currentAngle, wantedAngle, Time.deltaTime * (spinSpeed) * (float)pointSkip);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, facingToAngle(currentFacing)), Time.deltaTime * spinSpeed * (float)pointSkip);
        
        transform.rotation = Quaternion.Euler(0f, 0f, -currentAngle);

        rotNow = /*wantedAngle;*/currentAngle;
        rotThisFrame = (Mathf.Abs(rotNow - prevRot)/360f);
        rotPerMin = (rotThisFrame / Time.deltaTime) * 60f;

        speedSamples.Enqueue(rotPerMin);

        prevRot = rotNow;


        //textRPS.text = "RotPM " + rotPerSec * 60f;
        aveRPM = Mathf.Lerp(aveRPM, speedSamples.Average(), Time.deltaTime * 10f);
		textRPS.text = "Ave. RPM: " + (aveRPM.ToString("F1"));

        if (aveRPM > bestRotPerMin)
        {
            bestRotPerMin = aveRPM;
			syncBestText();
        }
	}

    private void syncBestText()
    {
        textBestRPS.text = "Best RPM: " + (bestRotPerMin.ToString("F1"));
	}

    private void syncCompassPointTexts()
    {
        for (int p = 0; p < 16; p++)
        {
            if (p % pointSkip == 0 /*&& p != (int)currentFacing*/)
            {
				//textCompassPoints[p].alpha = (gameRunning && p == (int)currentFacing) ? 1 : .2f;
				textCompassPoints[p].color = (gameRunning && p == (int)currentFacing) ? Color.black : Color.white;
				textCompassPoints[p].alpha = (gameRunning && (p == (int)currentFacing || isNextFacing((Facing)p))) ? 1 : .1f;
			}
            else
            {
				textCompassPoints[p].color = Color.white;
				textCompassPoints[p].alpha = .005f;
			}
        }
    }

    private float facingToAngle(Facing facing)
    {
        return ((float)facing / 16f) * 360f;
    }

    private Facing getNextFacing(int way)
    {
        int newFacInt = (int)currentFacing + way * pointSkip;
        if (newFacInt < 0) newFacInt += 16;
        else if (newFacInt >= 16) newFacInt -= 16;

        return (Facing)newFacInt;
    }
}
