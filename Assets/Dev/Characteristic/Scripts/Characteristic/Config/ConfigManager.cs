using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow
{
    public class ConfigManager : MonoSingleton<ConfigManager>
    {
        #region define variable
        // ID user after login 
        public static string IdUser = "IdUser";
        public static string LoginType = "LoginType";

        // version file config current in game
        public static string VersionConfig = "VersionConfig";

        // Name file data of user will store de byte data from the object.
        public static string FileNameProfile = "UserProfile";


        // Name file data of game will store de byte data from the object.
        public static string FileNameGameData = "GameData";

        // Distance auto move position 
        public static float DistanceMoveVassar = 10.0f;

        #endregion
    }
}