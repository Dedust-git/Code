using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class FloatEvent : MonoBehaviour
{
    [SerializeField] GameObject preFab;//引用库
    // Start is called before the first frame update
    public void MakeText(Vector2 textPosition,string texts)
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 2f;
        GameObject obj = Instantiate(preFab);//创造一个画布
        obj.SetActive(true);
        obj.GetComponent<Canvas>().worldCamera = Camera.main;//设置主摄像机
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = texts;
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 2;
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().transform.position = textPosition;
        //Debug.Log("成功复制");
        Destroy(obj, 2f);
    }
}
