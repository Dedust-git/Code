using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerEnterBound : MonoBehaviour
{
    [SerializeField] private Collider2DEventSO onPlayerEnter;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //玩家进入时发生事件
        //Debug.Log(this.name+"检测到" +other.name +"进入");
        onPlayerEnter.RaiseEvent(GetComponent<Collider2D>());
    }
}
