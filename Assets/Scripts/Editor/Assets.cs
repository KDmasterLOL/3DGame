using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using Object = UnityEngine.Object;

public class Assets : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var asset in importedAssets)
            ProcessAssetLabels(asset);

        foreach (var asset in movedAssets)
            ProcessAssetLabels(asset);
    }

    private static void ProcessAssetLabels(string assetPath)
    {
        Debug.Log(assetPath);
        var labels = ParseAssetPath(assetPath);

        

        var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        if (obj)
        {

            if (labels.Count == 0)
            {
                AssetDatabase.ClearLabels(obj);
                return;
            }

            var oldLabels = AssetDatabase.GetLabels(obj);
            var labelsArray = new string[] { string.Join('-', labels) };
            foreach (var lab in labelsArray)
                Debug.Log(lab);

            if (HaveLabelsChanged(oldLabels, labelsArray))
            {
                AssetDatabase.SetLabels(obj, labelsArray);
                Debug.Log("Labels changed");
            }
        }
    }

    private static List<string> ParseAssetPath(string path)
    {
        var labels = new List<string>();

        foreach (var label in path.Split("/"))
            labels.Add(label);

        var last = labels[^1];
        var extension = last[last.LastIndexOf(".")..];

        labels[^1] = last.Replace(extension, ExtToLab(extension));

        Debug.Log(extension);


        return labels;
    }

    private static string ExtToLab(string ext) => ext switch
    {
        ".mat" => "material",
        _ => ext,
    };


    private static bool HaveLabelsChanged(string[] oldLabels, string[] newLabels)
    {
        return oldLabels.SequenceEqual(newLabels) == false;
    }
}
