using System;
using System.Collections.Generic;
using TextEffects;
using UnityEngine;

/// <summary>
/// 全局文字特效系统 - 类似 SNKRX 的 global_text_tags
/// 集中管理所有可用的文字特效标签
/// </summary>
public static class TextEffectSystem
{
    // 全局特效标签表
    public static Dictionary<string, TextTag> GlobalTags = new Dictionary<string, TextTag>();
    
    // 静态构造函数，初始化所有默认特效
    static TextEffectSystem()
    {
        InitializeDefaultTags();
    }
    
    /// <summary>
    /// 初始化默认的文字特效
    /// </summary>
    private static void InitializeDefaultTags()
    {
        // wavy - 文字波浪形逐个跳动 (SNKRX 原版)
        // 使用："[wavy]Hello[/wavy]"
        RegisterTag(new TextTag("wavy", (c, dt, i, text) => {
            c.oy = 2f * Mathf.Sin(4f * Time.time + i);
        }));
        
        // wavy_mid - 中等幅度波浪
        RegisterTag(new TextTag("wavy_mid", (c, dt, i, text) => {
            c.oy = 0.75f * Mathf.Sin(3f * Time.time + i);
        }));
        
        // wavy_mid2 - 较小幅度波浪
        RegisterTag(new TextTag("wavy_mid2", (c, dt, i, text) => {
            c.oy = 0.5f * Mathf.Sin(3f * Time.time + i);
        }));
        
        // wavy_lower - 微弱波浪
        RegisterTag(new TextTag("wavy_lower", (c, dt, i, text) => {
            c.oy = 0.25f * Mathf.Sin(2f * Time.time + i);
        }));
        
        // shake - 震动效果
        RegisterTag(new TextTag("shake", (c, dt, i, text) => {
            c.ox = UnityEngine.Random.Range(-1f, 1f);
            c.oy = UnityEngine.Random.Range(-1f, 1f);
        }));
        
        // pulse - 脉冲缩放
        RegisterTag(new TextTag("pulse", (c, dt, i, text) => {
            c.scale = 1f + 0.2f * Mathf.Sin(4f * Time.time + i * 0.5f);
        }));
        
        // rainbow - 彩虹色
        RegisterTag(new TextTag("rainbow", (c, dt, i, text) => {
            float hue = (Time.time * 0.5f + i * 0.1f) % 1f;
            c.color = Color.HSVToRGB(hue, 0.8f, 1f);
        }));
        
        // bounce - 弹跳
        RegisterTag(new TextTag("bounce", (c, dt, i, text) => {
            c.oy = Mathf.Abs(Mathf.Sin(3f * Time.time + i)) * 5f;
        }));
        
        // spin - 旋转
        RegisterTag(new TextTag("spin", (c, dt, i, text) => {
            c.rotation = Mathf.Sin(2f * Time.time + i) * 15f;
        }));
        
        // 颜色标签
        RegisterTag(new TextTag("red", (c, dt, i, text) => c.color = Color.red));
        RegisterTag(new TextTag("green", (c, dt, i, text) => c.color = Color.green));
        RegisterTag(new TextTag("blue", (c, dt, i, text) => c.color = Color.blue));
        RegisterTag(new TextTag("yellow", (c, dt, i, text) => c.color = Color.yellow));
        RegisterTag(new TextTag("cyan", (c, dt, i, text) => c.color = Color.cyan));
        RegisterTag(new TextTag("magenta", (c, dt, i, text) => c.color = Color.magenta));
        RegisterTag(new TextTag("white", (c, dt, i, text) => c.color = Color.white));
        RegisterTag(new TextTag("black", (c, dt, i, text) => c.color = Color.black));
    }
    
    /// <summary>
    /// 注册一个新的特效标签
    /// </summary>
    public static void RegisterTag(TextTag tag)
    {
        GlobalTags[tag.name] = tag;
    }
    
    /// <summary>
    /// 获取特效标签
    /// </summary>
    public static TextTag GetTag(string name)
    {
        return GlobalTags.TryGetValue(name, out var tag) ? tag : null;
    }
    
    /// <summary>
    /// 检查标签是否存在
    /// </summary>
    public static bool HasTag(string name)
    {
        return GlobalTags.ContainsKey(name);
    }
}
