using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Threading;
using PieceManager;


namespace LackingImaginationV2
{

    public class xAbominationEssence
    {
        public static string Ability_Name = "Bane";
        
        public static List<Character> Abom = new List<Character>();
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
            
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xAbominationCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);
                
                foreach (Character ch in xAbominationEssence.Abom)
                {
                    if(ch.GetComponent<Tameable>() == null) break;
                    ch.GetComponent<Tameable>().UnSummon();
                }
                xAbominationEssence.Abom.Clear();
                
                LackingImaginationV2Plugin.Log($"xAbominationEssence Button was pressed");


                GameObject effect = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_firetreecut_dead_abomination"), player.transform.position, Quaternion.identity);
                foreach (Transform child in effect.transform)
                {
                    Transform TransformSystem = child.GetComponent<Transform>();
                    if (TransformSystem != null)
                    {
                        TransformSystem.localScale *= 0.3f;
                    }
                }
            
                Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * 3f;
                Vector3 randomPosition = player.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
                GameObject baby = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Abomination"), randomPosition, Quaternion.identity);
                baby.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
                baby.GetComponent<Transform>().localScale = 0.5f * Vector3.one;
                baby.GetComponent<Humanoid>().m_name = "Bane(Ally)";
                baby.GetComponent<Humanoid>().SetMaxHealth(baby.GetComponent<Humanoid>().GetMaxHealthBase() * 5f);
                baby.GetComponent<Humanoid>().m_speed = 3f;
                baby.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
                baby.GetComponent<MonsterAI>().m_consumeItems = new List<ItemDrop>() {ZNetScene.instance.GetPrefab("Wood").GetComponent<ItemDrop>()};
                baby.GetComponent<MonsterAI>().m_consumeRange = 2f;
                baby.GetComponent<MonsterAI>().m_consumeSearchRange = 10f;
                baby.GetComponent<MonsterAI>().m_consumeSearchInterval = 10f;
                baby.AddComponent<Tameable>();
                // baby.GetComponent<Tameable>().m_startsTamed = true;
                baby.GetComponent<Tameable>().Tame();
                baby.GetComponent<Tameable>().m_unsummonDistance = 150f;
                baby.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
                baby.GetComponent<Tameable>().m_commandable = true;
                baby.GetComponent<Tameable>().m_unSummonEffect = new EffectList()
                {
                    m_effectPrefabs = new EffectList.EffectData[]
                    {
                        new()
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("vfx_firetreecut_dead_abomination"),
                            m_enabled = true,
                        }
                    }
                };
                baby.GetComponent<CharacterDrop>().m_dropsEnabled = false;
                foreach (CharacterDrop.Drop drop in baby.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
                // baby.GetComponent<Character>().SetLevel(3);
                Abom.Add(baby.GetComponent<Character>());

               
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }
        
    }

    [HarmonyPatch]
    public static class xAbominationEssencePassive
    {
        [HarmonyPatch(typeof(Player), "GetBodyArmor")]
        public static class Abomination_GetBodyArmor_Patch
        {
            public static void Postfix(ref float __result)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_abomination_essence"))
                {
                    __result += LackingImaginationGlobal.c_abominationBaneArmor;
                }
            }
        }
        
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        public static class Abomination_GetTotalFoodValue_Patch
        {
            public static void Postfix( ref float hp)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_abomination_essence") && EnvMan.instance.IsDay())
                {
                    hp -= hp * LackingImaginationGlobal.c_abominationBaneHealth;
                }
            }
        }
        
        
    }
}