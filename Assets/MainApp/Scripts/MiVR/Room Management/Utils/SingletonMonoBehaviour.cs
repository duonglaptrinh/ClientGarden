using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : SingletonMonoBehaviour<MyClassName> {}
/// </summary>

namespace jp.co.mirabo.Application.RoomManagement
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		// Check to see if we're about to be destroyed.
		private static bool shuttingDown = false;
		private static object lockObject = new object();

		/// Custom this field in Inspector, default is true
		[SerializeField] public bool IsDontDestroyThisObject = true;

		private static T instance;

		/// 
		/// <summary>
		/// Access singleton instance through this propriety.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (shuttingDown)
				{
					DebugExtension.LogWarning("[Singleton] Instance '" + typeof(T) +
					                 "' already destroyed. Returning null.");
					return null;
				}

				lock (lockObject)
				{
					if (instance == null)
					{
						// Search for existing instance.
						instance = (T) FindObjectOfType(typeof(T));

						// Create new instance if one doesn't already exist.
						if (instance == null)
						{
							// Need to create a new GameObject to attach the singleton to.
							var singletonObject = new GameObject();
							instance = singletonObject.AddComponent<T>();
							singletonObject.name = "~[Singleton] " + typeof(T).Name.ToString();
						}
					}

					return instance;
				}
			}
		}

		protected void Awake()
		{
			if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
			{
				// DebugExtension.Log("Another instance of " + GetType() + " is already exist! Destroying self...");
				DestroyImmediate(gameObject);
				return;
			}
			else
			{
				instance = this as T;
				OnAwake();
			}

			if (IsDontDestroyThisObject) DontDestroyOnLoad(gameObject);

		}

		protected void OnApplicationQuit()
		{
			if (instance == this)
			{
				DoOnApplicationQuit();
				shuttingDown = true;
			}
		}

		protected void OnDestroy()
		{
			if (instance == this)
			{
				DoOnDestroy();
				shuttingDown = true;
			}
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void DoOnApplicationQuit()
		{
		}

		protected virtual void DoOnDestroy()
		{
		}
	}
}