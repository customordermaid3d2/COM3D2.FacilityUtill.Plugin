
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.FacilityUtill.Plugin
{
    class FacilityUtillPatch
    {
        [HarmonyPatch(typeof(CharacterMgr), "SetActive")]
        [HarmonyPostfix]// CharacterMgr의 SetActive가 실행 후에 아래 메소드 작동
        public static void SetActive(Maid f_maid, int f_nActiveSlotNo, bool f_bMan)
        {
            FacilityUtillMain.MyLog.LogMessage("CharacterMgr.SetActive", f_nActiveSlotNo, f_bMan, f_maid.status.fullNameEnStyle);
        }

        [HarmonyPatch(typeof(CharacterMgr), "Deactivate")]
        [HarmonyPrefix] // CharacterMgr의 SetActive가 실행 전에 아래 메소드 작동
        public static void Deactivate(int f_nActiveSlotNo, bool f_bMan)
        {
            FacilityUtillMain.MyLog.LogMessage("CharacterMgr.Deactivate", f_nActiveSlotNo, f_bMan);
        }
    }
}
