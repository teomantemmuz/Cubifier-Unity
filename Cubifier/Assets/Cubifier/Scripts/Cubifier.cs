using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quality {VeryLow, Low, Medium, High, VeryHigh}

public class Cubifier : MonoBehaviour
{
    public Texture2D sourceImage;//source image.
    private Texture2D image;
    [Space]
    public Material mat;//material attached to the gameobject.

    public GameObject prefab;//if you want to use your own prefab instead of primitive cubes.
    [Space]
    public bool hasRigidbody = false;//if true gameobject has a rigidbody.
    [Space]
    public Transform cubeHolder;// parent object of the cubes.
    [Space]
    public float scaleFactor = 1;
    public float alphaCutOut = 0.05f;//if a pixel has less alpha then cutOutPoint it won't be created.
    [Header("Higher quality decreases performance")]
    public Quality quality;

    void Start()
    {
        Cubify();
    }
    
    /*
     * Cubify funtion takes 2D Texture as input and turns it to a 3D gameobject which made from cubes
     */

    [ContextMenu("Cubify")]
    void Cubify()
    {
        SetQuality();
        Color[] pixels = image.GetPixels();

        GameObject holder;
        if (!cubeHolder)
            holder = new GameObject();
        else
            holder = cubeHolder.gameObject;
        GameObject[] imageWall = new GameObject[pixels.Length];

        for (int index = 0; index < pixels.Length; index++)
        {
            if(CheckPixel(pixels[index]))
            {
                if(prefab)
                {
                    imageWall[index] = Instantiate(prefab);
                }

                else
                {
                    imageWall[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }


                imageWall[index].transform.position = new Vector3(index % image.width, index / image.width, 0);
                imageWall[index].transform.parent = holder.transform;


                MeshRenderer mr = imageWall[index].GetComponent<MeshRenderer>();
                mr.material = mat;
                mr.material.color = pixels[index];

                if (hasRigidbody)
                    imageWall[index].AddComponent<Rigidbody>();
            }
        }

        CenterImage(holder.transform);

        holder.transform.localScale = GetVectorScale(scaleFactor);
    }

    bool CheckPixel(Color pixel)
    {
        if (pixel.a < alphaCutOut)
            return false;
        else
            return true;
    }

    void CenterImage(Transform holder)
    {
        Vector3 center = Vector3.zero; 

        //Find center.
        foreach (Transform item in holder)
        {
            center += item.localPosition;
        }
        center /= holder.childCount;

        //Subtract center from every cube.
        foreach (Transform item in holder)
        {
            item.localPosition -= center;
        }
    }

    Vector3 GetVectorScale(float scale)
    {
        return Vector3.one * scale;
    }

    void SetQuality()
    {
        if (quality == Quality.VeryLow)
            image = TextureScale.Bilinear(sourceImage, 32, 32);
        else if(quality == Quality.Low)
            image = TextureScale.Bilinear(sourceImage, 48, 48);
        else if (quality == Quality.Medium)
            image = TextureScale.Bilinear(sourceImage, 64, 64);
        else if (quality == Quality.High)
            image = TextureScale.Bilinear(sourceImage, 128, 128);
        else if (quality == Quality.VeryHigh)
            image = TextureScale.Bilinear(sourceImage, 256, 256);
    }
}
