using BepInEx;
using HarmonyLib;
using KBEngine;
using System.Reflection;
using UnityEngine;

namespace zjr_mcs
{
    [BepInPlugin("plugins.zjr.mcs_tieren", "zjr铁人插件", "1.0.0.0")]
    public class tierenBepInExMod : BaseUnityPlugin
    {// 在插件启动时会直接调用Awake()方法
        void Awake()
        {
            // 使用Debug.Log()方法来将文本输出到控制台
            Debug.Log("Hello, world!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Tab.TabDataBase), "Save")]
    class TierenSavePatch
    {
        public static bool Prefix(Tab.TabDataBase __instance)
        {
            UIPopTip.Inst.Pop("铁人模式不能手动存档", PopTipIconType.感悟);
            return false;
        }
    }

    [HarmonyPatch(typeof(Tab.TabSystemPanel), "ReturnTittle")]
    class TierenReturnPatch
    {
        static bool b_saving;
        public static bool Prefix(Tab.TabSystemPanel __instance)
        {
            TySelect.inst.Show("是否保存并返回主界面？", delegate
            {
                if (!b_saving)
                {
                    b_saving = true;
                    UIPopTip.Inst.Pop("4秒后返回主界面", PopTipIconType.感悟);
                    int tmp_id = PlayerPrefs.GetInt("NowPlayerFileAvatar");
                    YSNewSaveSystem.SaveGame(tmp_id, 1, null, false);
                    SingletonMono<Tab.TabUIMag>.Instance.TryEscClose();
                    System.Threading.Thread tmp_t = new System.Threading.Thread(() => returnTitle(__instance));
                    tmp_t.Start();
                }
            }, null, true);
            return false;
        }
        static void returnTitle(Tab.TabSystemPanel __instance)
        {
            System.Threading.Thread.Sleep(4000);
            if (FpUIMag.inst != null)
            {
                UnityEngine.Object.Destroy(FpUIMag.inst.gameObject);
            }
            if (TpUIMag.inst != null)
            {
                UnityEngine.Object.Destroy(TpUIMag.inst.gameObject);
            }
            if (script.Submit.SubmitUIMag.Inst != null)
            {
                script.Submit.SubmitUIMag.Inst.Close();
            }
            if (script.NewLianDan.LianDanUIMag.Instance != null)
            {
                UnityEngine.Object.Destroy(script.NewLianDan.LianDanUIMag.Instance.gameObject);
            }
            if (LianQiTotalManager.inst != null)
            {
                UnityEngine.Object.Destroy(LianQiTotalManager.inst.gameObject);
            }
            YSGame.YSSaveGame.Reset();
            KBEngineApp.app.entities[10] = null;
            KBEngineApp.app.entities.Remove(10);
            b_saving = false;
            Tools.instance.loadOtherScenes("MainMenu");
        }
    }
    [HarmonyPatch(typeof(Tab.TabSystemPanel), "QuitGame")]
    class TierenQuitPatch
    {
        static bool b_saving;
        public static bool Prefix(Tab.TabSystemPanel __instance)
        {
            TySelect.inst.Show("是否保存并退出游戏？", delegate
            {
                if (!b_saving)
                {
                    b_saving = true;
                    UIPopTip.Inst.Pop("4秒后退出游戏", PopTipIconType.感悟);
                    int tmp_id = PlayerPrefs.GetInt("NowPlayerFileAvatar");
                    YSNewSaveSystem.SaveGame(tmp_id, 1, null, false);
                    SingletonMono<Tab.TabUIMag>.Instance.TryEscClose();
                    System.Threading.Thread tmp_t = new System.Threading.Thread(() => returnTitle(__instance));
                    tmp_t.Start();
                }
            }, null, true);
            return false;
        }
        static void returnTitle(Tab.TabSystemPanel __instance)
        {
            System.Threading.Thread.Sleep(4000);
            Application.Quit();
        }
    }
}
