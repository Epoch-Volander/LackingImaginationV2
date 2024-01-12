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
using System.Runtime.Versioning;
using System.Threading;



namespace LackingImaginationV2
{
// $enemy_surtling "vfx_Potion_stamina_medium" "vfx_WishbonePing"
    public class xSurtlingEssence
    {
        public static string Ability_Name = "Harbinger";
        public static int Charges;
        public static bool SurtlingController;
        
        private static float minDistanceBetweenCharacters = LackingImaginationGlobal.c_surtlingHarbingerMinDistance; // Set the minimum distance between characters
        
        public static List<GameObject[]> Rifts = new List<GameObject[]>();
        public static List<Character> Surts = new List<Character>();
        
        public static void Process_Input(Player player, int position)
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                
                List<Character> allch = new List<Character>();
                allch.Clear();
                Character.GetCharactersInRange(player.GetCenterPoint(), 25f, allch);
                Func<Character, bool> condition = ch => ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI;
                if (allch.Any(condition))
                {
                    Charges = int.Parse(xSurtlingEssencePassive.SurtlingStats[0]);
                    
                    if(Charges == 0) Charges = CorePay(1);
                
                    if(Charges > 0)
                    {
                        Charges--;
                        xSurtlingEssencePassive.SurtlingStats[0] = Charges.ToString();
                    
                        //Ability Cooldown
                        StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                        se_cd.m_ttl = LackingImaginationUtilities.xSurtlingCooldownTime;
                        player.GetSEMan().AddStatusEffect(se_cd);
                    
                        //Effects, animations, and sounds
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_ImpDeath"), player.transform.position, Quaternion.identity);
                        UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_imp_death"), player.transform.position, Quaternion.identity);
                        // UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_Harbinger, player.transform.position, Quaternion.identity);
                    
                        LackingImaginationV2Plugin.UseGuardianPower = false;
                        // SurtlingController = true;
                        RPC_LI.AnimationCaller("Surtling", true);
                        ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player)).SetTrigger("gpower");
                        // SurtlingController = false;
                        RPC_LI.AnimationCaller("Surtling", false);
                        
                        //Lingering effects
                        SE_Harbinger se_harbinger = (SE_Harbinger)ScriptableObject.CreateInstance(typeof(SE_Harbinger));
                        se_harbinger.m_ttl = SE_Harbinger.m_baseTTL;
                    
                        player.GetSEMan().AddStatusEffect(se_harbinger);
                    
