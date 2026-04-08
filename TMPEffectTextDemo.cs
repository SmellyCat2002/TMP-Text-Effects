using TextEffects;
using UnityEngine;

/// <summary>
/// TMPEffectText 演示脚本
/// </summary>
public class TMPEffectTextDemo : MonoBehaviour
{
    [Header("TMP 特效文本对象")]
    public TMPEffectText shopText;
    public TMPEffectText partyText;
    public TMPEffectText goldText;
    
    [Header("动态更新")]
    public float updateInterval = 1f;
    private float timer;
    private int gold = 100;
    
    void Start()
    {
        // 示例1：商店标题
        if (shopText != null)
        {
            shopText.SetText("[wavy_mid]SHOP[/wavy_mid] - Gold: [yellow]100");
        }
        
        // 示例2：队伍信息
        if (partyText != null)
        {
            partyText.SetText("[wavy_mid]PARTY[/wavy_mid] [cyan]3/4[/cyan]");
        }
        
        // 示例3：金币显示 - 多种特效组合
        if (goldText != null)
        {
            goldText.SetText("[rainbow]★[/rainbow] [wavy]GOLD:[/wavy] [yellow][shake]999[/shake][/yellow] [rainbow]★[/rainbow]");
        }
    }
    
    void Update()
    {
        // 动态更新金币数量
        timer += Time.deltaTime;
        if (timer >= updateInterval && goldText != null)
        {
            timer = 0;
            gold = Random.Range(50, 999);
            goldText.SetText($"[rainbow]★[/rainbow] [wavy]GOLD:[/wavy] [yellow][shake]{gold}[/shake][/yellow] [rainbow]★[/rainbow]");
        }
        
        // 按键切换不同效果
        if (Input.GetKeyDown(KeyCode.Alpha1))
            shopText?.SetText("[wavy]WAVY[/wavy] - 波浪效果");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            shopText?.SetText("[shake]SHAKE[/shake] - 震动效果");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            shopText?.SetText("[rainbow]RAINBOW[/rainbow] - 彩虹效果");
        if (Input.GetKeyDown(KeyCode.Alpha4))
            shopText?.SetText("[pulse]PULSE[/pulse] - 脉冲效果");
        if (Input.GetKeyDown(KeyCode.Alpha5))
            shopText?.SetText("[bounce]BOUNCE[/bounce] - 弹跳效果");
        if (Input.GetKeyDown(KeyCode.Alpha6))
            shopText?.SetText("[spin]SPIN[/spin] - 旋转效果");
        if (Input.GetKeyDown(KeyCode.Alpha0))
            shopText?.SetText("[wavy_mid]SHOP[/wavy_mid] - [yellow]Gold: 100[/yellow]");
    }
}
