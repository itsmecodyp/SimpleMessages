using BepInEx;
using System;
using System.Collections.Generic;

namespace SimpleMessages
{
	public partial class Messages : BaseUnityPlugin
	{
		public void CreateStartPhrases()
		{
			//s
			actorStartPhrases.Add("human", new StarterData(new List<string> { "Hello.", "What do you want?", "Can I help you?" }));
			actorStartPhrases.Add("chicken", new StarterData(new List<string> { "bu-bock", "bock!", "Where's my egg?", "Do I smell grease?", "Eat more beef" }));
			actorStartPhrases.Add("rooster", new StarterData(new List<string> { "bu-bock", "bock!", "Where's my egg?", "Do I smell grease?", "Eat more beef" }));
			actorStartPhrases.Add("elf", new StarterData(new List<string> { "Greetings.", "Sshhh...", "I'm a vegan." }));
			actorStartPhrases.Add("dwarf", new StarterData(new List<string> { "Oy!", "Get back to work.", "Where did I leave my cup?", "Ooww, my head..", "You're big!" }));
			actorStartPhrases.Add("orc", new StarterData(new List<string> { "Grrr..", "Go away.", "Want to fight?", "I'll break your bones.", "You are small.", "Meat... *drools*" }));
			actorStartPhrases.Add("baby_human", new StarterData(new List<string> { "That tickles!", "Want to play?", "Look over there!", "Where's my friend?", "I'm a big kid!" }));
			actorStartPhrases.Add("baby_elf", new StarterData(new List<string> { "That tickles!", "Want to play?", "Look over there!", "Where's my friend?", "I'm a big kid!" }));
			actorStartPhrases.Add("baby_orc", new StarterData(new List<string> { "That tickles!", "Want to play?", "Look over there!", "Where's my friend?", "I'm a big kid!" }));
			actorStartPhrases.Add("baby_dwarf", new StarterData(new List<string> { "That tickles!", "Want to play?", "Look over there!", "Where's my friend?", "I'm a big kid!" }));
			actorStartPhrases.Add("dragon", new StarterData(new List<string> { "Grrr...", "I'm tired.", "Where's my treasure?", "BURN!", "So small!", "RAH!" }));
			actorStartPhrases.Add("sheep", new StarterData(new List<string> { "Ba.", "Baaaahh!", "Shh.. They can't know.", ".....", "Buuuhh.", "What are you doing?" }));
			actorStartPhrases.Add("cow", new StarterData(new List<string> { "Mooo", "Mooooove!", "Shh.. They can't know.", ".....", "*Chews grass*" }));
			actorStartPhrases.Add("penguin", new StarterData(new List<string> { "It might be a spy.", "What sort of noises does a penguin make?", "How did I get here?", "My feet don't have feelings..." }));
			actorStartPhrases.Add("turtle", new StarterData(new List<string> { "Is that a bit of moss?", "How old am I?", "Where is my egg?", ".....", "Wow you're fast..." }));
			actorStartPhrases.Add("river_turtle", new StarterData(new List<string> { "Is that a bit of moss?", "How old am I?", "Where is my egg?", ".....", "Wow you're fast..." }));
			actorStartPhrases.Add("crab", new StarterData(new List<string> { "Click click.", "Let's dance!", "There's a rave going down later...", ".....", "Where's the big one?" }));
			actorStartPhrases.Add("fairy", new StarterData(new List<string> { "Watch out for lightning..", "Do you hear them?", "Shh.. They're coming", ".....", "*Giggles*" }));
			actorStartPhrases.Add("enchanted_fairy", new StarterData(new List<string> { "Watch out for lightning..", "Do you hear them?", "Shh.. They're coming", ".....", "*Giggles*" }));
		}

		//key: statsID, value: list/array of phrases to start conversations
		public static Dictionary<string, StarterData> actorStartPhrases = new Dictionary<string, StarterData>();

		[Serializable]
		public class StarterData
		{
			//actor id is assigned by dict already
			public List<string> starterPhrases;

			public StarterData(List<string> starterPhrases)
			{
				this.starterPhrases = starterPhrases;
			}
		}
	}
}
