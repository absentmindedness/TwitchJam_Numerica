using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwitchChat;
using UnityEngine;

public class CommandQueue : MonoBehaviour
{
    [SerializeField] TMP_Text textQueueList;

	private Queue<Chatter> theCommandQueue = new Queue<Chatter>();

    void Start()
    {
        clearTheQueue();
    }

    public void clearTheQueue()
    {
        theCommandQueue.Clear();
		updateQueueList();

	}

	public void addToQueue(Chatter chat)
    {
        theCommandQueue.Enqueue(chat);
		updateQueueList();
	}

	public Chatter getChatterCommand()
    {
        if (theCommandQueue.Count > 0)
        {
            Chatter chat = theCommandQueue.Dequeue();
            updateQueueList();
            return chat;
        }
        else
            return null;
	}

	private void updateQueueList()
    {
        string txt = "";
        foreach (Chatter chat in theCommandQueue)
        {
            txt += $"<color={chat.tags.colorHex}>{chat.tags.displayName}</color> : {chat.message}\n";
		}

        textQueueList.text = txt;
    }
}
