using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskData : MonoBehaviour
{
    public enum DiskType { normal, heavy, fast };

    public DiskType type;
    public int score;
    public Color color;
    public Vector3 scale;

    private void Awake()                                                                // 初始化飞碟
    {
        gameObject.GetComponent<Renderer>().material.color = color;                     // 颜色
        gameObject.transform.localScale = scale * Random.Range( 0.85f, 1.15f );         // 大小浮动
    }
}