                        //Get suitable targets
                        List<Character> allCharacters = new List<Character>();
                        allCharacters.Clear();
                        Character.GetCharactersInRange(Player.m_localPlayer.GetCenterPoint(), 25f, allCharacters);
                        List<Character> affectedCharacters = new List<Character>();
                        affectedCharacters.Clear();
                        foreach (Character currentCharacter in allCharacters)
                        {
                            if ((currentCharacter.GetBaseAI() is MonsterAI && currentCharacter.GetBaseAI().IsEnemy(Player.m_localPlayer)) && !currentCharacter.m_tamed ||
                                currentCharacter.GetBaseAI() is AnimalAI)
                            {
                                bool tooClose = false;
                            
                                // Check if the current character is too close to any affected character
                                foreach (Character affectedCharacter in affectedCharacters)
                                {
                                    float distance = Vector3.Distance(currentCharacter.transform.position, affectedCharacter.transform.position);
                                    if (distance < minDistanceBetweenCharacters)
                                    {
                                        tooClose = true;
                                        break; // No need to check other affected characters if one is too close
                                    }
                                }
                                if (!tooClose)
                                {
                                    // GameObject effectInstance = UnityEngine.Object.Instantiate(LackingImaginationV2Plugin.fx_Harbinger, currentCharacter.transform.position, Quaternion.identity);
                                    
                                    EffectList m_starteffect = new EffectList
                                    {
                                        m_effectPrefabs = new EffectList.EffectData[]
                                        {
                                            new()
                                            {
                                                m_prefab = LackingImaginationV2Plugin.fx_Harbinger,
                                                m_enabled = true,
                                                m_variant = 0,
                                                m_attach = false,
                                                m_follow = false,
                                                // m_inheritParentScale = true,
                                                // m_multiplyParentVisualScale = true,
                                                // m_scale = true,
                                            }
                                        }
                                    };
                                    
                                    m_starteffect.m_effectPrefabs[0].m_prefab.GetComponent<TimedDestruction>().m_timeout = SE_Harbinger.m_baseTTL;
                                    
                                    GameObject[] Aura = m_starteffect.Create(currentCharacter.transform.position, Quaternion.identity);
                                    Rifts.Add(Aura);
                                    
                                    // Add the current character to the list of affected characters
                                    affectedCharacters.Add(currentCharacter);
                                    
                                    List<Character> BurnAoe = new List<Character>();
                                    Character.GetCharactersInRange(currentCharacter.GetCenterPoint(), 3.5f, BurnAoe);
                                    foreach (Character burnChar in BurnAoe)
                                    {
                                        if ((burnChar.GetBaseAI() is MonsterAI && burnChar.GetBaseAI().IsEnemy(Player.m_localPlayer)) && !burnChar.m_tamed || burnChar.GetBaseAI() is AnimalAI)
                                        {
                                            HitData hitData = new HitData();
                                            hitData.m_damage.m_fire = UnityEngine.Random.Range(1f, 3f);
                                            hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_surtlingHarbingerBurn));
                                            hitData.m_point = burnChar.GetCenterPoint();
                                            hitData.SetAttacker(player);
                                            burnChar.Damage(hitData);
                                        }
                                    }
                                    SummonRift(currentCharacter);
                                    // SummonRift(currentCharacter);
                                }
                            }
                        }
                    }
                }
                else
                {
                    player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Has No Targets");
                }
            }
            // else
            // {
            //     player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            // }
        }
        static int CorePay(int cost)
        {
            if (Player.m_localPlayer.m_inventory.CountItems("$item_surtlingcore") >= cost)
            {
                Player.m_localPlayer.m_inventory.RemoveItem("$item_surtlingcore", cost);
                return (int)LackingImaginationGlobal.c_surtlingHarbingerCharges;
            }
            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Requires Surtling Cores to fuel");
            return 0;
        }
        static void SummonRift(Character currentCharacter)
        {
            Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * 2f;
            Vector3 randomPosition = currentCharacter.transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
            GameObject surt = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("Surtling"), randomPosition, Quaternion.identity);
            
            surt.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
            surt.GetComponent<Humanoid>().SetMaxHealth(surt.GetComponent<Humanoid>().GetMaxHealthBase() * 4f);
            surt.GetComponent<MonsterAI>().m_attackPlayerObjects = false;
            surt.GetComponent<CharacterDrop>().m_dropsEnabled = false;
            surt.AddComponent<Tameable>();
            surt.GetComponent<Character>().SetTamed(true);
            surt.GetComponent<Tameable>().SetText("Surtling(Ally)");
            // surt.GetComponent<Tameable>().m_nview.GetZDO().Set(ZDOVars.s_tamedName, "Surtling(Ally)");
            surt.GetComponent<Tameable>().m_unsummonDistance = 100f;
            surt.GetComponent<Tameable>().m_unsummonOnOwnerLogoutSeconds = 3f;
            foreach (CharacterDrop.Drop drop in surt.GetComponent<CharacterDrop>().m_drops) drop.m_chance = 0f;
            Surts.Add(surt.GetComponent<Character>());
        }
    }

    
    
    [HarmonyPatch]
    public static class xSurtlingEssencePassive
    {
        public static List<string> SurtlingStats = new List<string>(){"0"};


        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        public static class Surtling_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (xSurtlingEssence.Rifts.Any() && __instance.IsPlayer() && __instance == Player.m_localPlayer && !__instance.GetSEMan().HaveStatusEffect("SE_Harbinger"))
                {
                    foreach (GameObject[] rift in xSurtlingEssence.Rifts)
                    {
                       
                        foreach (GameObject auraObject in rift)
                        {
                            UnityEngine.Object.Destroy(auraObject);
                        }
                    }
                    xSurtlingEssence.Rifts.Clear();
                    foreach (Character surt in xSurtlingEssence.Surts)
                    {
                        DestroyNow(surt);
                    }
                    xSurtlingEssence.Surts.Clear();
                }

                if (EssenceItemData.equipedEssence.Contains("$item_surtling_essence"))
                {
                    __instance.m_tolerateWater = false;
                }
                else
                {
                    __instance.m_tolerateWater = true;
                }
            }
            static void DestroyNow(Character surt)
            {
                if (!surt.m_nview.IsValid() || !surt.m_nview.IsOwner())
                    return;
                surt.GetComponent<Character>().ApplyDamage(new HitData()
                {
                    m_damage = {
                        m_damage = 999999f
                    },
                    m_point = surt.transform.position
                }, false, true);
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        public static class Surtling_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance,ref float dt)
            {

                if (EssenceItemData.equipedEssence.Contains("$item_surtling_essence"))
                {
                    __instance.m_nearFireTimer = 0f;
                }
            }
        }
        
        //Damaged or Debuff when wet?
        
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        public static class Surtling_RPC_Damage_Patch
        {
            public static void Prefix(Character __instance, ref HitData hit)
            {
                if (EssenceItemData.equipedEssence.Contains("$item_surtling_essence"))
                {
                    if (__instance.IsDebugFlying())
                        return;
                    if ((UnityEngine.Object) hit.GetAttacker() == (UnityEngine.Object) Player.m_localPlayer)
                    {
                        __instance.m_localPlayerHasHit = true;
                    }
                    if (!__instance.m_nview.IsOwner() || (double) __instance.GetHealth() <= 0.0 || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || hit.m_dodgeable && __instance.IsDodgeInvincible())
                        return;
                    Character attacker = hit.GetAttacker();
                    if (hit.HaveAttacker() && (UnityEngine.Object)attacker == (UnityEngine.Object)null || __instance.IsPlayer() && !__instance.IsPVPEnabled() && (UnityEngine.Object)attacker != (UnityEngine.Object)null && attacker.IsPlayer() && !hit.m_ignorePVP)
                        return;
                    if (__instance != null && __instance.IsPlayer() && hit.m_hitType == HitData.HitType.Water)
                    {
                        hit.m_damage.m_damage *= 0.2f;
                        // LackingImaginationV2Plugin.Log($"WaterHit");
                        
                    }
                }
            }
        }
        
        
    }
}