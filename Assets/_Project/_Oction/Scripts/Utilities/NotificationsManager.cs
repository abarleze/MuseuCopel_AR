//EVENTS MANAGER CLASS - for receiving notifications and notifying listeners
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationsManager : MonoBehaviour
{
	//Internal reference to all listeners for notifications
	private Dictionary<string, List<Component>> Listeners = new Dictionary<string, List<Component>>();
	
	//Public property
	public static NotificationsManager Instance
	{
		get
		{
			if(instance == null) instance = new NotificationsManager();
			
			return instance;
		}
	}
	
	//Internal reference
	private static NotificationsManager instance = null;
	
	/// <summary>
	/// Function to add a listener for an notification to the listeners list
	/// </summary>
	/// <param name='Sender'>
	/// Sender.
	/// </param>
	/// <param name='NotificationName'>
	/// Notification name.
	/// </param>
	public void AddListener(Component Sender, string NotificationName)
	{
		//Add listener to dictionary
		if(!Listeners.ContainsKey(NotificationName))
			Listeners.Add (NotificationName, new List<Component>());
		
		//Add object to listener list for this notification
		Listeners[NotificationName].Add(Sender);
	}
	
	/// <summary>
	/// Function to remove a listener for a notification
	/// </summary>
	/// <param name='Sender'>
	/// Sender.
	/// </param>
	/// <param name='NotificationName'>
	/// Notification name.
	/// </param>
	public void RemoveListener(Component Sender, string NotificationName)
	{
		//If no key in dictionary exists, then exit
		if(!Listeners.ContainsKey(NotificationName))
			return;
		
		//Cycle through listeners and identify component, and then remove
		for(int i = Listeners[NotificationName].Count-1; i>=0; i--) 
		{
			//Check instance ID
			if(Listeners[NotificationName][i].GetInstanceID() == Sender.GetInstanceID())
				Listeners[NotificationName].RemoveAt(i); //Matched. Remove from list
		}
	}
	
	/// <summary>
	/// Function to post a notification to a listener
	/// </summary>
	/// <param name='Sender'>
	/// Sender.
	/// </param>
	/// <param name='NotificationName'>
	/// Notification name.
	/// </param>
	public void PostNotification(Component Sender, string NotificationName)
	{		
		//If no key in dictionary exists, then exit
		if(!Listeners.ContainsKey(NotificationName))
			return;

		//Else post notification to all matching listeners
		foreach(Component Listener in Listeners[NotificationName])
			Listener.SendMessage(NotificationName, Sender, SendMessageOptions.DontRequireReceiver);
	}
	
	/// <summary>
	/// Function to clear all listeners
	/// </summary>
	public void ClearListeners()
	{
		//Removes all listeners
		Listeners.Clear();
	}
	
	/// <summary>
	/// Function to remove redundant listeners - deleted and removed listeners
	/// </summary>
	public void RemoveRedundancies()
	{
		//Create new dictionary
		Dictionary<string, List<Component>> TmpListeners = new Dictionary<string, List<Component>>();
			
		//Cycle through all dictionary entries
		foreach(KeyValuePair<string, List<Component>> Item in Listeners)
		{
			//Cycle through all listener objects in list, remove null objects
			for(int i = Item.Value.Count-1; i>=0; i--)
			{
				//If null, then remove item
				if(Item.Value[i] == null)
					Item.Value.RemoveAt(i);
			}
			
			//If items remain in list for this notification, then add this to tmp dictionary
			if(Item.Value.Count > 0)
				TmpListeners.Add (Item.Key, Item.Value);
		}
		
		//Replace listeners object with new, optimized dictionary
		Listeners = TmpListeners;
	}
	
	/// <summary>
	/// Called when a new level is loaded; remove redundant entries from dictionary; in case left-over from previous scene
	/// </summary>
	void OnLevelWasLoaded()
	{
		//Clear redundancies
		RemoveRedundancies();
	}
	
	void Awake()
	{
		//Check if there is an existing instance of this object
		if(instance)
			DestroyImmediate(gameObject); //Delete duplicate
		else
		{
			instance = this; //Make this object the only instance
			DontDestroyOnLoad (gameObject); //Set as do not destroy
		}
	}
}

//Example: create an enemy class with:
//void Start()
//{
//	NotificationsManager.Instance.AddListener(this, "Shot");
//}
//void Shot(Component Sender)
//{
//	Destroy (gameObject);
//}

//Example: create a target class with:
//void OnMouseDown()
//{
//	NotificationsManager.Instance.PostNotification(this, "Shot");
//}