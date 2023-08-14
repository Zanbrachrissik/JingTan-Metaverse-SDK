using System;
using System.Collections.Generic;
using UnityEngine;


namespace Ant.MetaVerse{
    public class AvatarViewObject : IAvatarService, IService
    {

        private static AvatarViewObject _instance;
        public static AvatarViewObject Instance{
            get{
                if(_instance == null){
                    _instance = new AvatarViewObject();
                }
                return _instance;
            }
        }

        public void GetAvatar(string sceneId, Action<Exception, UnityEngine.Object> callback)
        {
            try{
                Debug.Log("GetAvatar's sceneId: " + sceneId);
            }
            catch(Exception e){
                callback(e, null);
            }
        }
    }
}