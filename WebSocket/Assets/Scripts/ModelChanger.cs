
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelChanger : MonoBehaviour
{
    [SerializeField] private List<Mesh> mesh;
    [SerializeField] private MeshFilter meshFilter;
    private Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = (mesh.Count - 1);
    }

    public void ChangeMesh()
    {
        meshFilter.mesh = mesh[((int)slider.value)];
    }
}
