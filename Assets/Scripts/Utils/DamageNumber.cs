using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    private float floatSpeed;
    void Start()
    {
        floatSpeed = Random.Range(0.5f, 1.5f);
        Destroy (gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * floatSpeed;
    }

    public void SetValue (int value){
        damageText.text = value.ToString();
    }
}
