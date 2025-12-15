using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class FloatDamage : MonoBehaviour
{
    [SerializeField] GameObject preFab;//引用库
    // Start is called before the first frame update
    public void MakeText(Attack attacker)
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 2f;
        GameObject obj = Instantiate(preFab);//创造一个画布
        obj.SetActive(true);
        obj.GetComponent<Canvas>().worldCamera = Camera.main;//设置主摄像机
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = attacker.currentDamage.ToString();//设置数值
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().transform.position = spawnPosition;//设置位置
        //Debug.Log("成功复制");
        Destroy(obj, 1f);
    }
}
