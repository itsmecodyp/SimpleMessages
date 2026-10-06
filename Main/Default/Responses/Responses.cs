using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleMessages
{
	public partial class Messages : BaseUnityPlugin
	{
		public void CreateResponseDicts()
		{
			// New format
			ResponseData hello1 = new ResponseData(
				inputToRespondTo: "Hello.",
				actorAssetID: new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				actorReply: new List<string> { "How are you?", "Greetings.", "Lovely weather.", "Hello!", "Hi there!" },
				responseAction: null
			);

			ResponseData hello2 = new ResponseData(
				"Hello!",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "How are you?", "Greetings.", "Can I help you?" },
				null
			);

			ResponseData howAreYou = new ResponseData(
				"How are you?",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "I'm doing well.", "A bit ill..", "Alright and you?", "I'm great!" },
				null
			);

			ResponseData aBitIll = new ResponseData(
				"A bit ill..",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "Sorry to hear that..", "Stay away from me, please.", "Who'd you catch it from?" },
				null
			);

			ResponseData imDoingWell = new ResponseData(
				"I'm doing well.",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "That's good.", "Glad to hear.", "Aye me too.", "Nice." },
				null
			);

			ResponseData canIHelpYou = new ResponseData(
				"Can I help you?",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "If you wouldn't mind...", "No, thank you.", "I can help too!" },
				null
			);

			ResponseData ifYouWouldntMind = new ResponseData(
				"If you wouldn't mind..",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "Okay, what do you need?", "I can help, let's go.", "Lead the way!" },
				null
			);

			ResponseData noThankYou = new ResponseData(
				"No, thank you.",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "Okay.", "If you say so.", "Just ask if you need me!" },
				null
			);

			ResponseData no = new ResponseData(
				"No.",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "Okay.", "A little rude..", "I wasn't even asking you.", "Fine." },
				null
			);

			ResponseData wheresMyFriend = new ResponseData(
				"Where's my friend?",
				new List<string> { "unit_human", "unit_elf", "unit_dwarf" },
				new List<string> { "He's over there!", "I'm right here!", "Are you lost?", "I don't know." },
				null
			);

			ResponseData example1 = new ResponseData(
				"Howdy!",
				new List<string> { "unit_human" },
				new List<string> { "Hows it going?", "Hello.", "Wanna fight?" },
				null
			);

			ResponseData example2 = new ResponseData(
				"Wanna fight?",
				new List<string> { "unit_human" },
				new List<string> { "Bring it on!", "Let's go!" },
				startFight
			);

			ResponseData grrr2 = new ResponseData(
				"Grrr..",
				new List<string> { "unit_orc" },
				new List<string> { "What's the matter?", "Calm down.", "Need help with something?" },
				null
			);

			ResponseData goAway = new ResponseData(
				"Go away.",
				new List<string> { "unit_orc" },
				new List<string> { "Fine.", "I was just leaving.", "No need to be rude." },
				null
			);

			ResponseData buBock = new ResponseData(
				"bu-bock",
				new List<string> { "chicken", "rooster" },
				new List<string> { "bock bock!", "Cluck cluck." },
				null
			);

			ResponseData bock = new ResponseData(
				"bock!",
				new List<string> { "chicken", "rooster" },
				new List<string> { "Cluck!", "bu-bock!" },
				null
			);

			ResponseData greetings = new ResponseData(
				"Greetings.",
				new List<string> { "unit_elf", "unit_human" },
				new List<string> { "Hello.", "Greetings to you too.", "How are you?" },
				null
			);

			ResponseData oy = new ResponseData(
				"Oy!",
				new List<string> { "unit_dwarf" },
				new List<string> { "What?", "Yes?", "What do you want?" },
				null
			);

			ResponseData sshhh = new ResponseData(
				"Sshhh...",
				new List<string> { "unit_elf", "fairy", "enchanted_fairy" },
				new List<string> { "Quiet!", "What's going on?", "Did you hear that?" },
				null
			);

			ResponseData rah = new ResponseData(
				"RAH!",
				new List<string> { "dragon", "unit_orc" },
				new List<string> { "RAH!", "Scared?", "Want to fight?" },
				null
			);

			ResponseData ba = new ResponseData(
				"Ba.",
				new List<string> { "sheep" },
				new List<string> { "Baaaahh!", "What do you want?", "Meee?" },
				null
			);

			ResponseData moo = new ResponseData(
				"Mooo",
				new List<string> { "cow" },
				new List<string> { "Mooooove!", "What do you want?", "....." },
				null
			);

			ResponseData wannaPlay = new ResponseData(
				"Want to play?",
				new List<string> { "baby_human", "baby_elf", "baby_orc", "baby_dwarf" },
				new List<string> { "Sure!", "Let's play!", "What game?", "Okay!" },
				null
			);

			ResponseData lookOverThere = new ResponseData(
				"Look over there!",
				new List<string> { "baby_human", "baby_elf", "baby_orc", "baby_dwarf" },
				new List<string> { "Where?", "What is it?", "I see it!", "Cool!" },
				null
			);

			ResponseData tired = new ResponseData(
				"I'm tired.",
				new List<string> { "unit_human" },
				new List<string> { "Take a rest.", "Sleep well.", "You should relax." },
				null
			);

			ResponseData smellsLikeGrease = new ResponseData(
				"Do I smell grease?",
				new List<string> { "chicken", "rooster" },
				new List<string> { "Uh-oh...", "Not good.", "Run away!" },
				null
			);

			ResponseData imAVegan = new ResponseData(
				"I'm a vegan.",
				new List<string> { "unit_elf" },
				new List<string> { "Good for you.", "Interesting.", "Me too." },
				null
			);

			ResponseData myHeadHurts = new ResponseData(
				"Ooww, my head..",
				new List<string> { "unit_dwarf" },
				new List<string> { "Rough night?", "Take it easy.", "Need some water?" },
				null
			);
		}

		public static Dictionary<string, ResponseAction> AvailableActions = new Dictionary<string, ResponseAction>
		{
			{ "Default Action", otherTest },
			{ "Start Fight", startFight }
		};

		//key: inputToRespondTo, value: response data
		public static Dictionary<string, ResponseData> actorResponses = new Dictionary<string, ResponseData>();

		[Serializable]
		public delegate void ResponseAction(Actor pActor = null, Actor pTarget = null);


		[Serializable]
		public class ResponseData
		{
			//input to start these responses, cannot be null
			//maybe this should be key in dict and actor id is separate?
			public string inputToRespondTo;

			//list of actor ids that can respond with this
			public List<string> actorAssetID;

			//key: statsID, value: list/array of replies
			//if statsID is not in here, actor type will not verbally respond to input
			public List<string> actorReply = new List<string>();

			public string actionName; // New field to store the name of the action

			//possible action to execute with certain inputs
			//thems fightin words
			[NonSerialized]
			public ResponseAction responseAction;

			public ResponseData(string inputToRespondTo, List<string> actorAssetID, List<string> actorReply, ResponseAction responseAction)
			{
				this.inputToRespondTo = inputToRespondTo;
				this.actorAssetID = actorAssetID;
				this.actorReply = actorReply;
				this.responseAction = responseAction;
				if(responseAction != null)
				this.actionName = SimpleMessages.Messages.AvailableActions.FirstOrDefault(pair => pair.Value == responseAction).Key;

				if (string.IsNullOrEmpty(inputToRespondTo) == false && SimpleMessages.Messages.actorResponses.ContainsKey(inputToRespondTo) == false)
				{
					SimpleMessages.Messages.actorResponses.Add(inputToRespondTo, this);
				}
			}
		}
	}
}
