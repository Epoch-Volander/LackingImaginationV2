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
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace LackingImaginationV2
{

    public class xSeaSerpentEssence
    {
        private static int Script_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

        public static string Ability_Name = "Sea King"; // Aoe vortex that pulls enemies in, ref fuling berserker for smooth movement

        private static GameObject GO_SeaKingProjectile;
        private static Projectile P_SeaKingProjectile;

        public static void Process_Input(Player player, int position) //a class like ROT, will select a random fish, use that fish as icon for status, if eaten, will double the range of vortex for 1 day
        {
            if (!player.GetSEMan().HaveStatusEffect(LackingImaginationUtilities.CooldownString(position)))
            {
                //Ability Cooldown
                StatusEffect se_cd = LackingImaginationUtilities.CDEffect(position);
                se_cd.m_ttl = LackingImaginationUtilities.xSeaSerpentCooldownTime;
                player.GetSEMan().AddStatusEffect(se_cd);

                LackingImaginationV2Plugin.Log($"Serpent Button was pressed");

                Vector3 vector = player.transform.position + player.transform.up * 1.5f + player.GetLookDir() * .5f;
                GameObject prefab = LackingImaginationV2Plugin.p_SeaKing;
                ScheduleFireProjectile(player, vector, prefab);
                
            }
            else
            {
                player.Message(MessageHud.MessageType.TopLeft, $"{Ability_Name} Gathering Power");
            }
        }

        private static void ScheduleFireProjectile(Player player, Vector3 vector, GameObject prefab)
        {
            // Create the projectile
            GO_SeaKingProjectile = UnityEngine.Object.Instantiate(prefab, new Vector3(vector.x, vector.y, vector.z), Quaternion.identity);
            P_SeaKingProjectile = GO_SeaKingProjectile.GetComponent<Projectile>();
            P_SeaKingProjectile.name = "Sea King";
            P_SeaKingProjectile.m_respawnItemOnHit = false;
            P_SeaKingProjectile.m_groundHitOnly = false;
            P_SeaKingProjectile.m_ttl = 60f;
            P_SeaKingProjectile.m_gravity = 5f;
            P_SeaKingProjectile.m_rayRadius = .3f;
            P_SeaKingProjectile.m_aoe = 3f;
            P_SeaKingProjectile.m_hitNoise = 100f;
            P_SeaKingProjectile.transform.localRotation = Quaternion.LookRotation(player.GetAimDir(vector));
            P_SeaKingProjectile.transform.localScale = Vector3.one;

            RaycastHit hitInfo = default(RaycastHit);
            Vector3 player_position = player.transform.position;
            Vector3 target = (!Physics.Raycast(vector, player.GetLookDir(), out hitInfo, float.PositiveInfinity, Script_Layermask) || !(bool)hitInfo.collider) ? (player_position + player.GetLookDir() * 1000f) : hitInfo.point;
            HitData hitData = new HitData();
            // hitData.m_damage.m_slash = UnityEngine.Random.Range(2f, 4f);
            // hitData.m_damage.m_blunt = UnityEngine.Random.Range(1f, 2f);
             hitData.ApplyModifier(((Player.m_localPlayer.GetCurrentWeapon().GetDamage().GetTotalDamage()) * LackingImaginationGlobal.c_seaSerpentSeaKingeProjectile));
            hitData.m_pushForce = 3f;
            hitData.SetAttacker(player);
            Vector3 a = Vector3.MoveTowards(GO_SeaKingProjectile.transform.position, target, 1f);
            P_SeaKingProjectile.Setup(player, (a - GO_SeaKingProjectile.transform.position) * 25f, -1f, hitData, null, null);
            GO_SeaKingProjectile = null;
        }


    }

    [HarmonyPatch]
    public static class xSeaSerpentEssencePassive
    {
        public static List<Character> Fish = new List<Character>();
        public static List<float> FishTImer = new List<float>();
        public static Vector3 impactLocation;
        public static float FishQuality = 0.0f;

        public static List<ItemDrop.ItemData> allCravings = new List<ItemDrop.ItemData>() { };
        public static Dictionary<string, string> nameCravings = new Dictionary<string, string>()
        {
            { "$animal_fish1", "Perch" },            
            { "$animal_fish2", "Pike" },
            { "$animal_fish5", "Trollfish" },
            { "$animal_fish4", "Tetra" },
            { "$animal_fish3", "Tuna" },
            { "$animal_fish8", "Coral Cod" },
            { "$animal_fish6", "Giant Herring" },
            { "$animal_fish7", "Grouper" },
            { "$animal_fish12", "Pufferfish" },
            { "$animal_fish9", "Anglerfish" },
            { "$animal_fish11", "Magmafish" },
            { "$animal_fish10", "Northern Salmon" },
        };

        public static List<string> SeaSerpentStats = new List<string>(){"",};
        
        [HarmonyPatch(typeof(Projectile), "SpawnOnHit")]
        public static class SeaSerpent_SpawnOnHit_Patch
        {
            public static void Postfix(Projectile __instance, ref GameObject go)
            {
                if (__instance.name == "Sea King")
                {
                    impactLocation = __instance.transform.position + __instance.transform.TransformDirection(__instance.m_spawnOffset);
                    float bonusRadius = 0.0f;
                    float baseDuration = LackingImaginationGlobal.c_seaSerpentSeaKingeProjectileDuration;
                    if (__instance.m_owner.GetSEMan().HaveStatusEffect("SE_Satiated"))
                    {
                        bonusRadius += FishQuality;
                        baseDuration += FishQuality * 2;
                    }
                    List<Character> allCharacters = new List<Character>();
                    allCharacters.Clear();
                    Character.GetCharactersInRange(impactLocation, 20f + bonusRadius, allCharacters);
                    foreach (Character ch in allCharacters)
                    {
                        if ((ch.GetBaseAI() != null && ch.GetBaseAI() is MonsterAI && ch.GetBaseAI().IsEnemy(Player.m_localPlayer)) 
                            && !ch.m_tamed ||ch.GetBaseAI() != null && ch.GetBaseAI() is AnimalAI)
                        {
                           //suck em in
                           Fish.Add(ch);
                           FishTImer.Add(baseDuration);
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(Character), "CustomFixedUpdate")]
        public static class SeaSerpent_CustomFixedUpdate_Patch
        {
            public static void Postfix(Character __instance)
            {
                if (!__instance.IsPlayer() && Fish.Contains(__instance) )
                {
                    Vector3 targetPosition = impactLocation;
                    float growthMovement = 0.9f;
                    float CircleRadius = 1f;
                    Vector3 currentPosition = __instance.transform.position;
                    
                    float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

                    if(FishTImer[Fish.IndexOf(__instance)] > 0.0f)
                    {
                        FishTImer[Fish.IndexOf(__instance)] -= Time.deltaTime;
                        if (distanceToTarget > CircleRadius)
                        {
                            // Calculate the new position by moving closer to the target using Lerp
                            currentPosition = Vector3.Lerp(currentPosition, targetPosition, growthMovement * Time.deltaTime);

                            // Update the character's position
                            __instance.transform.position = currentPosition;
                            
                            // Disable gravity for the character's rigidbody (if it has one)
                            Rigidbody rigidbody = __instance.GetComponent<Rigidbody>();
                            if (rigidbody != null)
                            {
                                rigidbody.useGravity = false;
                            }
                        }
                    }
                    if (FishTImer[Fish.IndexOf(__instance)] <= 0.0f)
                    {
                        FishTImer.RemoveAt(Fish.IndexOf(__instance));
                        Fish.Remove(__instance);
                    }
                }
                
            }
        }
        
        [HarmonyPatch(typeof(Player), "UpdateEnvStatusEffects")]
        public static class SeaSerpent_UpdateEnvStatusEffects_Patch
        {
            public static void Prefix(Player __instance, ref float dt)
            {
                SE_Craving se_craving = (SE_Craving)ScriptableObject.CreateInstance(typeof(SE_Craving));
                if (EssenceItemData.equipedEssence.Contains("$item_serpent_essence") && !__instance.GetSEMan().HaveStatusEffect("SE_Satiated"))
                {
                    if (!__instance.GetSEMan().HaveStatusEffect("SE_Craving"))
                    {
                        FishQuality = 0.0f;
                        ItemDrop.ItemData fish = FishSelect(__instance);
                        if(fish != null)
                        {
                            se_craving.m_icon = fish.GetIcon();
                            __instance.GetSEMan().AddStatusEffect(se_craving);
                        }
                    }
                }
                else if (__instance.GetSEMan().HaveStatusEffect("SE_Craving"))
                {
                    __instance.GetSEMan().RemoveStatusEffect(se_craving);
                }
            }
            
            [HarmonyPatch(typeof(Hud), "UpdateStatusEffects")]
            public static class SeaSerpent_UpdateStatusEffectsn_Patch
            {
                public static void Postfix(Hud __instance, ref List<StatusEffect> statusEffects)
                {
                    for (int index = 0; index < statusEffects.Count; ++index)
                    {
                        StatusEffect statusEffect1 = statusEffects[index];
                        if (statusEffect1.name == "SE_Craving")
                        {
                            string iconText = nameCravings[FishName(statusEffect1.m_icon)];
                            RectTransform statusEffect2 = __instance.m_statusEffects[index];
                            Text component2 = statusEffect2.Find("TimeText").GetComponent<Text>();
                            if (!string.IsNullOrEmpty(iconText))
                            {
                                component2.gameObject.SetActive(value: true);
                                component2.text = iconText;
                            }
                            else
                            {
                                component2.gameObject.SetActive(value: false);
                            }
                        }
                    }
                }
            }
            public static ItemDrop.ItemData FishSelect(Player player)
            { 
                List<ItemDrop.ItemData> knownCravings = new List<ItemDrop.ItemData>();
                
                for (int i = 0; i < allCravings.Count; i++)
                {
                    if (player.IsKnownMaterial(allCravings[i].m_shared.m_name))
                    {
                        knownCravings.Add(allCravings[i]);
                    }
                }
                if (SeaSerpentStats[0] != "")
                {
                    for (int i = 0; i < knownCravings.Count; i++)
                    {
                        if (knownCravings[i].m_shared.m_name == SeaSerpentStats[0]) return knownCravings[i];
                    }
                }
                if (knownCravings.Any())
                {
                    int RNDM = Random.Range(0, knownCravings.Count);
                    ItemDrop.ItemData randomFish = knownCravings[RNDM];

                    SeaSerpentStats[0] = randomFish.m_shared.m_name;
                    return randomFish;
                }
                return null;
            }
        }
    
        
        [HarmonyPatch(typeof(Humanoid), "UseItem")]
        public static class SeaSerpent_UseItem_Patch
        {
            public static void Prefix(Humanoid __instance, ref Inventory inventory, ref ItemDrop.ItemData item, ref bool fromInventoryGui)
            {
                if (inventory == null)
                    inventory = __instance.m_inventory;
                if (!inventory.ContainsItem(item))
                    return;
                GameObject hoverObject = __instance.GetHoverObject();
                Hoverable componentInParent1 = (bool) (UnityEngine.Object) hoverObject ? hoverObject.GetComponentInParent<Hoverable>() : (Hoverable) null;
                if (componentInParent1 != null && !fromInventoryGui)
                {
                    Interactable componentInParent2 = hoverObject.GetComponentInParent<Interactable>();
                    if (componentInParent2 != null && componentInParent2.UseItem(__instance, item))
                    {
                        __instance.DoInteractAnimation(hoverObject.transform.position);
                        return;
                    }
                }
                if (__instance.m_seman.HaveStatusEffect("SE_Craving"))
                {
                    if (item.m_shared.m_name == FishName(__instance.m_seman.GetStatusEffect("SE_Craving".GetStableHashCode()).m_icon))
                    {
                        __instance.m_consumeItemEffects.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                        __instance.m_zanim.SetTrigger("eat");
                        inventory.RemoveItem(item.m_shared.m_name, 1);
                        __instance.m_seman.RemoveStatusEffect("SE_Craving".GetStableHashCode());
                        SeaSerpentStats[0] = "";
                        __instance.m_seman.AddStatusEffect("SE_Satiated".GetHashCode());
                        FishQuality = FishIndex(item);
                        LackingImaginationV2Plugin.Log($"SerpentQ {FishQuality}");
                        return;
                    }
                }
            }
        }
        public static string FishName(Sprite icon)
        {
            for (int i = 0; i < allCravings.Count; i++)
            {
                if (allCravings[i].GetIcon() == icon)
                {
                     return allCravings[i].m_shared.m_name;
                }
            }
            return "none";
        }

        public static float FishIndex(ItemDrop.ItemData item)
        {
            for (int i = 0; i < allCravings.Count; i++)
            {
                if (allCravings[i].m_shared.m_name == item.m_shared.m_name)
                {
                    return (float)(i + 1);
                }
            }
            return 0f;
        }
        
        
        
    }
}