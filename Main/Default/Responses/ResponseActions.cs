using BepInEx;
using UnityEngine;

namespace SimpleMessages
{
	public partial class Messages : BaseUnityPlugin
	{
		public static void startFight(Actor origin, Actor target)
		{
			bool didAttack = origin.tryToAttack(target);
			Debug.Log("attempting to attack from conversation ending: " + didAttack);
		}

		public static void otherTest(Actor origin, Actor target)
		{
			bool didAttack = origin.tryToAttack(target);
			Debug.Log("test");
		}
	}
}
