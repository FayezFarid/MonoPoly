using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void AddLandTileInstance(this Dictionary<LandTypes, List<LandTitleInstance>> ownedLandWithSameType,
        LandTitleInstance landTitleInstance)
    {
        LandTypes landType = landTitleInstance.LandDef.LandType;
        if (ownedLandWithSameType.ContainsKey(landType))
        {
            if (ownedLandWithSameType[landType].Contains(landTitleInstance))
            {
                SpicyHarissaLogger.LogErrorCritical(
                    $"Adding LandTile instance that already Exist Name [{landTitleInstance.LandDef.name}] Type [{landType}]");
                return;
            }

            ownedLandWithSameType[landType].Add(landTitleInstance);
            return;
        }

        List<LandTitleInstance> landTilesInstances = new List<LandTitleInstance>();

        landTilesInstances.Add(landTitleInstance);

        ownedLandWithSameType.Add(landType, landTilesInstances);
    }

    public static void RemoveLandTileInstance(this Dictionary<LandTypes, List<LandTitleInstance>> ownedLandWithSameType,
        LandTitleInstance landTitleInstance)
    {
        LandTypes landType = landTitleInstance.LandDef.LandType;
        if (ownedLandWithSameType.ContainsKey(landType))
        {
            if (!ownedLandWithSameType[landType].Contains(landTitleInstance))
            {
                SpicyHarissaLogger.LogErrorCritical(
                    $"Removing LandTile instance that didn't Exist Name [{landTitleInstance.LandDef.name}] Type [{landType}]");
                return;
            }

            ownedLandWithSameType[landType].Remove(landTitleInstance);
           
        }
        else
        {
            SpicyHarissaLogger.LogErrorCritical(
                $"Trying to Remove land which key does not exist[{landTitleInstance.LandDef.name}] Type [{landType}]");
            
        }
    }

    /// <summary>
    /// Incremante Index according to total number if index is equal or high it will reset back to 0
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="totalNumber"> when using count add -1 because it uses ==</param>
    /// <returns></returns>
    public static int GetNextIndex(int currentIndex, int totalNumber)
    {
        if (currentIndex == totalNumber)
        {
            return 0;
        }

        return currentIndex + 1;
    }
    
    public static Vector3 GetDirection(this Vector3 playerPosition,Vector3 tileTransform)
    {
        var heading = tileTransform - playerPosition;
        var distance = heading.magnitude;
        return  heading / distance;
    }

   
}