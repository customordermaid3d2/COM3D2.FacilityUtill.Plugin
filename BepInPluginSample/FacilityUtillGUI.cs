using BepInEx;
using BepInEx.Configuration;

using COM3D2.LillyUtill;
using COM3D2API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.FacilityUtill.Plugin
{
    class FacilityUtillGUI : MonoBehaviour
    {
        public static FacilityUtillGUI instance;

        private static ConfigFile config;

        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        private static int windowId = new System.Random().Next();

        private static Vector2 scrollPosition;

        // 위치 저장용 테스트 json
        public static MyWindowRect myWindowRect;

        public static bool IsOpen
        {
            get => myWindowRect.IsOpen;
            set => myWindowRect.IsOpen = value;            
        }

        // GUI ON OFF 설정파일로 저장
        private static ConfigEntry<bool> IsGUIOn;

        public static bool isGUIOn
        {
            get => IsGUIOn.Value;
            set => IsGUIOn.Value = value;
        }

        internal static FacilityUtillGUI Install(GameObject parent,ConfigFile config)
        {
            FacilityUtillGUI. config = config;
            instance = parent.GetComponent<FacilityUtillGUI>();
            if (instance == null)
            {
                instance = parent.AddComponent<FacilityUtillGUI>();
                FacilityUtillMain.MyLog.LogMessage("GameObjectMgr.Install", instance.name);                
            }
            return instance;
        }

        public void Awake()
        {
            myWindowRect = new MyWindowRect(config, MyAttribute.PLAGIN_FULL_NAME,ho:120);
            IsGUIOn = config.Bind("GUI", "isGUIOn", false);
            ShowCounter = config.Bind("GUI", "isGUIOnKey", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha5, KeyCode.LeftControl));
            SystemShortcutAPI.AddButton(MyAttribute.PLAGIN_FULL_NAME, new Action(delegate () { FacilityUtillGUI.isGUIOn = !FacilityUtillGUI.isGUIOn; }), MyAttribute.PLAGIN_NAME + " : " + ShowCounter.Value.ToString(), MyUtill.ExtractResource(COM3D2.FacilityUtill.Plugin.Properties.Resources.icon));
        }

        public void OnEnable()
        {
            FacilityUtillMain.MyLog.LogMessage("OnEnable");

            FacilityUtillGUI.myWindowRect.load();
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }
        /*
        public void Start()
        {
            Sample.MyLog.LogMessage("Start");            
        }
        */
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FacilityUtillGUI.myWindowRect.save();
        }

        private void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            if (ShowCounter.Value.IsUp())
            {
                isGUIOn = !isGUIOn;
                FacilityUtillMain.MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            }
        }

        public void OnGUI()
        {
            if (!isGUIOn)
                return;

            //GUI.skin.window = ;

            //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        public void WindowFunction(int id)
        {
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Label(MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(),GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { IsOpen = !IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn = false; }
            GUILayout.EndHorizontal();

            if (!IsOpen)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                if (GUILayout.Button("Facility Auot Set - Random")) FacilityUtill.SetFacilityAll(true);
                if (GUILayout.Button("Facility Auot Set - sequential")) FacilityUtill.SetFacilityAll(false);
                if (GUILayout.Button("Facility exp max")) FacilityUtill.SetMaxExp();

                GUILayout.EndScrollView();

                if (GUI.changed)
                {
                    Debug.Log("changed");
                }

            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        public void OnDisable()
        {
            FacilityUtillGUI.isCoroutine = false;
            FacilityUtillGUI.myWindowRect.save();
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        public static bool isCoroutine = false;
        public static int CoroutineCount = 0;

        private IEnumerator MyCoroutine()
        {
            isCoroutine = true;
            while (isCoroutine)
            {
                FacilityUtillMain.MyLog.LogMessage("MyCoroutine ", ++CoroutineCount);
                //yield return null;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
