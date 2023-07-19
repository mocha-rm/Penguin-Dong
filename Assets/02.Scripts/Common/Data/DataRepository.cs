using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

namespace Delvine
{
	public abstract class DataRepository
	{
		protected SQLiteConnection _connection;

		public DataRepository(string DatabaseName)
		{
#if UNITY_EDITOR
			var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
			// check if file exists in Application.persistentDataPath
			var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
			
			if (!File.Exists(filepath))
			{				
#if UNITY_ANDROID
				var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
				while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
				// then save to Application.persistentDataPath
				if(string.IsNullOrEmpty(loadDb.error))
                {
					File.WriteAllBytes(filepath, loadDb.bytes);
				}
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
				if(File.Exists(loadDb))
				{
					File.Copy(loadDb, filepath);
				}
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
				if(File.Exists(loadDb))
				{
					File.Copy(loadDb, filepath);
				}

#elif UNITY_WINRT
				var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
				// then save to Application.persistentDataPath
				if(File.Exists(loadDb))
				{
					File.Copy(loadDb, filepath);
				}
		
#elif UNITY_STANDALONE_OSX
				var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
				// then save to Application.persistentDataPath
				if(File.Exists(loadDb))
				{
					File.Copy(loadDb, filepath);
				}
#else
				var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
				// then save to Application.persistentDataPath
				if(File.Exists(loadDb))
				{
					File.Copy(loadDb, filepath);
				}
#endif
			}
			var dbPath = filepath;
#endif
			_connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
		}


		public virtual void Clear()
		{
			_connection.Close();
		}

	}
}
