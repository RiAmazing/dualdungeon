using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Assets.VoxelOptimizer
{
    public class VoxelModelPly
    {

        public int numVerts = 0;
        public int numFaces = 0;

        public Vector3[] vert;
        public Vector3[] norm;
        public int[] vertColor;
        public List<int> colorPalette;
        public Vector2[] uv;
        public int[] faces;

        // TODO : colorPalete
        int tex_size = 1024;
        public Texture2D tex;

        public static VoxelModelPly LoadModel(string path)
        {
            var model = new VoxelModelPly();

            var headerVertex = "element vertex";
            var headerFace = "element face";
            var headerEnd = "end_header";

            var numVerts = -1;
            var numFaces = -1;


            StreamReader file = new StreamReader(path, Encoding.Default);
            string line = "";

            // --- parse header
            for (var i = 0; i < 100; i++)
            {
                line = file.ReadLine();
                if (line.IndexOf(headerVertex) != -1) numVerts = Int32.Parse(line.Substring(line.IndexOf(headerVertex) + headerVertex.Length));
                if (line.IndexOf(headerFace) != -1) numFaces = Int32.Parse(line.Substring(line.IndexOf(headerFace) + headerFace.Length));
                if (line.IndexOf(headerEnd) != -1) break;
            }

            model.numVerts = numVerts;
            model.numFaces = numFaces;
            model.vert = new Vector3[numVerts];
            model.vertColor = new int[numVerts * 3];
            model.colorPalette = new List<int>();
            model.norm = new Vector3[numVerts];
            model.uv = new Vector2[numVerts];
            model.faces = new int[numFaces * 3];

            // ---- parse verticies
            string[] dataArray = null;
            for (var i = 0; i < numVerts; i++)
            {
                line = file.ReadLine();
                dataArray = line.Split(' ');
                model.vert[i] = new Vector3(float.Parse(dataArray[0]), float.Parse(dataArray[1]), float.Parse(dataArray[2]));
                model.vertColor[i * 3] = Int32.Parse(dataArray[3]);
                model.vertColor[i * 3 + 1] = Int32.Parse(dataArray[4]);
                model.vertColor[i * 3 + 2] = Int32.Parse(dataArray[5]);
            }


            // ---- parse faces
            for (var i = 0; i < numFaces; i++ )
            {
                line = file.ReadLine();
                dataArray = line.Split(' ');
                model.faces[i * 3] = Int32.Parse(dataArray[1]);
                model.faces[i * 3 + 1] = Int32.Parse(dataArray[2]);
                model.faces[i * 3 + 2] = Int32.Parse(dataArray[3]);
            }

            file.Close();

            model.CalculateNormals();
            model.generateTexture();

            return model;
        }

        void CalculateNormals()
        {
            for(var i = 0; i < numFaces; i++)
            {
                var v1 = faces[i*3]; var v2 = faces[i*3+1]; var v3 = faces[i*3+2];
                norm[v1] = norm[v2] = norm[v3] = Vector3.Cross(vert[v2] - vert[v1], vert[v3] - vert[v1]);
            }

        }

        void generateTexture()
        {
            int numCol = 0;
            bool added = false;
            for (var i = 0; i < numVerts; i++ )
            {
                added = false;

                // check if color already in color palette
                for(var j = 0; j < numCol; j++)
                {
                    if (vertColor[i * 3] == colorPalette[j * 3] &&
                        vertColor[i * 3 + 1] == colorPalette[j * 3 + 1] &&
                        vertColor[i * 3 + 2] == colorPalette[j * 3 + 2])
                    {
                        added = true;
                        vertColor[i * 3] = j;
                        break;
                    }

                }

                // if not in palette, add to palette
                if (!added)
                {
                    // save col in palette list
                    colorPalette.Add(vertColor[i * 3]);
                    colorPalette.Add(vertColor[i * 3 + 1]);
                    colorPalette.Add(vertColor[i * 3 + 2]);

                    vertColor[i * 3] = numCol; // set first off vertCol triple to col index
                    numCol++;
                }
            }

            tex_size = Mathf.FloorToInt(Mathf.Sqrt(colorPalette.Count)) + 1;
            tex = new Texture2D(tex_size, tex_size, TextureFormat.RGB24, false);

            for (var i = 0; i < numVerts; i++)
            {
                var u = i % tex_size;
                var v = Mathf.FloorToInt(i / tex_size);
                var col = new Color(vertColor[i * 3] / 255, vertColor[i * 3 + 1] / 255, vertColor[i * 3 + 2] / 255);
                tex.SetPixel(v, u, col);

                uv[i] = new Vector2((float)u / (float)tex_size, (float)v / (float)tex_size);
            }
        }

        public string ToMtl(string texPath)
        {
            string mtl = "newmtl material0" +
                "\nKa 1.000000 1.000000 1.000000" +
                "\nKd 1.000000 1.000000 1.000000" +
                "\nKs 0.000000 0.000000 0.000000" +
                "\nTr 1.000000" +
                "\nillum 1" +
                "\nNs 0.000000" +
                "\nmap_Kd " + texPath;

            return mtl;
        }

        public string ToObj(string mtlPath)
        {
            string obj = "";

            obj += "mtllib " + mtlPath;

            for (var i = 0; i < numVerts; i++)
            {
                obj += "\nv " + vert[i].x + " " + vert[i].y + " " + vert[i].z;
            }

            for (var i = 0; i < numVerts; i++)
            {
                obj += "\nvt " + uv[i].x + " " + uv[i].y;
            }

            for (var i = 0; i < numVerts; i++)
            {
                obj += "\nvn " + norm[i].x + " " + norm[i].y + " " + norm[i].z;
            }

            obj += "\nusemtl material0";

            for (var i = 0; i < faces.Length; i++ )
            {
                if(i % 3 == 0) obj += "\nf";
                obj += " " + (faces[i] +1);
            }

            return obj;
        }

    }
}
