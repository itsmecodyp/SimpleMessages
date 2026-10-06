using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleMessages
{
	public partial class Messages : BaseUnityPlugin
	{
		public static ConfigEntry<string> mainColorHex;
		public static ConfigEntry<string> borderColorHex;//mostly affects content for now, not border
		public static ConfigEntry<int> fontSize;
		public static bool isEditingStarter;
		public static bool isEditingResponse;
		public static string editingStarterKey;
		public static string editingResponseKey;
		public static StarterData editingStarter;
		public static ResponseData editingResponse;
		public static float timerToWait = 10f;
		public static bool showHideStartersWindow;
		public static bool showHideRepliesWindow;
		public static Rect starterMenuRect;
		public static Rect repliesMenuRect;
		// New variables for the editing windows
		public static Rect editStarterRect;
		public static Rect editResponseRect;

		public static bool showMenu;
		public static Rect menuRect;

		private string selectedActor;
		private string selectedResponse;
		private string newPhrase = "";
		private string newActorKey = "";
		private string actorID = "";

		private bool isEditingActor = false;
		private bool isAddingNewActor = false;
		private bool isAddingNewResponse = false;
		public const string pluginGuid = "cody.worldbox.simple.messages";
		public const string pluginName = "SimpleMessages";
		public const string pluginVersion = "0.0.0.7";
		public static int windowInUse = 0;
		public static List<ModMessage> listOfMessages = new List<ModMessage>();
		public static int messageID;
		public float lastTimer;
		public static Messages self;

		public static List<Actor> actorsOnCooldown = new List<Actor>();
		//change to list or dict of multiple conversations?
		public static Actor replyActor;
		public static Actor actorReplyActorIsRespondingTo;
		public static string replyString;
		public static string stringToReplyTo;
		public static ResponseData reply;

		public static float timeAtLastMessage;

		public bool showSubMod = true;

		public static string[] escortCommandLines = new string[] { "Get over here!", "Line up!", "Surrender or die...", "Where's my rope?" };
		public static string[] escortEncourageLines = new string[] { "Let's go!", "Move it!", "Faster!", "Pick your feet up!" };
		public static string[] fearLines = new string[] { "Noooo!", "I don't wanna die!", "WHY", "Wha-", "My kids..." };
		public static string[] sorryLines = new string[] { "...", "I'm sorry.." };

		[Serializable]
		public class ModMessage
		{
			public int id;
			public Actor assignedActor;
			public string TitleText = "";
			public string MessageText = "";
			public float duration = 3.0f;
			public float startTime;
		}
	}
}
