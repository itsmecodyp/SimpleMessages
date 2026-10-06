using BepInEx;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleMessages
{
	public partial class Messages : BaseUnityPlugin
	{
		public void UIStuff()
		{
			if (showSubMod)
			{
				if (GUI.Button(new Rect(Screen.width - 120, 140, 95, 20), "Messages"))
					showMenu = !showMenu;//k

				if (GUI.Button(new Rect(Screen.width - 25, 140, 25, 20), "x"))
					showSubMod = false;
			}
			float scale = GetScaleFactor();
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            if (showMenu)
			{
				menuRect = GUILayout.Window(9675309, menuRect, MainMessageWindow, "Messages", GUILayout.MaxWidth(200f), GUILayout.MinWidth(200f));

				if (showHideStartersWindow)
				{
					starterMenuRect = GUILayout.Window(9675310, starterMenuRect, StartersWindow, "Starters", GUILayout.MinWidth(225f));
					starterMenuRect.position = new Vector2(menuRect.x - starterMenuRect.width - 10, menuRect.y);

					if (isEditingStarter)
					{
						editStarterRect = GUILayout.Window(9675312, editStarterRect, EditStarterWindow, "Edit Starter", GUILayout.MinWidth(200f));
						editStarterRect.position = new Vector2(starterMenuRect.x - editStarterRect.width - 10, starterMenuRect.y);
					}
				}

				if (showHideRepliesWindow)
				{
                    repliesMenuRect = GUILayout.Window(
                        9675311,
                        repliesMenuRect,
                        RepliesWindow,
                        "Replies",
                        GUILayout.MinWidth(225f)
                    );
                    repliesMenuRect.x = menuRect.x + menuRect.width + 10;
                    repliesMenuRect.y = menuRect.y;
                    if (isEditingResponse)
					{
						editResponseRect = GUILayout.Window(9675313, editResponseRect, EditResponseWindow, "Edit Response");
						editResponseRect.position = new Vector2(repliesMenuRect.x + repliesMenuRect.width + 10, repliesMenuRect.y);
					}
				}
			}
            GUI.matrix = Matrix4x4.identity;

        }

        private void MainMessageWindow(int windowID)
		{
			SetGuiWindowInUse(windowID);
			Color or = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (isEnabled == true)
            {
                GUI.backgroundColor = Color.green;
            }
			else
			{
                GUI.backgroundColor = Color.red;
            }
            if (GUILayout.Button("Enable messages"))
                isEnabled = !isEnabled;
			GUI.backgroundColor = or;
            if (GUILayout.Button("Show Starters"))
				showHideStartersWindow = !showHideStartersWindow;

			if (GUILayout.Button("Show Replies"))
			{
                //repliesMenuRect.width = 280f;
                //repliesMenuRect.height = 400f;
                showHideRepliesWindow = !showHideRepliesWindow;
            }

            if (GUILayout.Button("Add New Starter"))
				AddNewStarter();

			if (GUILayout.Button("Add New Response"))
				AddNewResponse();
			if(GUILayout.Button("Save to file"))
			{
				SaveCurrent();
			}
			GUI.DragWindow();
		}

		private void StartersWindow(int windowID)
		{
			SetGuiWindowInUse(windowID);
			Color or = GUI.backgroundColor;
            // Leave a bit of room for the title bar/borders
            float scrollHeight = starterMenuRect.height - 30f;
            scrollPosStarts = GUILayout.BeginScrollView(
             scrollPosStarts,
             GUILayout.Height(scrollHeight)
         );

            foreach (var starter in actorStartPhrases)
			{
				GUILayout.BeginHorizontal();
				if (editingResponseKey == starter.Key)
				{
					GUI.backgroundColor = Color.green;
				}
				else
				{
					GUI.backgroundColor = or;
				}
				if (GUILayout.Button(starter.Key))
					StartEditingStarter(starter.Key);

				if (GUILayout.Button("Delete", GUILayout.Width(60)))
					actorStartPhrases.Remove(starter.Key);

				GUILayout.EndHorizontal();
			}
            GUILayout.EndScrollView();

            GUI.DragWindow();
		}
		public Vector2 scrollPosStarts;
        public Vector2 scrollPosReplies;

        private void RepliesWindow(int windowID)
        {
            SetGuiWindowInUse(windowID);

            float maxWidth = 0f;

            foreach (var response in actorResponses)
            {
                float width = GUI.skin.button.CalcSize(new GUIContent(response.Key)).x;
                maxWidth = Mathf.Max(maxWidth, width);
            }

            // Add room for delete button + spacing + borders
            maxWidth += 100f;

            float scrollHeight = repliesMenuRect.height - 30f;

            scrollPosReplies = GUILayout.BeginScrollView(
                scrollPosReplies,
                GUILayout.Width(maxWidth),
                GUILayout.Height(scrollHeight)
            );

            foreach (var response in actorResponses)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(response.Key, GUILayout.ExpandWidth(true)))
                    StartEditingResponse(response.Key);

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    actorResponses.Remove(response.Key);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
        string newName = "";
		private void EditStarterWindow(int windowID)
		{
			SetGuiWindowInUse(windowID);
			GUILayout.Label($"Editing Starter: {editingStarterKey}");
			newName = GUILayout.TextField(newName); // Updated value
			if (GUILayout.Button("Rename"))
				RenameStarter(newName);

			// Edit actor replies
			GUILayout.Label("Actor phrases:");
			for (int i = 0; i < editingStarter.starterPhrases.Count; i++)
			{
				GUILayout.BeginHorizontal();
				editingStarter.starterPhrases[i] = GUILayout.TextField(editingStarter.starterPhrases[i]);

				if (GUILayout.Button("Remove", GUILayout.Width(60)))
					editingStarter.starterPhrases.RemoveAt(i);

				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Add New"))
				editingStarter.starterPhrases.Add(string.Empty);

			GUI.DragWindow();
		}

		string newName2 = "";
		private int selectedActionIndex = 0;
		private List<string> actionNames = SimpleMessages.Messages.AvailableActions.Keys.ToList();

		private void EditResponseWindow(int windowID)
		{
			SetGuiWindowInUse(windowID);
			GUILayout.Label($"Editing Response: {editingResponseKey}");

			// Rename response
			newName2 = GUILayout.TextField(newName2);
			if (GUILayout.Button("Rename"))
				RenameResponse(newName2);

			// Actor Assets
			GUILayout.Label("Actor types that respond:");
			for (int i = 0; i < editingResponse.actorAssetID.Count; i++)
			{
				GUILayout.BeginHorizontal();
				editingResponse.actorAssetID[i] = GUILayout.TextField(editingResponse.actorAssetID[i]);

				if (GUILayout.Button("Remove", GUILayout.Width(60)))
					editingResponse.actorAssetID.RemoveAt(i);

				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Add New Actor Asset"))
				editingResponse.actorAssetID.Add(string.Empty);

			// Actor Replies
			GUILayout.Label("Valid responses:");
			for (int i = 0; i < editingResponse.actorReply.Count; i++)
			{
				GUILayout.BeginHorizontal();
				editingResponse.actorReply[i] = GUILayout.TextField(editingResponse.actorReply[i]);

				if (GUILayout.Button("Remove", GUILayout.Width(60)))
					editingResponse.actorReply.RemoveAt(i);

				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Add New Reply"))
				editingResponse.actorReply.Add(string.Empty);

			// Actor Replies
			GUILayout.Label("Response action:");
			for (int i = 0; i < actionNames.Count; i++)
			{
				// Highlight the background color if this action is the current one
				if (SimpleMessages.Messages.AvailableActions[actionNames[i]] == editingResponse.responseAction)
					GUI.backgroundColor = Color.green;
				else
					GUI.backgroundColor = Color.white;

				// Render the button
				if (GUILayout.Button(actionNames[i]))
				{
					selectedActionIndex = i;
					editingResponse.responseAction = SimpleMessages.Messages.AvailableActions[actionNames[selectedActionIndex]];
				}
			}

			GUI.DragWindow();
		}

		public static string newKey = "New Starter";
		public static int n = 1;
		private void AddNewStarter()
		{
			string key = newKey + n.ToString();
			actorStartPhrases.Add(key, new StarterData(new List<string>{ "Stuff" , "to say"}));
			n++;
		}

		private void AddNewResponse()
		{
			string newKey = "New Response";
			actorResponses[newKey] = new ResponseData(newKey, new List<string>(), new List<string>(), null);
		}

		private void RenameStarter(string newName)
		{
			if(newName != editingStarterKey)
			{
				actorStartPhrases.Add(newName, editingStarter);
				editingStarter = actorStartPhrases[newName];
				actorStartPhrases.Remove(editingStarterKey);
				editingStarterKey = newName;
			}
		}

		private void RenameResponse(string newName)
		{
			if (!string.IsNullOrEmpty(newName) && newName != editingResponseKey)
			{
				if (!actorResponses.ContainsKey(newName))
				{
					var data = actorResponses[editingResponseKey];
					actorResponses.Remove(editingResponseKey);
					actorResponses[newName] = data;
					editingResponseKey = newName;
					editingResponse.inputToRespondTo = newName;
				}
				else
				{
					Debug.LogWarning("The new name already exists in the dictionary.");
				}
			}
			else
			{
				Debug.LogWarning("Invalid or empty name.");
			}
		}

		private void StartEditingStarter(string key)
		{
			editingStarterKey = key;
			editingStarter = actorStartPhrases[key];
			isEditingStarter = true;
		}

		private void StartEditingResponse(string key)
		{
			editingResponseKey = key;
			editingResponse = actorResponses[key];
			isEditingResponse = true;
		}

		public void AddActorWindow(int windowID)
		{
			SetGuiWindowInUse(windowID);
			GUILayout.Label("Add New Actor", GUILayout.Width(200));

			newActorKey = GUILayout.TextField(newActorKey, GUILayout.Width(200));
			newPhrase = GUILayout.TextField(newPhrase, GUILayout.Width(200));

			if (GUILayout.Button("Add", GUILayout.Width(200)))
			{
				if (!string.IsNullOrWhiteSpace(newActorKey) && !actorStartPhrases.ContainsKey(newActorKey))
				{
					actorStartPhrases[newActorKey] = new StarterData(new List<string> { newPhrase });
					isAddingNewActor = false;
					newActorKey = "";
					newPhrase = "";
				}
			}

			GUI.DragWindow();
		}

		public void AddResponseWindow(int windowID)
		{
			SetGuiWindowInUse(windowID);
			GUILayout.Label("Add New Response", GUILayout.Width(200));

			newPhrase = GUILayout.TextField(newPhrase, GUILayout.Width(200));
			actorID = GUILayout.TextField(actorID, GUILayout.Width(200));

			if (GUILayout.Button("Add", GUILayout.Width(200)))
			{
				if (!string.IsNullOrWhiteSpace(newPhrase) && !actorResponses.ContainsKey(newPhrase))
				{
					var newResponse = new ResponseData(newPhrase, new List<string> { actorID }, new List<string>(), null);
					actorResponses[newPhrase] = newResponse;
					isAddingNewResponse = false;
					newPhrase = "";
				}
			}

			GUI.DragWindow();
		}
	}
}
