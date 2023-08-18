using System.Collections;
using System.Collections.Generic;
using TwitchChat;
using UnityEngine;

public class FrogCode : MonoBehaviour
{
    [SerializeField] CommandQueue myCommandQueue;
    [SerializeField] float delayGetCommand = 1f;

	private Vector2 wantedPos;
    private float timeToNextCommand;

    void Start()
    {
        wantedPos = new Vector2(0f, -8f);
        timeToNextCommand = delayGetCommand;
    }

    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, wantedPos, Time.deltaTime * 4f);

        timeToNextCommand -= Time.deltaTime;
        if (timeToNextCommand <= 0)
        {
            timeToNextCommand = delayGetCommand;
            Chatter chatCom = myCommandQueue.getChatterCommand();
            if (chatCom != null)
            {
				switch (chatCom.message)
				{
					case "hop":
						hop(0);
						break;
					case "hopl":
						hop(-1);
						break;
					case "hopr":
						hop(1);
						break;

				}
			}
        }
    }

    public void hop(int sideways)
    {
        wantedPos.y += 1;
        wantedPos.x += sideways;
    }
 }
