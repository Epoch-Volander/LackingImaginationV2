using System;
using System.IO;
using HarmonyLib;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;




namespace LackingImaginationV2
{
    public class EssenceStatsLI
    {
         public static List<List<string>> li_stringList = new List<List<string>>();

        [HarmonyPatch(typeof(PlayerProfile), nameof(PlayerProfile.SavePlayerToDisk), null)]
        public static class Save_LI_StringList_Patch
        {
            public static void Postfix(PlayerProfile __instance, string ___m_filename, string ___m_playerName)
            {
                LackingImaginationV2Plugin.Log($"sav{li_stringList.Count}");
                foreach (List<string> subList in li_stringList)
                {
                    LackingImaginationV2Plugin.Log($"Sub-list count: {subList.Count}");
                    foreach (string item in subList)
                    {
                        LackingImaginationV2Plugin.Log($"Item: {item}");
                    }

                    LackingImaginationV2Plugin.Log("End of sub-list");
                }

                try
                {
                    Directory.CreateDirectory(Utils.GetSaveDataPath(FileHelpers.FileSource.Local) +
                                              "/characters/LackingI");
                    string text = Utils.GetSaveDataPath(FileHelpers.FileSource.Local) + "/characters/LackingI/" +
                                  ___m_filename + "_li_strings.fch";
                    string text3 = Utils.GetSaveDataPath(FileHelpers.FileSource.Local) + "/characters/LackingI/" +
                                   ___m_filename + "_li_strings.fch.new";
                    ZPackage zPackage = new ZPackage();

                    // Serialize the string list data
                    zPackage.Write(li_stringList.Count);
                    foreach (var strList in li_stringList)
                    {
                        zPackage.Write(strList.Count);
                        foreach (var str in strList)
                        {
                            zPackage.Write(str);
                        }
                    }

                    byte[] array = zPackage.GenerateHash();
                    byte[] array2 = zPackage.GetArray();
                    FileStream fileStream = File.Create(text3);
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                    binaryWriter.Write(array2.Length);
                    binaryWriter.Write(array2);
                    binaryWriter.Write(array.Length);
                    binaryWriter.Write(array);
                    binaryWriter.Flush();
                    fileStream.Flush(flushToDisk: true);
                    fileStream.Close();
                    fileStream.Dispose();
                    if (File.Exists(text))
                    {
                        File.Delete(text);
                    }

                    File.Move(text3, text);
                }
                catch (NullReferenceException ex)
                {
                    LackingImaginationV2Plugin.Log($"failed to save");
                }
            }
        }

        [HarmonyPatch(typeof(PlayerProfile), nameof(PlayerProfile.LoadPlayerFromDisk), null)]
        public class Load_LI_StringList_Patch
        {
            public static void Postfix(PlayerProfile __instance, string ___m_filename, string ___m_playerName)
            {
                try
                {
                    if (li_stringList == null)
                    {
                        li_stringList = new List<List<string>>();
                    }
        
                    li_stringList.Clear();
        
                    li_stringList.Add(xLoxEssencePassive.LoxEaten);
                    li_stringList.Add(xWolfEssencePassive.WolfStats);
                    li_stringList.Add(xFulingShamanEssencePassive.FulingShamanStats);
                    li_stringList.Add(xSurtlingEssencePassive.SurtlingStats);
                    li_stringList.Add(xNeckEssencePassive.NeckStats);
                    li_stringList.Add(xLeechEssencePassive.LeechStats);
                    li_stringList.Add(xGreydwarfEssencePassive.GreydwarfStats);
                    li_stringList.Add(xSkeletonEssencePassive.SkeletonStats);
                    li_stringList.Add(xDraugrRot.RotStats);
                    li_stringList.Add(xDraugrEssencePassive.DraugrStats);
                    li_stringList.Add(xDraugrEliteEssencePassive.DraugrEliteStats);
                    li_stringList.Add(xSeaSerpentEssencePassive.SeaSerpentStats);
                    li_stringList.Add(xTickEssencePassive.TickStats);
                    li_stringList.Add(xStoneGolemEssencePassive.StoneGolemStats);
                    li_stringList.Add(xYagluthEssencePassive.YagluthStats);
                    li_stringList.Add(xBrennaEssencePassive.BrennaStats);
                    li_stringList.Add(xRancidRemainsEssencePassive.RancidRemainsStats);
                    li_stringList.Add(xThungrEssencePassive.ThungrStats);
                    
                    
                    
                    ZPackage zPackage = LoadStringDataFromDisk(___m_filename);
                    if (zPackage == null)
                    {
                        // No data for the string lists
                        goto LoadExit;
                    }
                    
                    int listCount = zPackage.ReadInt();
                    for (int i = 0; i < listCount; i++)
                    {
                        int strCount = zPackage.ReadInt();
                        if (li_stringList[i].Count == 0)
                        {
                            for (int j = 0; j < strCount; j++)
                            {
                                string str = zPackage.ReadString();
                                li_stringList[i].Add(str);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < strCount; j++)
                            {
                                string str = zPackage.ReadString();
                                li_stringList[i][j] = str;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Exception while loading string list: " + ex.ToString());
                }
        
                LoadExit: ;
            }
        
            private static ZPackage LoadStringDataFromDisk(string m_filename)
            {
                string text = Utils.GetSaveDataPath(FileHelpers.FileSource.Local) + "/characters/LackingI/" +
                              m_filename + "_li_strings.fch";
                FileStream fileStream = null;
                try
                {
                    fileStream = File.OpenRead(text);
                }
                catch
                {
                    // ZLog.Log("  failed to load " + text);
                    return null;
                }
        
                // if (fileStream == null)
                // {
                //     return null;
                // }
        
                byte[] data;
        
                BinaryReader binaryReader = null;
                try
                {
                    binaryReader = new BinaryReader(fileStream);
                    int count = binaryReader.ReadInt32();
                    data = binaryReader.ReadBytes(count);
                    int count2 = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(count2);
                }
                catch
                {
                    if (binaryReader != null)
                    {
                        binaryReader.Close();
                    }
                    
                    // ZLog.LogError("  error loading player data");
                    fileStream.Dispose();
                    return null;
                }
                finally
                {
                    if (binaryReader != null)
                    {
                        binaryReader.Close();
                    }
                }
        
                fileStream.Dispose();
                return new ZPackage(data);
            }
        }

        
    }
}