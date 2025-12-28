using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] private ZoneEventSO onPlayerEnter;
    public Collider2D bound; 
    public string ZoneName; //这个区域的名字

    public bool isImportant = false;//是否重要，决定是否会广播

    private void OnEnable()
    {
        bound = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //玩家进入时发生事件
        onPlayerEnter.RaiseEvent(this);
        
    }
}
