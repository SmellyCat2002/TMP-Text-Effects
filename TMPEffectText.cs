using System.Collections.Generic;
using TextEffects;
using TMPro;
using UnityEngine;

namespace TextEffects
{
    /// <summary>
    /// TMP 特效文字组件 - 使用 TextMeshPro 顶点动画
    /// 性能更好，只有一个 GameObject
    /// 
    /// 使用示例：
    /// var text = GetComponent<TMPEffectText>();
    /// text.SetText("[wavy_mid, yellow]SHOP[/wavy_mid] - Gold: [yellow]100");
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TMPEffectText : MonoBehaviour
    {
        [Header("文本设置")]
        [TextArea(3, 10)]
        public string rawText = "[wavy,rainbow,pulse]Hello World[/wavy]";
        
        // 解析后的数据
        private List<CharData> charDataList = new List<CharData>();
        private List<List<string>> charTags = new List<List<string>>();
        
        // TMP 组件
        private TMP_Text tmpText;
        
        // 原始顶点位置（用于计算偏移）
        private Vector3[][] originalVertices;
        
        void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
        }
        
        void Start()
        {
            ParseText();  // 内部会直接设置 tmpText.text
        }
        
        void Update()
        {
            UpdateMesh();
        }
        
        /// <summary>
        /// 设置新文本
        /// </summary>
        public void SetText(string newText)
        {
            rawText = newText;
            ParseText();  // 内部会直接设置 tmpText.text
        }
        
        /// <summary>
        /// 解析带标签的文本（与 EffectText 相同）
        /// </summary>
        private void ParseText()
        {
            charDataList.Clear();
            charTags.Clear();
            
            string text = rawText;
            int i = 0;
            List<string> currentTags = new List<string>();
            
            while (i < text.Length)
            {
                // 检测标签开始 [tag]
                if (text[i] == '[')
                {
                    int closeBracket = text.IndexOf(']', i);
                    if (closeBracket > i)
                    {
                        string tagContent = text.Substring(i + 1, closeBracket - i - 1);
                        
                        // 检查是否是结束标签 [/tag]
                        if (tagContent.StartsWith("/"))
                        {
                            string endTag = tagContent.Substring(1);
                            currentTags.Remove(endTag);
                        }
                        else
                        {
                            // 解析多个标签 [tag1, tag2]
                            string[] tags = tagContent.Split(',');
                            foreach (var tag in tags)
                            {
                                string trimmedTag = tag.Trim();
                                if (!string.IsNullOrEmpty(trimmedTag))
                                {
                                    currentTags.Add(trimmedTag);
                                }
                            }
                        }
                        
                        i = closeBracket + 1;
                        continue;
                    }
                }
                
                // 普通字符
                CharData charData = new CharData(text[i]);
                charData.color = Color.white;
                charDataList.Add(charData);
                charTags.Add(new List<string>(currentTags));
                
                i++;
            }
            
            // 构建纯文本并直接设置给 TMP
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var cd in charDataList)
            {
                sb.Append(cd.character);
            }
            tmpText.text = sb.ToString();
        }
        
        /// <summary>
        /// 更新 TMP 网格 - 核心流程
        /// </summary>
        private void UpdateMesh()
        {
            tmpText.ForceMeshUpdate();
            
            var textInfo = tmpText.textInfo;
            int charCount = textInfo.characterCount;
            
            if (charCount == 0) return;
            
            for (int i = 0; i < charCount && i < charDataList.Count; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                
                // 1. 重置
                ResetCharData(charDataList[i]);
                
                // 2. 应用标签效果（开发者关心的部分）
                ApplyTags(charDataList[i], i);
                
                // 3. 应用到顶点（底层细节）
                ApplyToVertices(i, textInfo);
            }
            
            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
        
        /// <summary>
        /// 重置字符数据 - 每帧从基准开始
        /// </summary>
        private void ResetCharData(CharData data)
        {
            data.ox = 0;
            data.oy = 0;
            data.scale = 1f;
            data.rotation = 0f;
        }
        
        /// <summary>
        /// 应用标签效果
        /// </summary>
        private void ApplyTags(CharData data, int index)
        {
            if (index >= charTags.Count) return;
            
            foreach (string tagName in charTags[index])
            {
                TextTag tag = TextEffectSystem.GetTag(tagName);
                tag?.update?.Invoke(data, Time.deltaTime, index, tmpText.text);
            }
        }
        
        /// <summary>
        /// 将 CharData 应用到 TMP 顶点（底层实现）
        /// </summary>
        private void ApplyToVertices(int charIndex, TMP_TextInfo textInfo)
        {
            var charInfo = textInfo.characterInfo[charIndex];
            var charData = charDataList[charIndex];
            
            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            
            Vector3[] verts = textInfo.meshInfo[matIndex].vertices;
            Color32[] colors = textInfo.meshInfo[matIndex].colors32;
            
            // 计算中心点
            Vector3 center = (verts[vertIndex] + verts[vertIndex + 2]) * 0.5f;
            
            // 变换4个顶点
            for (int j = 0; j < 4; j++)
            {
                Vector3 pos = verts[vertIndex + j];
                
                // 相对中心变换
                pos = TransformRelativeToCenter(pos, center, charData);
                
                verts[vertIndex + j] = pos;
                colors[vertIndex + j] = charData.color;
            }
        }
        
        /// <summary>
        /// 相对于中心点进行变换
        /// </summary>
        private Vector3 TransformRelativeToCenter(Vector3 pos, Vector3 center, CharData data)
        {
            // 移到原点
            pos -= center;
            
            // 缩放
            pos *= data.scale;
            
            // 旋转
            pos = Rotate2D(pos, data.rotation);
            
            // 移回 + 偏移
            pos += center;
            pos.x += data.ox;
            pos.y += data.oy;
            
            return pos;
        }
        
        /// <summary>
        /// 2D旋转
        /// </summary>
        private Vector3 Rotate2D(Vector3 pos, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            
            return new Vector3(
                pos.x * cos - pos.y * sin,
                pos.x * sin + pos.y * cos,
                pos.z
            );
        }
    }
}
