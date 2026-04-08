using System;
using UnityEngine;

namespace TextEffects
{
    /// <summary>
    /// 文字特效标签 - 类似 SNKRX 的 TextTag
    /// 每个标签定义一个字符的更新逻辑
    /// </summary>
    [Serializable]
    public class TextTag
    {
        public string name;
        
        // 字符更新委托：修改字符的偏移、颜色、缩放等
        // c: 字符数据, dt: 时间增量, i: 字符索引, text: 完整文本
        public Action<CharData, float, int, string> update;
        
        public TextTag(string name, Action<CharData, float, int, string> updateFunc)
        {
            this.name = name;
            this.update = updateFunc;
        }
    }
    
    /// <summary>
    /// 单个字符的数据
    /// </summary>
    [Serializable]
    public class CharData
    {
        public char character;      // 字符
        public float ox, oy;        // 偏移量 (offset x, offset y)
        public Color color;         // 颜色
        public float scale;         // 缩放
        public float rotation;      // 旋转
        
        public CharData(char c)
        {
            character = c;
            ox = oy = 0;
            color = Color.white;
            scale = 1f;
            rotation = 0f;
        }
    }
}
