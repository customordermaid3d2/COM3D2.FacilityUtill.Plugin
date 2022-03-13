using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using COM3D2API;
using FacilityFlag;
using HarmonyLib;
using LillyUtill.MyWindowRect;
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
        public const string PLAGIN_VERSION = "22.3.13";
        public const string PLAGIN_FULL_NAME = "COM3D2.FacilityUtill.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInPlugin("COM3D2.Sample.Plugin", "COM3D2.Sample.Plugin", "21.6.6")]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]
    public class FacilityUtillMain : BaseUnityPlugin
    {

        public static FacilityUtillMain sample;
        internal static ManualLogSource log;

        private static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;
        
        public static WindowRectUtill myWindowRect;
        //private static Vector2 scrollPosition;

        public FacilityUtillMain()
        {
            sample = this;
            log = Logger;
        }

        /// <summary>
        ///  게임 실행시 한번만 실행됨
        /// </summary>
        public void Awake()
        {            
            log.LogInfo("Awake");
            myWindowRect = new WindowRectUtill(WindowFunctionBody, Config, Logger,  MyAttribute.PLAGIN_NAME, "FU", ho: 120);
        }

        /// <summary>
        /// 게임 실행시 한번만 실행됨
        /// </summary>
        public void Start()
        {
            log.LogInfo("Start");
            ShowCounter = Config.Bind("GUI", "isGUIOnKey", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha5, KeyCode.LeftControl));

            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME
                , () => { myWindowRect.IsGUIOnOffChg(); }
                , MyAttribute.PLAGIN_NAME + " : " + ShowCounter.Value.ToString()
                , Properties.Resources.icon
            );

            FacilityUtill.facilityExpArray = Traverse.Create(GameMain.Instance.FacilityMgr).Field("m_FacilityExpArray").GetValue<DataArray<int, SimpleExperienceSystem>>();
        }

        public void OnGUI()
        {
            myWindowRect.OnGUI();
            /*
            if (!myWindowRect.IsGUIOn)
                return;

            myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
            */
        }
        /*
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

                WindowFunctionBody(id);

                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }
        */

        private void WindowFunctionBody(int id)
        {
            if (GUILayout.Button("Facility Auot Set - Random")) FacilityUtill.SetFacilityAll(true);
            if (GUILayout.Button("Facility Auot Set - sequential")) FacilityUtill.SetFacilityAll(false);
            if (GUILayout.Button("Facility exp max")) FacilityUtill.SetMaxExp();
        }
    }
}
