using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialValue : MonoBehaviour
{
    [SerializeField] private Material material;
    private float amount = 0;
    private float min = 0f , max = 0.2f; 
    private bool isUp = true;

    private void Start() {
        amount = min;
        isUp = true;
    }
    
    private void Update() 
    {
        if(isUp)
        {
            amount += Time.deltaTime * 0.5f;
            if (amount >= max)
                isUp = false;
        }
        else
        {
            amount -= Time.deltaTime * 0.5f;
            if (amount <= min)
                isUp = true;
        }

        material.SetFloat("_MetalFade", amount);
    }
}
