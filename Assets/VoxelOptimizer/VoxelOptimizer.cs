#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets.VoxelOptimizer;

public class VoxelOptimizer : AssetPostprocessor
{

    static List<string> generatedTextures = new List<string>();

    // LISTEN FOR UNITY ASSET CHANGED EVENT
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        CheckForPlyFiles(importedAssets);
    }

    // if asset is .ply file process it
    static void CheckForPlyFiles(string[] assets)
    {
        foreach(var path in assets)
        {
            var subpath = path.Split('/');
            if (subpath[subpath.Length - 1].Contains(".ply")) ProcessPlyFile(path);
        }
    }

    void OnPreprocessTexture()
    {
        // check if new texture was created by voxelpotimizer
        if(generatedTextures.Contains(assetPath)) 
        {
            generatedTextures.Remove(assetPath);

            TextureImporter importer = assetImporter as TextureImporter;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            importer.filterMode = FilterMode.Point;
            Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
            if (asset) {
                EditorUtility.SetDirty(asset);
            }
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

    static string GetFilename(string path)
    {
        // get filename
        var fname = path.Split('/')[path.Split('/').Length - 1];
        // remove file extensions
        while(fname.IndexOf(".mc") != -1 || fname.IndexOf(".ply") != -1)
        {
            fname = fname.Substring(0, fname.LastIndexOf("."));
        }
        return fname;
    }
    
    static string ChangeExtension(string path, string ext)
    {
        return path.Substring(0, path.LastIndexOf(".ply")) + ext;
    }


    static void ProcessPlyFile(string path)
    {
        Debug.Log("importing: " + path);

        // import .ply model
        var model = VoxelModelPly.LoadModel(path);

        // TODO : optimize

        // ---- EXPORT
        // export texture
        string texPath = ChangeExtension(path, ".png");
        WritePng(texPath, model.tex);
        generatedTextures.Add(texPath);
        Debug.Log("generated " + texPath);
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