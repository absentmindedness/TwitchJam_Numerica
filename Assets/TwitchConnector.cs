using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchChat;
using UnityEngine.Events;
using TMPro;

public class TwitchConnector : MonoBehaviour
{
	[SerializeField] TMP_Text textTitle, textLastPlayer;

	[SerializeField] GameObject loginThing, gameOptionsThing;

    public string channelName = "";
	public string lastPlayerName = "";

	void Start()
	{
		//TwitchController.Login("");
		//TwitchController.onTwitchMessageReceived += OnMessageReceived;
		TwitchController.onChannelJoined += OnChannelJoined;

		loginThing.SetActive(true);
		gameOptionsThing.SetActive(false);

		setLastPlayerName("");
	}

	public void uiLoginToTwitch(TMP_Text textChannelName)
	{
		Debug.Log("Logging in to channel: " + textChannelName.text);
		channelName = textChannelName.text;

		loginThing.SetActive(false);

		TwitchController.Login("autopsyturvey"/*channelName*/);
		//TwitchController.onTwitchMessageReceived += OnMessageReceived;
	}

	private void OnDestroy()
	{
		TwitchController.onTwitchMessageReceived -= OnMessageReceived;
	}

	private void OnChannelJoined()
	{
		Debug.Log("JOINED CHANNEL");

		if (channelName == "") return;

		loginThing.SetActive(false);
		gameOptionsThing.SetActive(true);
		//FindObjectOfType<BottleCode>().startGame();

		TwitchController.onTwitchMessageReceived += OnMessageReceived;
	}

	private void setLastPlayerName(string name)
	{
		lastPlayerName = name;
		syncLastPlayerName();
	}

	private void syncLastPlayerName()
	{
		textLastPlayer.text = lastPlayerName == "" ? "" : "Last: " +lastPlayerName;
	}

	private void OnMessageReceived(Chatter chatter)
	{
		Debug.Log($"Message received from <color={chatter.tags.colorHex}>{chatter.tags.displayName}</color> : {chatter.message}");

		BottleCode bottleCode = FindAnyObjectByType<BottleCode>();

		for (int p = 0; p < 16; p++)
		{
			Facing checkFacing = (Facing)p;
			if (chatter.message.ToUpper() == checkFacing.ToString())
			{
				if (bottleCode.isNextFacing(checkFacing) && chatter.tags.displayName != lastPlayerName)
				{
					bottleCode.increaseFacing();
				}
				else
				{
					textTitle.text = "<size=50%>"+chatter.tags.displayName + "\n\n</size>BOTTLED\nIT!";
					bottleCode.resetForNewTurn();
				}

				setLastPlayerName(chatter.tags.displayName);
			}
		}

	}

	void Update()
    {
        
    }
}
