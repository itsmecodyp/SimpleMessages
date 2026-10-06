using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SimpleMessages
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public partial class Messages : BaseUnityPlugin {
        public static HarmonyLib.Harmony harmony;

        public void Awake()
        {
            Debug.Log("Messages awake");
            self = this;
            mainColorHex = Config.Bind("Style", "MainColorHex", "808080", "Main background color of the message (hex string, e.g. 808080 or FF0000)");
            borderColorHex = Config.Bind("Style", "BorderColorHex", "FFFFFF", "Content/font color of the message (hex string)");
            fontSize = Config.Bind("Style", "FontSize", 15, "Font size of the message");
            CreateStartPhrases();
			LoadStartersFromJson();
            CreateResponseDicts();
			SaveCurrent();
            Debug.Log("Messages awake end");
        }
        public void OnDestroy()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public static bool enable = true;
        public static bool isEnabled;

		public void OnGUI()
		{
			UIStuff();
			if (isEnabled)
			{
                if (listOfMessages.Count >= 1)
                {
                    for (int i = 0; i < listOfMessages.Count; i++)
                    {
                        ModMessage activeMessage = listOfMessages[i];
                        if (activeMessage.startTime + activeMessage.duration > Time.realtimeSinceStartup)
                        {
                            Actor actor = activeMessage.assignedActor;
                            ActorData data = null;
                            if (actor != null)
                                data = actor.data;
                            if (data != null && actor.isAlive())
                            {
                                Vector2 textDimensions = GUI.skin.window.CalcSize(new GUIContent(activeMessage.MessageText));
                                if (Camera.main != null)
                                {
                                    Vector3 position = Camera.main.WorldToScreenPoint(actor.cur_transform_position);
                                    // adding a random number (3536) to make sure theres no conflict with window id in other mods
                                    Rect window = GUILayout.Window(activeMessage.id + 3536,
                                        new Rect(position.x - (textDimensions.x / 2), Screen.height - position.y - (textDimensions.y * 2), textDimensions.x, textDimensions.y),
                                        ActorMessageDisplayWindow,
                                        "",
                                        new GUIStyle());
                                }
                            }
                        }
                    }
                }
            }
		}

        public static bool HasActorStartPhrase(string id)
        {
            return actorStartPhrases.ContainsKey(id);
        }

        public static string GetRandomActorStartPhrase(string id)
        {
            if (!actorStartPhrases.TryGetValue(id, out var phrases))
                return null;

            return phrases.starterPhrases.GetRandom();
        }

        public static float GetScaleFactor()
		{
			Type guiMainType = Type.GetType("SimplerGUI.GuiMain, SimplerGUI");
			if (guiMainType != null)
			{
				FieldInfo field = guiMainType.GetField("scaleFactor", BindingFlags.Public | BindingFlags.Static);
				if (field != null)
				{
					return (float)field.GetValue(null);
				}
			}
			return 1f;
		}

		public static void SetGuiWindowInUse(int windowID)
		{
			Type guiMainType = Type.GetType("SimplerGUI.GuiMain, SimplerGUI");
			if (guiMainType != null)
			{
				MethodInfo method = guiMainType.GetMethod("SetWindowInUse", BindingFlags.Public | BindingFlags.Static);
				if (method != null)
				{
					method.Invoke(null, new object[] { windowID });
				}
			}
		}

		public static bool UseDebugHotkeys()
		{
			Type guiMainType = Type.GetType("SimplerGUI.GuiMain, SimplerGUI");
			if (guiMainType != null)
			{
				FieldInfo field = guiMainType.GetField("useDebugHotkeys", BindingFlags.Public | BindingFlags.Static);
				if (field != null)
				{
					return (bool)field.GetValue(null);
				}
			}
			return false;
		}

		public void Update()
		{
			repliesMenuRect.height = 225f;
			editResponseRect.height = 0f;
			starterMenuRect.height = 225f;
			editStarterRect.height = 0f;
			if (enable)
			{
				/*
				if (UseDebugHotkeys() && Input.GetKeyDown(KeyCode.M))
				{
					Actor potentialStarter = UnitClipboard_Main.ClosestActorToTile(MapBox.instance.getMouseTilePos(), 5f);
					if (potentialStarter != null)
					{
						string id = potentialStarter.asset.id;
						Console.WriteLine("actor found with id:" + id);
						if (Messages.actorStartPhrases.ContainsKey(id))
						{
							string phraseToSay = Messages.actorStartPhrases[id].starterPhrases.GetRandom();
							Messages.ActorSay(potentialStarter, phraseToSay, 3);
						}
						else
						{
							Console.WriteLine("No conversation starter found for actor ID");
						}
					}
					else
					{
						Console.WriteLine("No actor found near cursor");
					}
				}
				*/
				if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
				{
					showSubMod = true;
				}
				Messages.ConversationStart();
				Messages.ReplyUpdate();
			}
		}

		public static void ActorSay(Actor targetActor, string messageText, float duration = 3f)
        {
            if(Messages.isEnabled == false)
            {
                return;
            }
			if(actorsOnCooldown.Contains(targetActor)) {
                return;
			}
            ModMessage newMessage = new ModMessage
            {
                id = messageID // id starts at 0, random start addition to make sure no conflict with other mod menu ids
            };
            messageID++; // and increments upwards each time
            newMessage.assignedActor = targetActor;
            newMessage.duration = duration;
            newMessage.startTime = Time.realtimeSinceStartup;
            newMessage.MessageText = messageText;
            listOfMessages.Add(newMessage);
			//Debug.Log("reply start");
			timeAtLastMessage = Time.realtimeSinceStartup; // track time for auto reply

            replyActor = null; // reset to be sure
            replyString = null;

            //potential replier to the message being said right now
			//this was null until we find closest visible unit, but thats also turning up null?? so grab random unit on island
            Actor potentialReplier = targetActor.current_island.actors.GetRandom();
            ListPool<Actor> visibleMinusOriginal = new ListPool<Actor>();

            if (potentialReplier == null)
            {
                potentialReplier = ClosestActorToTile(targetActor.current_tile, targetActor);
				if(potentialReplier == null)
                {
                    //old check, saving as backup in case new one fails
                    //Debug.Log("new potReply failed!");
					//maybe closest actor doesnt have reply, check visible units instead
					if (MapBox.instance.units.visible_units.array.Length > 2)
					{
						//make list of targets that dont include the person who just spoke
						foreach (Actor actor in MapBox.instance.units.visible_units.array)
						{
							if (actor != targetActor)
							{
								visibleMinusOriginal.Add(actor);
							}
						}
						if (visibleMinusOriginal.Count > 0)
						{
							Actor temp = visibleMinusOriginal.GetRandom();
							potentialReplier = temp;
						}
					}
				}
			}
            if(potentialReplier != null && !actorsOnCooldown.Contains(potentialReplier)) {
                if(actorResponses.TryGetValue(messageText, out ResponseData response)) {
                    //make sure response has assigned actor asset id
                    if(response.actorAssetID != null) {
                        //and potential replier matches that id
                        if(response.actorAssetID.Contains(potentialReplier.asset.id)){
                            reply = response;
                            replyActor = potentialReplier;
                            actorReplyActorIsRespondingTo = targetActor;
                            stringToReplyTo = messageText;
                           
							//Debug.Log("Replier assigned, moving to original speaker and canceling both AI");
                            replyActor.cancelAllBeh();
                            //replyActor.goTo(actorReplyActorIsRespondingTo.current_tile);
                            replyActor.makeWait(5f);
							actorReplyActorIsRespondingTo.cancelAllBeh();
							actorReplyActorIsRespondingTo.makeWait(5f);
                           
						}
						else
                        {
                            //Debug.Log("Replier actor asset ID does not match assigned for response");
                        }
                    }
                    //actorAssetId is null, use any actor
                    else {
                        //Debug.Log("Replier actor asset ID not in list or list is null, choosing any available");
                        reply = response;
                        replyActor = potentialReplier;
                        actorReplyActorIsRespondingTo = targetActor;
                        stringToReplyTo = messageText;
                    }
                }
                //if no replies, assign null
                else {
                    Debug.Log("No response found for:" + messageText);
                    reply = null;
                    replyActor = null;
                    actorReplyActorIsRespondingTo = null;
                    replyString = null;
                    stringToReplyTo = null;
                }
            }
            else {
                Debug.Log("No potential replier found");
            }
            //handle cooldown so actors dont say multiple things at once
            actorsOnCooldown.Add(targetActor);
            IEnumerator coroutine = WaitAndRemoveActorFromCooldown(duration, targetActor);
            self.StartCoroutine(coroutine);
        }

        public static IEnumerator WaitAndRemoveActorFromCooldown(float time, Actor target)
        {
            yield return new WaitForSeconds(time);
			if(actorsOnCooldown.Contains(target)) {
                actorsOnCooldown.Remove(target);
            }
        }

        //for refreshing changes to files mid-gameplay
        public static void Reload()
        {
            LoadStartersFromJson();
            LoadCustomsFromJson();
        }

		public static void LoadStartersFromJson()
		{
			string path1 = Application.streamingAssetsPath + "/messages";

			// Load starters
			string startersPath = path1 + "/starters";
			if (Directory.Exists(startersPath))
			{
				foreach (string f in Directory.GetFiles(startersPath))
				{
					string fileName = Path.GetFileNameWithoutExtension(f); // Safely get the filename without extension
					Debug.Log("Loading filename: " + fileName);

					string jsonData = File.ReadAllText(f);
					if (actorStartPhrases.ContainsKey(fileName))
					{
						actorStartPhrases[fileName] = JsonUtility.FromJson<StarterData>(jsonData);
					}
					else
					{
						actorStartPhrases.Add(fileName, JsonUtility.FromJson<StarterData>(jsonData));
					}
				}
			}

			// Load replies
			string repliesPath = path1 + "/replies";
			if (Directory.Exists(repliesPath))
			{
				foreach (string f in Directory.GetFiles(repliesPath))
				{
					string jsonData = File.ReadAllText(f);
					ResponseData response = JsonUtility.FromJson<ResponseData>(jsonData);

					// Assign the delegate based on actionName
					if (!string.IsNullOrEmpty(response.actionName) && SimpleMessages.Messages.AvailableActions.TryGetValue(response.actionName, out var action))
					{
						response.responseAction = action;
					}

					if (actorResponses.ContainsKey(response.inputToRespondTo))
					{
						actorResponses[response.inputToRespondTo] = response;
					}
					else
					{
						actorResponses.Add(response.inputToRespondTo, response);
					}
				}
			}

		}

		public static void LoadCustomsFromJson()
		{
			string path1 = Application.streamingAssetsPath + "/messages/custom/";

			// Load escortCommand if exists
			string escortCommandPath = path1 + "escortCommand.json";
			if (File.Exists(escortCommandPath))
			{
				string[] lines = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(escortCommandPath));
				escortCommandLines = lines;
			}
		}

		//export pre-made message assets so there is reference to create new ones
		public static void SaveCurrent()
		{
			string path1 = Application.streamingAssetsPath + "/messages";

			// Ensure directories exist
			EnsureDirectoryExists(path1);
			EnsureDirectoryExists(path1 + "/starters");
			EnsureDirectoryExists(path1 + "/replies");
			EnsureDirectoryExists(path1 + "/custom");

			// Save starter phrases
			string startersPath = path1 + "/starters/";
			foreach (KeyValuePair<string, StarterData> starterData in actorStartPhrases)
			{
				string path2 = Path.Combine(startersPath, SanitizeFileName(starterData.Key) + ".json");
				Debug.Log($"Saving starter data to: {path2}");
				Debug.Log($"Key used: {starterData.Key}");

				SaveDataToFile(path2, starterData.Value);
			}

			// Save replies
			string repliesPath = path1 + "/replies/";
			foreach (KeyValuePair<string, ResponseData> responseData in actorResponses)
			{

				// Clean up invalid chars in filename
				string path2 = repliesPath + SanitizeFileName(responseData.Key) + ".json";

				SaveDataToFile(path2, responseData.Value);
			}

			// Save custom messages
			string customPath = path1 + "/custom/";
			SaveCustomMessages(customPath);
		}

		private static void SaveDataToFile(string path, object data)
		{
			if(data is ResponseData responseData)
			{
				responseData.actionName = SimpleMessages.Messages.AvailableActions.FirstOrDefault(pair => pair.Value == responseData.responseAction).Key;
			}

			string json = JsonUtility.ToJson(data, true);
			if (File.Exists(path))
			{
				string existingJson = File.ReadAllText(path);
				if (existingJson != json)  // Only save if data has changed
				{
					File.WriteAllText(path, json);
				}
			}
			else
			{
				File.WriteAllText(path, json);
			}
		}

		private static void EnsureDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private static string SanitizeFileName(string fileName)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			return new string(fileName.Where(x => !invalidChars.Contains(x)).ToArray());
		}

		private static void SaveCustomMessages(string customPath)
		{
			// Save escortCommand.json if necessary
			SaveCustomFile(customPath + "escortCommand.json", escortCommandLines);
			SaveCustomFile(customPath + "escortEncouragement.json", escortEncourageLines);
			SaveCustomFile(customPath + "fear.json", fearLines);
			SaveCustomFile(customPath + "sorry.json", sorryLines);
		}

		private static void SaveCustomFile(string path, object data)
		{
			if (!File.Exists(path))
			{
				string json = JsonConvert.SerializeObject(data);
				File.WriteAllText(path, json);
			}
		}

		public static string RemoveInvalidChars(string filename)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                filename = filename.Replace(c.ToString(), "");
            }
            return filename;
        }

        private Color ParseHexColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.white;
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            if (hex.Length != 6) return Color.white;

            try
            {
                float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
                float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
                float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
                return new Color(r, g, b, 1f);
            }
            catch
            {
                return Color.white;
            }
        }

        public void ActorMessageDisplayWindow(int windowID)
        {
            ModMessage activeMessage = listOfMessages[windowID - 3536]; // removing the conflict number
            if(activeMessage != null) {
                Color originalBg = GUI.backgroundColor;
                Color originalContent = GUI.contentColor;

                GUI.backgroundColor = ParseHexColor(mainColorHex?.Value ?? "808080");
                GUI.contentColor = ParseHexColor(borderColorHex?.Value ?? "FFFFFF");

                GUIStyle newStyle = new GUIStyle(GUI.skin.box);
                newStyle.alignment = TextAnchor.MiddleCenter;
                newStyle.fontSize = fontSize?.Value ?? 12;

                GUILayout.Box(activeMessage.MessageText, newStyle);

                GUI.backgroundColor = originalBg;
                GUI.contentColor = originalContent;
            }
            GUI.DragWindow();
        }

		public static Actor ClosestActorToTile(WorldTile pTarget, Actor excluded = null)
		{
			HashSet<Actor> listToSearch = new HashSet<Actor>();
			if(MapBox.instance.units.visible_units.count > 1)
			{
                foreach (Actor actorToCheck in MapBox.instance.units.visible_units.array)
				{
					if(actorToCheck != excluded)
					{
						listToSearch.Add(actorToCheck);
					}
				}
            }

            Actor tBest = null;
            int tBestDist = int.MaxValue;
            Vector2Int tTargetPos = pTarget.pos;
            foreach (Actor tActor in listToSearch)
            {
                Vector2Int tTilePos = tActor.current_tile.pos;
                int tDist = (int)Toolbox.Dist(tTargetPos.x, tTargetPos.y, tTilePos.x, tTilePos.y);
                if (tDist < tBestDist)
                {
                    tBestDist = tDist;
                    tBest = tActor;
                }
            }
            return tBest;
        }

		public static void ReplyUpdate()
		{
			if (replyActor != null && reply != null)
			{
				if (timeAtLastMessage + 3.5f < Time.realtimeSinceStartup)
				{
					//if original message has replies saved
					if (reply.actorReply.Count > 0)
					{
						//if response can be responded to by this actor type
						if (reply.actorAssetID.Contains(replyActor.asset.id))
						{
							//response to say
							replyString = reply.actorReply.GetRandom();
							timeAtLastMessage = Time.realtimeSinceStartup; // auto reply after a time
						}
					}

					//if response has action to execute
					//target actor is origin speaker, replier is expected to be last initiator
					if (reply.responseAction != null)
					{
						//example: fighting, actor will execute attack against the person who was last speaking
						reply.responseAction(replyActor, actorReplyActorIsRespondingTo);
					}

					//if reply string was found before, say it now
					if (replyString != null)
					{
						ActorSay(replyActor, replyString);
					}
				}
			}
		}

		public static void ConversationStart()
		{
			//no talking for 10 seconds, initiate something
			if (timeAtLastMessage + timerToWait < Time.realtimeSinceStartup)
			{
				if (global::Config.game_loaded && MapBox.instance.units.visible_units.array.Length > 1)
				{
					Actor targetActor = MapBox.instance.units.visible_units.array.GetRandom();
					if(targetActor != null && targetActor.asset != null && targetActor.asset.id != null)//idk why the asset is null...
					{
                        if (actorStartPhrases.ContainsKey(targetActor.asset.id))
                        {
                            ActorSay(targetActor, actorStartPhrases[targetActor.asset.id].starterPhrases.GetRandom());
                            timerToWait = UnityEngine.Random.Range(7f, 10f);
                        }
                    }
					
				}
			}
		}
	}
}