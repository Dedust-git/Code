using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerStateBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    public float updateTime;//更新时间(视觉效果)
    private float updateTimeCounter;
    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            if (updateTimeCounter < 0)
            {
                healthDelayImage.fillAmount -= 0.75f * Time.deltaTime;
            }
            else
            {
                updateTimeCounter -= Time.deltaTime;//减去计时器
            }
        }
        else
        {
            healthDelayImage.fillAmount = healthImage.fillAmount;
        }
    }
    ///<summary>
    /// 接受Health的更变百分比
    ///</summary>
    ///<param name="percentage">百分比:Current/Max</param> 
    public void OnHealthChanged(float percentage)
    {
        updateTimeCounter = updateTime;
        healthImage.fillAmount = percentage;
    }
    ///<summary>
    /// 接受Power的更变百分比
    ///</summary>
    ///<param name="percentage">百分比:Current/Max</param> 
    public void OnPowerChanged(float percentage)
    {
        powerImage.fillAmount = percentage;
    }
}
