using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Scheme;

namespace UserAuthentication
{
    

    
    
  
    /// <summary>
    /// Defines the <see cref="UsersDB" />.
    /// </summary>
    public class UsersDB : MonoBehaviour
    {
        /// <summary>
        /// Defines the _connection.
        /// </summary>
        private static SQLiteConnection _connection;

        [SerializeField]
        bool _clearDB = false;

        // Start is called before the first frame update
        /// <summary>
        /// The Start.
        /// </summary>
        internal void Start()
        {
            string DatabaseName = "Users.db";
#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            Debug.Log("Final PATH: " + dbPath);

            if (_clearDB)
            {
                InitDB();
            }


        }

        void InitDB()
        {

            _connection.DropTable<User>();
            _connection.CreateTable<User>();

            _connection.DropTable<Shot>();
            _connection.CreateTable<Shot>();





            
            _connection.InsertAll(new[]{
            new User{
                UserName = "admin",
                Password = "admin",
                Email = "mahmoud.ashraf@umbrasys.com",
                FullName = "Mahmoud Ashraf",
                Created = System.DateTime.Now,
                Age = "26"
            }
            });
            try
            {
                int result = _connection.InsertAll(new[]{
                new User{
                    UserName = "Player",
                    Password = "Player",
                    Email = "mahmoud.ashraf.cis@gmail.com",
                    FullName = "Mahmoud Ashraf (player)",
                    Created = System.DateTime.Now,
                    Age = "26"
                }
                });

                //Debug.Log(result + " of rows were inserted!");
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
            
            var usersTable = _connection.Table<User>().ToList<User>();
            //foreach (var s in usersTable)
            //{
            //    Debug.Log(s.ToString());
            //}


            //Debug.LogWarning("Table of Student [0]");
            //var sOf0 = usersTable.First();
            //Debug.Log(sOf0.ToString());


            //Debug.LogWarning("Table of Student where name = Roberto");
            //User sRobert = _connection.Table<User>().Where(x => x.Name == "Roberto").FirstOrDefault();
            //Debug.Log(sRobert.ToString());


            //Debug.LogWarning("Table of Shot was updated with the following");
            
            Shot mShot = new Shot();
            mShot.PlayerId = 1;
            mShot.Score = 4000;
            mShot.Distance = 10;
            _connection.Insert(mShot);


            mShot = new Shot();
            mShot.PlayerId = 1;
            mShot.Score = 500;
            mShot.Distance = 10;
            _connection.Insert(mShot);
             


            var shotsTable = _connection.Table<Shot>().ToList<Shot>();
            //foreach (var s in shotsTable)
            //{
            //    Debug.Log(s.ToString());
            //}




            var q = from users in _connection.Table<User>()
                    join shots in _connection.Table<Shot>() on users.Id equals shots.PlayerId
                    where users.Id == 4


                    select new { shots.Score };
            //foreach (var r in q)
            //{
            //    Debug.Log(r.ToString());
            //}

            var joinResult = from u in usersTable
                             join s in shotsTable on u.Id equals s.PlayerId
                             select new { Score = s.Score, UserName = u.FullName, UserID = u.Id };

            //foreach (var r in joinResult)
            //{
            //    Debug.Log(r.ToString() + "[Score = " + r.Score + ", UserName = " + r.UserName + ", UserId + " + r.UserID);
            //}

            var joinResult2 = usersTable.AsEnumerable().Join(shotsTable.AsEnumerable(),
                u => u.Id,
                s => s.PlayerId,
                (u, s) => new { u, s }
                ).Where(z => true).Select(z => z);

            //Debug.LogError("hi" + joinResult2.AsEnumerable());
            //Debug.LogError("hi" + joinResult2.Count());
            //foreach (var r in joinResult2)
            //{
            //    Debug.Log(r.ToString());
            //}



            //var innerJoin = shotsTable.Join(shotsTable,
            //        usersTable, // inner
            //        shot1 => shot1.Id, // outerKeySelector
            //        user1 => user1.Id, //innerKeySelector
            //        (shot1, user1) => new { OwnerName = shot1} //result selector
            //        );


            //foreach (var obj in innerJoin)
            //{
            //    Debug.Log(obj.OwnerName);
            //}
            //Debug.Log(innerJoin);

            //int i = 0;

        }


        private void OnApplicationQuit()
        {
            //Debug.LogWarning("Application quit - colesing DB connection!");
            _connection.Close();
            _connection.Dispose();
        }
        public static SQLiteConnection Connect()
        {
            return _connection;
        }
    }

}