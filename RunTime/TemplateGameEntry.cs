using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ant.Metaverse
{
    public class TemplateGameEntry : MonoBehaviour {
        
        private static TemplateGameEntry _instance;
        public static TemplateGameEntry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TemplateGameEntry();
                }
                return _instance;
            }
        }

        private void Awake() {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public void StartGame(string jsonstring)
        {
            // 鲸探会调用StartGame方法，传入启动参数jsonstring
            Debug.Log("StartGame " + jsonstring);
            // 开始初始化、切换场景等操作
            // .........


            // 初始化完成后隐藏 loading 界面
            Factory.GetService<ICommonService>().HideLoadingView();
        }

        private void OnDestroy() {
            Debug.Log("TemplateGameEntry OnDestroy");
        }

    }
}