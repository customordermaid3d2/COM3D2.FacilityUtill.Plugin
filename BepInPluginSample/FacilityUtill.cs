using FacilityFlag;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BepInPluginSample
{
    internal class FacilityUtill
    {
        public static List<Facility.FacilityStatus> listbak;
        public static List<Facility.FacilityStatus> list;

        public static void SetFacilityAll(bool random)
        {


            SetFacilityListInit();

            //List<Facility.FacilityStatus> list = new();

            for (int i = 2; i < GameMain.Instance.FacilityMgr.FacilityCountMax; i++)
            {
                if (list.Count == 0)
                {
                    list.AddRange(listbak);
                }

                Facility.FacilityStatus item;
                if (random)
                    item = list[UnityEngine.Random.Range(0, FacilityUtill.list.Count)];
                else
                    item = list[0];

                Facility facility = GameMain.Instance.FacilityMgr.CreateNewFacility(item.typeID);


                GameMain.Instance.FacilityMgr.SetFacility(i, facility);

                list.Remove(item);

            }

            FacilityUtill.SetMaxExp();
        }

        private static DataArray<int, SimpleExperienceSystem> facilityExpArray;

        public static DataArray<int, SimpleExperienceSystem> FacilityExpArray
        {
            get
            {
                if (facilityExpArray == null)
                {
                    facilityExpArray = Traverse.Create(GameMain.Instance.FacilityMgr).Field("m_FacilityExpArray").GetValue<DataArray<int, SimpleExperienceSystem>>();
                }
                return facilityExpArray;
            }
            set => facilityExpArray = value;
        }

        public static void SetMaxExp()
        {
            if (FacilityExpArray == null)
            {
                //FacilityManagerPatch.m_FacilityExpArray= AccessTools.Field(typeof(FacilityManager), "m_FacilityAchievementList");

                return;
            }
            foreach (KeyValuePair<int, SimpleExperienceSystem> item in FacilityExpArray.Copy())
            {
                SimpleExperienceSystem experienceSystem = item.Value;
                experienceSystem.AddExp(experienceSystem.GetMaxLevelNeedExp());
            }

        }

        private static void SetFacilityListInit()
        {
            if (listbak == null)
            {
                listbak = FacilityDataTable.GetFacilityStatusArray(true).ToList();
                List<Facility.FacilityStatus> listbak2 = new(listbak);
                //listbak2.AddRange(listbak);
                foreach (var item in listbak2)
                {

                    if (!FacilityDataTable.GetFacilityCanBeDestroy(item.typeID, true))
                    //if (item.typeID==100 || item.typeID ==150)
                    {
                        // InvalidOperationException: Collection was modified; enumeration operation may not execute
                        listbak.Remove(item);// foreach 에서 Remove 실행시 배열이 바뀜. 즉 Remove 실행하기 위한 다른 조치 방안 필요
                    }
                }
            }
            if (list == null)
            {
                list = new List<Facility.FacilityStatus>();
            }
            list.Clear();
        }


    }
}