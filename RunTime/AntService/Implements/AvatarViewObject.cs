using System;
using System.Collections.Generic;
using UnityEngine;


namespace Ant.Metaverse
{

    /// <summary>
    /// AvatarViewObject is a class that is used to get the avatar object from the server.
    /// </summary>
    public class AvatarViewObject : IAvatarService
    {
        private GameObject _avatar;
        private static AvatarViewObject _instance;
        public static AvatarViewObject Instance{
            get{
                if(_instance == null){
                    _instance = new AvatarViewObject();
                }
                return _instance;
            }
        }

#region public methods
        public void GetAvatar(string sceneId, Action<Exception, GameObject> callback)
        {
            try{
                _avatar = GameObject.Find("Avatar");
                Debug.Log("GetAvatar's sceneId: " + sceneId);
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public bool ChangePart()
        {
            try{
                Debug.Log("ChangePart");
                return true;
            }
            catch(Exception e){
                return false;
            }
        }

#endregion

#region private methods


#endregion
    }
}