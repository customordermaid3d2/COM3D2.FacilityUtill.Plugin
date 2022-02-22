using BepInEx;
using BepInEx.Configuration;

using COM3D25.LillyUtill;
using COM3D2API;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.FacilityUtill.Plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "FacilityUtill";
        public const string PLAGIN_VERSION = "22.2.22";
        public const string PLAGIN_FULL_NAME = "COM3D2.FacilityUtill.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInPlugin("COM3D2.Sample.Plugin", "COM3D2.Sample.Plugin", "21.6.6")]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]
    public class FacilityUtillMain : BaseUnityPlugin
    {

        Harmony harmony;

        public static FacilityUtillMain sample;
        public static MyLog MyLog;


        public FacilityUtillMain()
        {
            sample = this;
        }

        /// <summary>
        ///  게임 실행시 한번만 실행됨
        /// </summary>
        public void Awake()
        {
            MyLog = new MyLog(Logger);
            MyLog.LogInfo("Awake");
        }



        public void OnEnable()
        {
            MyLog.LogInfo("OnEnable");

            //SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            harmony = Harmony.CreateAndPatchAll(typeof(FacilityUtillPatch));

            myWindowRect = new MyWindowRect(Config, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, "FU", ho: 120);
        }

        /// <summary>
        /// 게임 실행시 한번만 실행됨
        /// </summary>
        public void Start()
        {
            MyLog.LogInfo("Start");
            ShowCounter = Config.Bind("GUI", "isGUIOnKey", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha5, KeyCode.LeftControl));

            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME
                , new Action(delegate ()
                {
                    myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;
                    FacilityUtillMain.MyLog.LogInfo($"SystemShortcutAPI {myWindowRect.IsGUIOn}");
                })
                , MyAttribute.PLAGIN_NAME + " : " + ShowCounter.Value.ToString()
                , MyUtill.ExtractResource(COM3D25.FacilityUtill.Plugin.Properties.Resources.icon)
            );

        }

        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;
        private static int windowId = new System.Random().Next();
        public static MyWindowRect myWindowRect;
        private static Vector2 scrollPosition;

        public void OnGUI()
        {
            if (!myWindowRect.IsGUIOn)
                return;

            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        public void WindowFunction(int id)
        {
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsGUIOn = false; }
            GUILayout.EndHorizontal();

            if (!myWindowRect.IsOpen)
            {
            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                if (GUILayout.Button("Facility Auot Set - Random")) FacilityUtill.SetFacilityAll(true);
                if (GUILayout.Button("Facility Auot Set - sequential")) FacilityUtill.SetFacilityAll(false);
                if (GUILayout.Button("Facility exp max")) FacilityUtill.SetMaxExp();

                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }

        public void OnDisable()
        {
            MyLog.LogInfo("OnDisable");

            //SceneManager.sceneLoaded -= this.OnSceneLoaded;

            harmony.UnpatchSelf();// ==harmony.UnpatchAll(harmony.Id);
            //harmony.UnpatchAll(); // 정대 사용 금지. 다름 플러그인이 패치한것까지 다 풀려버림
        }


    }
}
