#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using Assets.VoxelOptimizer;

public class VoxelOptimizer : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        CheckForPlyFiles(importedAssets);
    }

    static void CheckForPlyFiles(string[] assets)
    {
        foreach(var path in assets)
        {
            var subpath = path.Split('/');
            if (subpath[subpath.Length - 1].Contains(".ply")) ProcessPlyFile(path);
        }
    }

    static void WriteTextFile(string path, string content)
    {
        var file = File.CreateText(path);
        file.Write(content);
        file.Close();
    }

    static void WritePng(string path, Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        var file = File.Create(path);
        file.Write(bytes, 0, bytes.Length);
        file.Close();
    }

    static string ChangeExtension(string path, string ext)
    {
        return path.Substring(0, path.LastIndexOf(".ply")) + ext;
    }





    static void ProcessPlyFile(string path)
    {
        Debug.Log("updating: " + path);

        // convert .ply -> .png, .mtl, .obj
        var model = VoxelModelPly.LoadModel(path);



        // ---- EXPORT
        // export texture
        string texPath = ChangeExtension(path, ".png");
        WritePng(texPath, model.tex);
        texPath = texPath.Split('/')[texPath.Split('/').Length - 1];

        // export material
        string mtlPath = ChangeExtension(path, ".mtl");
        WriteTextFile(mtlPath, model.ToMtl(texPath));

        // export obj
        WriteTextFile(ChangeExtension(path, ".obj"), model.ToObj(mtlPath));
    }




    // TODO : Removes double verticies
    static void mergeVerts()
    {
        // merge only if position, color and normal are all equal
    }


    // TODO : Optimizes mesh 
    static void optimize(VoxelModelPly model)
    {
        // generate edgecount per vert [2,4,6,8]
        int[] edgeCount = new int[model.numVerts];
        for(var i = 0; i < model.numFaces; i++)
        {
            edgeCount[model.faces[i*3]] += 2;
            edgeCount[model.faces[i*3+1]] += 2;
            edgeCount[model.faces[i*3+2]] += 2;
        }


        // use only significant verts edgeCount = [2,4]
        // vertex part of (numEdges / 2) triangles. 1 & 2 means vertex is significant
        for(var i = 0; i < edgeCount.Length; i++)
        {
            Debug.Log(edgeCount[i]);
        }

        // generate outline per polygon


        // triangulate verts 
    
    }

}
#endif