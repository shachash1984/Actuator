using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FBXScaleFix : AssetPostprocessor
{
    public void OnPreProcessModel()
    {
        ModelImporter modelImporter = (ModelImporter)assetImporter;
        modelImporter.globalScale = 1;
    }
}
