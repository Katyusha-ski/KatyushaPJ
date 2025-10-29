# 🎨 GRAPHICS PROGRAMMING ROADMAP - PHẦN PHỤ HỖ TRỢ UNITY

Context: Học Graphics Programming như skill phụ để nâng cao Unity development
Timeline: 6-12 tháng (parallel với Unity main track)
Cường độ: 3-5 giờ/tuần (không ảnh hưởng roadmap chính)

---

## 📊 TẦM NHÌN TỔNG QUAN

Graphics Programming trong Unity Context:

UNITY DEVELOPER LEVELS:
- Level 1: Gameplay Programmer (bạn ở đây)
  - Dùng built-in shaders, particle systems
- Level 2: Technical Artist (hybrid)
  - Custom shaders, ShaderGraph, VFX Graph
- Level 3: Graphics Programmer (advanced)
  - Custom render pipeline, low-level optimization

Bạn đang có: ShaderLab 25.5% + HLSL 5.3% → Foundation tốt!

---

## 🎯 LỢI ÍCH KHI HỌC GRAPHICS

Career Benefits:

| Skill Level | Job Opportunities | Salary Impact |
|---|---|---|
| No Graphics | Gameplay Programmer | Baseline |
| ShaderGraph + Basic Shaders | Technical Artist, Senior Unity Dev | +20-30% |
| HLSL + Custom Effects | Graphics Programmer, Lead Dev | +40-60% |
| Render Pipeline Expert | Graphics Engineer, AAA Studios | +80-100% |

Trong Portfolio:

Trước (chỉ Gameplay):

> "Developed 2D Action RPG with inventory system" → Interviewer: "OK, standard stuff"

Sau (+ Graphics):

> "Developed 2D Action RPG with custom shaders:
- Dissolve effect with noise textures
- Rim lighting for hit feedback
- Post-processing stack for stylized look"

→ Interviewer: "😍 Impressive technical depth!"

---

## 🗺️ ROADMAP TỔNG QUAN (6-12 THÁNG)

- Phase 1: Foundations (Tháng 1-2)
  - Math for Graphics (vectors, matrices, spaces)
  - Rendering pipeline overview
  - Unity ShaderGraph (no code)
- Phase 2: ShaderLab Basics (Tháng 3-4)
  - Vertex & Fragment shaders
  - Texture sampling
  - Lighting basics
- Phase 3: HLSL Intermediate (Tháng 5-6)
  - Custom lighting models
  - Normal mapping, parallax
  - Animated shaders (dissolve, glitch, water)
- Phase 4: Advanced Effects (Tháng 7-9)
  - Post-processing effects
  - Compute shaders
  - Custom render features (URP)
- Phase 5: Portfolio Projects (Tháng 10-12)
  - 3-5 shader showcase projects
  - Technical art portfolio
  - Breakdown videos

---

## 📚 CHI TIẾT TỪNG PHASE

### PHASE 1: FOUNDATIONS (Tháng 1-2)

#### Tuần 1-2: Math for Graphics

Mục tiêu: Hiểu toán đằng sau graphics (không cần chứng minh)
Topics cần nắm:

1) Vectors (2-3 ngày)

Concepts:
- ✅ Vector operations: add, subtract, scale
- ✅ Dot product (projection, angle)
- ✅ Cross product (perpendicular vector)
- ✅ Magnitude, normalization

Ứng dụng trong Unity:
- Dot product: Check if object is in front/behind camera
- Cross product: Calculate surface normals
- Normalize: Direction vectors for movement

Tài nguyên:
- 📺 3Blue1Brown: "Essence of Linear Algebra"
- 📖 Unity Manual: Vector Cookbook
- 🎮 Practice: Visualize vectors trong Unity với Debug.DrawLine()

Deliverable:

```csharp
// VectorPractice.cs - Vẽ dot product visualization
void OnDrawGizmos()
{
    Vector3 forward = transform.forward;
    Vector3 toTarget = (target.position - transform.position).normalized;
    
    float dot = Vector3.Dot(forward, toTarget);
    
    // Dot = 1: same direction (green)
    // Dot = 0: perpendicular (yellow)  
    // Dot = -1: opposite (red)
    Gizmos.color = Color.Lerp(Color.red, Color.green, (dot + 1) / 2);
    Gizmos.DrawLine(transform.position, target.position);
}
```

2) Matrices & Transformations (2-3 ngày)

Concepts:
- ✅ Translation, Rotation, Scale (TRS)
- ✅ Matrix multiplication order (why SRT?)
- ✅ Object space → World space → View space → Clip space
- ✅ Projection matrices (orthographic vs perspective)

Ứng dụng:
- Hiểu transform hierarchy
- Vertex shader transformations
- Camera frustum culling

Tài nguyên:
- 📺 "Math for Game Programmers: Understanding Homogeneous Coordinates"
- 📖 Cheat sheet: Transformation matrices
- 🎮 Unity: Matrix4x4 API exploration

Deliverable: Viết script vẽ coordinate axes của object (local vs world). Giải thích tại sao vertex shader cần UNITY_MATRIX_MVP.

3) Color Spaces & Gamma (1-2 ngày)

Concepts:
- ✅ RGB, HSV, HDR
- ✅ Linear vs Gamma space
- ✅ Why artists work in Gamma but shaders in Linear

Ứng dụng:
- Color blending in shaders
- Post-processing accuracy
- Why PBR workflows use Linear space

Quick Reference:

```csharp
// Linear space (physics correct)
Color blended = colorA * colorB; 

// Gamma space (perceptual)
Color blended = Color.Lerp(colorA, colorB, 0.5f);
```

#### Tuần 3-4: Rendering Pipeline

Mục tiêu: Hiểu flow từ 3D model → pixels trên màn hình

1) Graphics Pipeline Overview (2 ngày)

CPU Side:
1. Scene setup (objects, lights, camera)
2. Culling (frustum, occlusion)
3. Batching (reduce draw calls)
4. Submit draw calls to GPU

GPU Side:
5. Vertex Shader (transform vertices)
6. Rasterization (convert triangles → pixels)
7. Fragment/Pixel Shader (calculate pixel color)
8. Output Merger (depth test, blending)

Visualization:
- 📺 "A trip through the Graphics Pipeline" (blog series by Fabian Giesen)
- 🎮 Unity Frame Debugger hands-on

Bài tập: Mở Frame Debugger trong game của bạn, screenshot từng draw call, viết document: "Rendering breakdown của KatyushaPJ".

2) Unity ShaderGraph (5-7 ngày)

Tại sao học ShaderGraph trước code:
- ✅ Visual → dễ hiểu concepts
- ✅ See results ngay lập tức
- ✅ Learn nodes = learn shader operations
- ✅ Export code để xem HLSL generated

Projects (1 project/ngày):
- Day 1: Hello ShaderGraph — shader đơn giản: Color gradient (UV, Gradient, Color)
- Day 2: Texture Sampling — Texture shader với Tiling/Offset control
- Day 3: Vertex Animation — Wave effect (Time, Sine, Position)
- Day 4: Dissolve Effect — Dissolve với noise texture
- Day 5: Rim Lighting — Fresnel
- Day 6-7: Mini Project — Stylized water shader

Deliverable: 6 shaders working trong Unity, screenshots + notes về mỗi node, export code và highlight HLSL sections.

---

### PHASE 2: SHADERLAB BASICS (Tháng 3-4)

#### Tuần 1-2: Your First Shader

Mục tiêu: Viết shader đầu tiên bằng code

1) ShaderLab Structure (1 ngày)

```hlsl
Shader "Custom/MyFirstShader"
{
    Properties  // Inspector properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader  // GPU instructions
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata  // Input from mesh
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f  // Vertex → Fragment
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}
```

Practice: Type lại shader trên (không copy-paste), modify _Brightness, test with material.

2) Vertex Shader Magic (3-4 ngày)

Concepts: vertex shader runs per-vertex, interpolation to fragment, use vertex deformation for cheap animation.

Project examples: Pulsing, Flag Wave, Explode Mesh (see attachment for HLSL snippets).

Deliverable: 3 vertex shaders working, video demo effects.

3) Fragment Shader Effects (Tuần 3-4)

Concepts: Fragment (per-pixel) for color calculations. Prefer vertex where possible for perf.

Projects: Color Grading, UV Manipulation, Multi-Texture Blending.

Deliverable: 3 fragment shaders, understanding when to use vertex vs fragment.

---

### PHASE 3: HLSL INTERMEDIATE (Tháng 5-6)

#### Tuần 1-2: Lighting Basics

Goals: Understand PBR basics; implement diffuse and specular models; create toon shader.

Examples and HLSL snippets included in attachment — implement and compare with Standard shader.

#### Tuần 3-4: Texture Magic

Topics: Normal mapping, Parallax mapping, Triplanar mapping. Deliverable: working examples and comparisons.

---

### PHASE 4: ADVANCED EFFECTS (Tháng 7-9)

#### Tuần 1-3: Animated Shaders

Examples: Dissolve, Hologram, Water shader (portfolio piece). Focus on combining normal maps, fresnel, depth fade, foam edges.

#### Tuần 4-6: Post-Processing

Implement full-screen effects via OnRenderImage / Graphics.Blit. Effects: Grayscale, Blur, Sobel edge detection, Vignette.

Deliverable: 5 post-processing effects, toggle on/off with hotkey, before/after screenshots.

#### Tuần 7-9: Compute Shaders (Advanced)

When to use: GPU particles (10k+), fluid sim, procedural generation. Example compute shader and C# dispatch snippet included in attachment.

Deliverable (optional): GPU particle system, performance comparison CPU vs GPU.

---

### PHASE 5: PORTFOLIO PROJECTS (Tháng 10-12)

Mục tiêu: 3-5 showcase projects. Gợi ý projects:
- Stylized Character Shader (2 tuần)
- Environmental VFX Pack (2 tuần)
- Post-Processing Stack (1-2 tuần)
- Shader Tools (optional)
- Technical Art Portfolio Website (2 tuần)

Deliverable: Live URL, 3-5 projects documented, download links, breakdown videos.

---

## 📊 LEARNING RESOURCES (Curated)

Beginner → Intermediate:
- Freya Holmér - "Shaders for Game Devs" (YouTube)
- Brackeys - "Shaders in Unity" (YouTube)
- Catlike Coding - Unity Tutorials

Books:
- "Unity Shaders and Effects Cookbook" by Kenneth Lammers
- "The Book of Shaders" by Patricio Gonzalez Vivo

Interactive:
- Shadertoy, ShaderGraph Examples (Unity Learn)

Intermediate → Advanced: GPU Architecture, PBR Theory, Scriptable Render Pipeline docs.

Communities: Discord (Unity Developer Community), Unity Forums, Polycount, Reddit r/Unity3D.

---

## ⏱️ TIME MANAGEMENT (Parallel với Unity Track)

Weekly Schedule Example:

Weekdays:
- 7h-9h: Main Unity work (gameplay, Firebase, portfolio)
- 9h-9.30h: Break
- 9.30h-10.30h: Graphics learning (1h)
  - 30 min: Theory
  - 30 min: Practice

Weekends:
- Sáng: Unity main projects
- Chiều: Graphics mini-projects (2-3h)

Total: 5-7 giờ/tuần graphics

---

## 🎯 MILESTONES & CHECKPOINTS

- Month 2: Understand rendering pipeline, create 5 ShaderGraph shaders, write basic shader code
- Month 4: Lighting calculations, vertex animation shaders, toon shader complete
- Month 6: Normal mapping, parallax, 3 animated shaders, post-processing
- Month 9: 2 portfolio projects complete, breakdown videos
- Month 12: 3-5 showcase projects, portfolio website live

---

## 💼 CAREER PATHS VỚI GRAPHICS SKILLS

Junior Level (1-2 năm): Technical Artist Intern, Unity Developer (Graphics focus)

Mid Level (2-5 năm): Graphics Programmer

Senior/Lead: Senior Technical Artist, Lead Graphics Engineer

---

## 🚀 QUICK WIN STRATEGIES

If only 1 month: focus on Toon, Dissolve, Water. Deliverable: 1 video showcase (30-60s).

If only 1 week: ShaderGraph speed run — produce one polished effect and upload.

---

## ⚠️ COMMON PITFALLS (Tránh những lỗi này)

1. Tutorial Hell — Implement and modify, don't just watch.
2. Math Paralysis — Learn math on-demand.
3. Perfectionism — Ship, then iterate.
4. Ignore Performance — Profile and optimize.
5. Not Documenting — Comment and write breakdowns.

Performance Rules:
- Vertex shader cheaper than Fragment
- Avoid expensive loops in fragment
- Minimize texture lookups
- Mobile: keep instruction count low

---

## ✅ NEXT STEPS (cho bạn)

- Fork/clone repo, put this file under `Assets/Script/HuongDan/`
- Integrate 1 shader per week into KatyushaPJ
- Record short breakdown videos and add links to this document

Nếu muốn, tôi có thể: 1) tạo một issue list (GitHub Issues) từ milestones; 2) generate checklist tasks; 3) tách mỗi phase thành issues. Chọn 1 trong 3 để tôi tiếp tục.

---

**Chúc bạn học tốt và sớm có portfolio ấn tượng!** 🎉
🎨 GRAPHICS PROGRAMMING ROADMAP - PHẦN PHỤ HỖ TRỢ UNITY
Context: Học Graphics Programming như skill phụ để nâng cao Unity development
Timeline: 6-12 tháng (parallel với Unity main track)
Cường độ: 3-5 giờ/tuần (không ảnh hưởng roadmap chính)

📊 TẦM NHÌN TỔNG QUAN
Graphics Programming trong Unity Context:
Code
UNITY DEVELOPER LEVELS:
├─ Level 1: Gameplay Programmer (bạn ở đây)
│   └─ Dùng built-in shaders, particle systems
│
├─ Level 2: Technical Artist (hybrid)
│   └─ Custom shaders, ShaderGraph, VFX Graph
│
└─ Level 3: Graphics Programmer (advanced)
    └─ Custom render pipeline, low-level optimization
Bạn đang có: ShaderLab 25.5% + HLSL 5.3% → Foundation tốt!

🎯 LỢI ÍCH KHI HỌC GRAPHICS
Career Benefits:
Skill Level	Job Opportunities	Salary Impact
No Graphics	Gameplay Programmer	Baseline
ShaderGraph + Basic Shaders	Technical Artist, Senior Unity Dev	+20-30%
HLSL + Custom Effects	Graphics Programmer, Lead Dev	+40-60%
Render Pipeline Expert	Graphics Engineer, AAA Studios	+80-100%
Trong Portfolio:
Trước (chỉ Gameplay):

Code
"Developed 2D Action RPG with inventory system"
→ Interviewer: "OK, standard stuff"
Sau (+ Graphics):

Code
"Developed 2D Action RPG with custom shaders:
- Dissolve effect with noise textures
- Rim lighting for hit feedback  
- Post-processing stack for stylized look"
→ Interviewer: "😍 Impressive technical depth!"
🗺️ ROADMAP TỔNG QUAN (6-12 THÁNG)
Phase 1: Foundations (Tháng 1-2)
Math for Graphics (vectors, matrices, spaces)
Rendering pipeline overview
Unity ShaderGraph (no code)
Phase 2: ShaderLab Basics (Tháng 3-4)
Vertex & Fragment shaders
Texture sampling
Lighting basics
Phase 3: HLSL Intermediate (Tháng 5-6)
Custom lighting models
Normal mapping, parallax
Animated shaders (dissolve, glitch, water)
Phase 4: Advanced Effects (Tháng 7-9)
Post-processing effects
Compute shaders
Custom render features (URP)
Phase 5: Portfolio Projects (Tháng 10-12)
3-5 shader showcase projects
Technical art portfolio
Breakdown videos
📚 CHI TIẾT TỪNG PHASE
PHASE 1: FOUNDATIONS (Tháng 1-2)
Tuần 1-2: Math for Graphics
Mục tiêu: Hiểu toán đằng sau graphics (không cần chứng minh)
Topics cần nắm:

1. Vectors (2-3 ngày)

Code
Concepts:
✅ Vector operations: add, subtract, scale
✅ Dot product (projection, angle)
✅ Cross product (perpendicular vector)
✅ Magnitude, normalization

Ứng dụng trong Unity:
- Dot product: Check if object is in front/behind camera
- Cross product: Calculate surface normals
- Normalize: Direction vectors for movement
Tài nguyên:

📺 3Blue1Brown: "Essence of Linear Algebra" (15 videos, mỗi video 10-15 phút)
📖 Unity Manual: Vector Cookbook
🎮 Practice: Visualize vectors trong Unity với Debug.DrawLine()
Deliverable:

C#
// VectorPractice.cs - Vẽ dot product visualization
void OnDrawGizmos()
{
    Vector3 forward = transform.forward;
    Vector3 toTarget = (target.position - transform.position).normalized;
    
    float dot = Vector3.Dot(forward, toTarget);
    
    // Dot = 1: same direction (green)
    // Dot = 0: perpendicular (yellow)  
    // Dot = -1: opposite (red)
    Gizmos.color = Color.Lerp(Color.red, Color.green, (dot + 1) / 2);
    Gizmos.DrawLine(transform.position, target.position);
}
2. Matrices & Transformations (2-3 ngày)

Code
Concepts:
✅ Translation, Rotation, Scale (TRS)
✅ Matrix multiplication order (why SRT?)
✅ Object space → World space → View space → Clip space
✅ Projection matrices (orthographic vs perspective)

Ứng dụng:
- Hiểu transform hierarchy
- Vertex shader transformations
- Camera frustum culling
Tài nguyên:

📺 "Math for Game Programmers: Understanding Homogeneous Coordinates" (GDC talk)
📖 Cheat sheet: Transformation matrices
🎮 Unity: Matrix4x4 API exploration
Deliverable:

Viết script vẽ coordinate axes của object (local vs world)
Giải thích tại sao vertex shader cần UNITY_MATRIX_MVP
3. Color Spaces & Gamma (1-2 ngày)

Code
Concepts:
✅ RGB, HSV, HDR
✅ Linear vs Gamma space
✅ Why artists work in Gamma but shaders in Linear

Ứng dụng:
- Color blending in shaders
- Post-processing accuracy
- Why PBR workflows use Linear space
Quick Reference:

C#
// Linear space (physics correct)
Color blended = colorA * colorB; 

// Gamma space (perceptual)
Color blended = Color.Lerp(colorA, colorB, 0.5f);
Tuần 3-4: Rendering Pipeline
Mục tiêu: Hiểu flow từ 3D model → pixels trên màn hình
1. Graphics Pipeline Overview (2 ngày)

Code
CPU Side:
1. Scene setup (objects, lights, camera)
2. Culling (frustum, occlusion)
3. Batching (reduce draw calls)
4. Submit draw calls to GPU

GPU Side:
5. Vertex Shader (transform vertices)
6. Rasterization (convert triangles → pixels)
7. Fragment/Pixel Shader (calculate pixel color)
8. Output Merger (depth test, blending)
Visualization:

📺 "A trip through the Graphics Pipeline" (blog series by Fabian Giesen)
🎮 Unity Frame Debugger hands-on
Bài tập:

Mở Frame Debugger trong game của bạn
Screenshot từng draw call
Viết document: "Rendering breakdown của KatyushaPJ"
2. Unity ShaderGraph (5-7 ngày)

Tại sao học ShaderGraph trước code:

✅ Visual → dễ hiểu concepts
✅ See results ngay lập tức
✅ Learn nodes = learn shader operations
✅ Export code để xem HLSL generated
Projects (1 project/ngày):

Day 1: Hello ShaderGraph

Tạo shader đơn giản: Color gradient
Nodes: UV, Gradient, Color
Day 2: Texture Sampling

Texture shader với Tiling/Offset control
Nodes: Texture2D, Sampler, UV Transform
Day 3: Vertex Animation

Wave effect (grass, flag)
Nodes: Time, Sine, Position (Object space)
Day 4: Dissolve Effect

Dissolve với noise texture
Nodes: Texture Sample, Step, Smoothstep
Day 5: Rim Lighting

Fresnel effect
Nodes: Fresnel, Power, Multiply
Day 6-7: Mini Project

Stylized water shader
Combine: Wave animation + Fresnel + Normal mapping
Deliverable:

6 shaders working trong Unity
Screenshots + notes về mỗi node
Export code, highlight HLSL sections (để chuẩn bị Phase 2)
PHASE 2: SHADERLAB BASICS (Tháng 3-4)
Tuần 1-2: Your First Shader
Mục tiêu: Viết shader đầu tiên bằng code
1. ShaderLab Structure (1 ngày)

HLSL
Shader "Custom/MyFirstShader"
{
    Properties  // Inspector properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader  // GPU instructions
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata  // Input from mesh
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f  // Vertex → Fragment
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}
Breakdown từng dòng (1-2 ngày học):

POSITION: Semantic - tells GPU "this is vertex position"
UnityObjectToClipPos(): Transform Object → Clip space
TRANSFORM_TEX: Apply Tiling/Offset
tex2D(): Sample texture at UV
SV_Target: Output color
Practice:

Type lại shader trên (không copy-paste)
Modify: Thêm property _Brightness, multiply vào color
Test với material trong game
2. Vertex Shader Magic (3-4 ngày)

Concepts:

Vertex shader runs PER VERTEX
Modify vertex.position → change mesh shape
Interpolation: Vertex → Fragment (automatic)
Project 1: Pulsing Effect

HLSL
v2f vert (appdata v)
{
    v2f o;
    
    // Pulse vertex along normal
    float pulse = sin(_Time.y * _Speed) * _Amount;
    v.vertex.xyz += v.normal * pulse;
    
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}
Project 2: Flag Wave

HLSL
v.vertex.y += sin(v.vertex.x * _Frequency + _Time.y * _Speed) * _Amplitude;
Project 3: Explode Mesh

HLSL
// Move vertices outward from center
v.vertex.xyz += normalize(v.vertex.xyz) * _ExplodeAmount;
Deliverable:

3 vertex shaders working
Video demo effects
Understand: "Tại sao vertex animation rẻ hơn animation system?"
Tuần 3-4: Fragment Shader Effects
Concepts:

Fragment shader runs PER PIXEL
Color calculations, texture blending
Performance: Vertex shader > Fragment shader
Project 1: Color Grading

HLSL
fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    
    // Desaturate
    float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
    col.rgb = lerp(col.rgb, gray.xxx, _Saturation);
    
    // Brightness/Contrast
    col.rgb = (col.rgb - 0.5) * _Contrast + 0.5 + _Brightness;
    
    return col;
}
Project 2: UV Manipulation

HLSL
// Scroll texture
i.uv += _Time.y * _ScrollSpeed;

// Rotate UV
float2 rotatedUV = RotateUV(i.uv, _Time.y);
Project 3: Multi-Texture Blending

HLSL
fixed4 tex1 = tex2D(_MainTex, i.uv);
fixed4 tex2 = tex2D(_SecondTex, i.uv);
fixed4 mask = tex2D(_MaskTex, i.uv);

return lerp(tex1, tex2, mask.r);
Deliverable:

3 fragment shaders
Understand: "Khi nào dùng vertex, khi nào dùng fragment?"
PHASE 3: HLSL INTERMEDIATE (Tháng 5-6)
Tuần 1-2: Lighting Basics
Mục tiêu: Hiểu PBR (Physically Based Rendering) basics
1. Diffuse Lighting (Lambert) (2 ngày)

HLSL
// In fragment shader
float3 normal = normalize(i.worldNormal);
float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

float NdotL = max(0, dot(normal, lightDir));
fixed4 diffuse = _LightColor0 * NdotL;
Concepts:

N·L = 0: perpendicular (no light)
N·L = 1: facing light (full brightness)
max(0, ...): clamp negative values
2. Specular Lighting (Blinn-Phong) (2 ngày)

HLSL
float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
float3 halfDir = normalize(lightDir + viewDir);

float NdotH = max(0, dot(normal, halfDir));
float specular = pow(NdotH, _Glossiness);
Why Blinn-Phong > Phong:

Cheaper (half vector vs reflection)
Better interpolation
3. Custom Lighting (Toon Shading) (3-4 ngày)

HLSL
// Posterize lighting
float NdotL = dot(normal, lightDir);
float toonLight = smoothstep(_ShadowThreshold - 0.01, _ShadowThreshold + 0.01, NdotL);

fixed4 col = _BaseColor * toonLight;

// Add specular highlight
float specular = smoothstep(_SpecularThreshold - 0.01, _SpecularThreshold + 0.01, NdotH);
col.rgb += specular * _SpecularColor;

// Rim lighting
float rim = 1 - dot(viewDir, normal);
rim = smoothstep(_RimThreshold - 0.01, _RimThreshold + 0.01, rim);
col.rgb += rim * _RimColor;
Deliverable:

Toon shader hoàn chỉnh
Apply lên character trong game
Compare với Standard shader (screenshot)
Tuần 3-4: Texture Magic
1. Normal Mapping (2 ngày)

HLSL
// Sample normal map
float3 tangentNormal = UnpackNormal(tex2D(_NormalMap, i.uv));

// Transform tangent → world space
float3 worldNormal = normalize(
    tangentNormal.x * i.tangent +
    tangentNormal.y * i.binormal +
    tangentNormal.z * i.normal
);
Why normal maps:

Fake detail without high poly mesh
Crucial for modern games
2. Parallax Mapping (2-3 ngày)

HLSL
// Offset UV based on height map
float height = tex2D(_HeightMap, i.uv).r;
float2 offset = viewDir.xy * height * _ParallaxStrength;
i.uv += offset;

// Then sample texture with offset UV
fixed4 col = tex2D(_MainTex, i.uv);
Compare:

Normal map: Lighting fake detail
Parallax: UV offset fake depth
3. Triplanar Mapping (2 ngày)

HLSL
// Sample texture from 3 angles
fixed4 xProjection = tex2D(_MainTex, i.worldPos.yz);
fixed4 yProjection = tex2D(_MainTex, i.worldPos.xz);
fixed4 zProjection = tex2D(_MainTex, i.worldPos.xy);

// Blend based on normal
float3 blendWeights = abs(i.worldNormal);
blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);

fixed4 col = 
    xProjection * blendWeights.x +
    yProjection * blendWeights.y +
    zProjection * blendWeights.z;
Use case:

Terrain, rocks (no UV distortion)
PHASE 4: ADVANCED EFFECTS (Tháng 7-9)
Tuần 1-3: Animated Shaders
1. Dissolve Effect (2-3 ngày)

HLSL
// Sample noise
float noise = tex2D(_NoiseTex, i.uv).r;

// Cutoff based on dissolve amount
clip(noise - _DissolveAmount);

// Edge glow
float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, noise);
col.rgb += edge * _EdgeColor;
Applications:

Enemy death effect
Teleport VFX
Scene transitions
2. Hologram Shader (3-4 ngày)

HLSL
// Scanlines
float scanline = sin(i.uv.y * _ScanlineFrequency + _Time.y * _ScanlineSpeed);
col.rgb *= lerp(0.8, 1.0, scanline);

// Flicker
float flicker = sin(_Time.y * _FlickerSpeed) * 0.5 + 0.5;
col.a *= lerp(_MinAlpha, _MaxAlpha, flicker);

// Fresnel glow
float fresnel = pow(1 - dot(i.viewDir, i.normal), _FresnelPower);
col.rgb += fresnel * _GlowColor;
3. Water Shader (5-7 ngày) - PORTFOLIO PIECE

HLSL
Features:
✅ Normal mapping (2 layers, different speeds)
✅ Fresnel reflection
✅ Depth fade (shallow = transparent)
✅ Foam at edges
✅ Vertex wave animation
✅ Refraction (distortion)
This shader alone = hiring signal!

Tuần 4-6: Post-Processing
1. Full-Screen Shaders (2 ngày)

C#
// C# script
void OnRenderImage(RenderTexture src, RenderTexture dest)
{
    Graphics.Blit(src, dest, material);
}
HLSL
// Shader
fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    // Apply effect
    return col;
}
2. Common Effects:

Grayscale:

HLSL
float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
return fixed4(gray, gray, gray, 1);
Blur (Box Blur 3x3):

HLSL
fixed4 BlurBox(sampler2D tex, float2 uv, float2 texelSize)
{
    fixed4 sum = 0;
    for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
            sum += tex2D(tex, uv + float2(x, y) * texelSize);
    return sum / 9;
}
Edge Detection (Sobel):

HLSL
float Sobel(sampler2D tex, float2 uv)
{
    float2 texelSize = 1.0 / _ScreenParams.xy;
    
    // Sample 3x3 neighborhood
    float tl = Luminance(tex2D(tex, uv + float2(-1, 1) * texelSize));
    float t  = Luminance(tex2D(tex, uv + float2(0, 1) * texelSize));
    // ... (8 more samples)
    
    // Sobel kernels
    float gx = tl + 2*l + bl - tr - 2*r - br;
    float gy = tl + 2*t + tr - bl - 2*b - br;
    
    return sqrt(gx*gx + gy*gy);
}
Vignette:

HLSL
float2 center = i.uv - 0.5;
float vignette = 1 - dot(center, center) * _VignetteStrength;
col.rgb *= vignette;
Deliverable:

5 post-processing effects
Toggle ON/OFF trong game với hotkey
Before/After screenshots
Tuần 7-9: Compute Shaders (Advanced)
Khi nào cần:

GPU particle systems (10k+ particles)
Fluid simulation
Procedural generation
Heavy parallel computation
Example: Particle System

HLSL
// ComputeShader
#pragma kernel UpdateParticles

struct Particle
{
    float3 position;
    float3 velocity;
    float lifetime;
};

RWStructuredBuffer<Particle> particles;

[numthreads(64,1,1)]
void UpdateParticles (uint3 id : SV_DispatchThreadID)
{
    Particle p = particles[id.x];
    
    // Physics
    p.velocity += float3(0, -9.8, 0) * deltaTime;  // Gravity
    p.position += p.velocity * deltaTime;
    p.lifetime -= deltaTime;
    
    // Bounds check
    if (p.position.y < 0)
    {
        p.position.y = 0;
        p.velocity.y *= -0.5;  // Bounce
    }
    
    particles[id.x] = p;
}
C# Dispatch:

C#
computeShader.SetBuffer(kernel, "particles", particleBuffer);
computeShader.SetFloat("deltaTime", Time.deltaTime);
computeShader.Dispatch(kernel, particleCount / 64, 1, 1);
Deliverable (optional - chỉ nếu có thời gian):

GPU particle system (rain, snow, sparks)
Performance comparison: CPU vs GPU (10k particles)
PHASE 5: PORTFOLIO PROJECTS (Tháng 10-12)
Mục tiêu: 3-5 Showcase Projects
Project 1: Stylized Character Shader (2 tuần)

Code
Features:
✅ Toon shading (multi-step)
✅ Rim lighting
✅ Specular highlight
✅ Outline (inverted hull method)
✅ Texture + Normal map support
Deliverable:

Character model với shader
Video turnaround (360° rotation)
Breakdown: "How I made this shader" (blog post)
Project 2: Environmental VFX Pack (2 tuần)

Code
5 Shaders:
1. Stylized water (waves, foam, refraction)
2. Lava (scrolling textures, glow, heat distortion)
3. Magical portal (swirl, particles, glow)
4. Energy shield (hexagon pattern, hit ripples)
5. Hologram (scanlines, flicker, glitch)
Deliverable:

Demo scene với 5 effects
Video showcase (music, slow-mo)
Asset Store style documentation
Project 3: Post-Processing Stack (1-2 tuần)

Code
Effects:
1. Bloom (bright pass + blur + composite)
2. Chromatic aberration
3. Vignette + Film grain
4. Color grading (LUT)
5. Depth of field (simple)
Deliverable:

Toggle stack ON/OFF
Before/After comparison video
Performance metrics (FPS impact)
Project 4: Shader Tools (Optional - 2 tuần)

Code
Inspector UI:
- Custom material editor
- Real-time preview
- Preset system
Example:

C#
public class ToonShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
    {
        // Custom UI layout
        EditorGUILayout.LabelField("Lighting", EditorStyles.boldLabel);
        // ... properties
    }
}
Project 5: Technical Art Portfolio Website (2 tuần)

Structure:

Code
Homepage:
- Hero video (shader reel)
- About (3 sentences)
- Projects grid (thumbnails)

Each Project Page:
- Embedded video/GIF
- Feature list
- Technical breakdown (collapsible)
- Code snippets (syntax highlighted)
- Download link (Unity package)
Tools:

GitHub Pages (free hosting)
Jekyll theme (minimal)
Or: ArtStation, Behance
Deliverable:

Live URL
3-5 projects documented
Contact form/email
📊 LEARNING RESOURCES (Curated)
Beginner → Intermediate:
1. Free Courses:

📺 Freya Holmér - "Shaders for Game Devs" (YouTube)

BEST starter series (10 episodes)
Math explained visually
Unity focused
📺 Brackeys - "Shaders in Unity" (YouTube)

Quick intros (5-10 min each)
Practical examples
📖 Catlike Coding - Unity Tutorials

Deep technical articles
Rendering series (20+ tutorials)
2. Books:

📘 "Unity Shaders and Effects Cookbook" by Kenneth Lammers
Recipe-based (copy-paste friendly)
60+ shader examples
📘 "The Book of Shaders" by Patricio Gonzalez Vivo
Free online
GLSL (not HLSL) but concepts same
Interactive examples
3. Interactive Learning:

🌐 Shadertoy (shadertoy.com)
1000s of shaders
Live code editor
Learn by modifying others' code
🌐 ShaderGraph Examples (Unity Learn)
Official samples
Download projects
Intermediate → Advanced:
1. GPU Architecture:

📖 "A trip through the Graphics Pipeline" (blog)
How GPU works internally
Why performance matters
📖 GPU Gems series (free PDFs)
Advanced techniques
AAA studio methods
2. PBR Theory:

📖 "Physically Based Rendering" book (pbr-book.org)
Free online
Heavy math (optional deep dive)
📺 "Background: Physics and Math of Shading" (SIGGRAPH)
Academic but accessible
3. Unity Official:

📖 Scriptable Render Pipeline documentation
URP/HDRP custom passes
Advanced Unity features
Communities & Help:
1. Discord Servers:

Unity Developer Community (200k+ members)
#shaders channel
#technical-art
Technical Artists (TA) Discord
Industry pros
Shader help
2. Forums:

Unity Forums - Shaders
Polycount - Technical Talk
Reddit: r/Unity3D, r/GraphicsProgramming
3. Portfolio Inspiration:

ArtStation (search "technical artist")
80.lv (shader breakdowns)
Simon Trümpler (shader wizard)
⏱️ TIME MANAGEMENT (Parallel với Unity Track)
Weekly Schedule Example:
Code
MÔN 2-5 (Weekdays):
├─ 7h-9h: Main Unity work (gameplay, Firebase, portfolio)
├─ 9h-9.30h: Break
└─ 9.30h-10.30h: Graphics learning (1h)
    - 30 min: Theory (video/article)
    - 30 min: Practice (shader coding)

THỨ 7-CN (Weekends):
├─ Sáng: Unity main projects
└─ Chiều: Graphics mini-projects (2-3h)
    - Saturday: Implement shader tutorial
    - Sunday: Integrate into portfolio project
Total: 5-7 giờ/tuần graphics

Không ảnh hưởng Unity roadmap chính
6 tháng = 120-180 giờ (đủ cho Intermediate level)
🎯 MILESTONES & CHECKPOINTS
Month 2 Checkpoint:
 Hiểu rendering pipeline
 Tạo được 5 shaders bằng ShaderGraph
 Viết được basic shader code (texture + color)
Month 4 Checkpoint:
 Lighting calculations (diffuse, specular)
 Vertex animation shaders
 Toon shader hoàn chỉnh
Month 6 Checkpoint:
 Normal mapping, parallax
 3 animated shaders (dissolve, water, hologram)
 Post-processing effects
Month 9 Checkpoint:
 2 portfolio projects hoàn chỉnh
 Shader breakdown videos
 Technical blog posts
Month 12 Checkpoint:
 3-5 showcase projects
 Portfolio website live
 Apply Technical Artist intern positions
💼 CAREER PATHS VỚI GRAPHICS SKILLS
Junior Level (1-2 năm kinh nghiệm):
1. Technical Artist Intern

Code
Responsibilities:
- Optimize shaders for mobile
- Create VFX với Shader Graph
- Support artists với technical issues

Requirements:
✅ Shader Graph expert
✅ Basic HLSL
✅ Understanding of performance

Salary VN: 8-12M VND
2. Unity Developer (Graphics focus)

Code
Responsibilities:
- Implement art director vision
- Custom shaders cho game style
- Performance optimization

Requirements:
✅ HLSL intermediate
✅ Post-processing
✅ URP/HDRP knowledge

Salary VN: 10-15M VND
Mid Level (2-5 năm):
3. Graphics Programmer

Code
Responsibilities:
- Custom render features
- Optimize rendering pipeline
- Tools for artists

Requirements:
✅ HLSL advanced
✅ Compute shaders
✅ C# rendering API

Salary VN: 20-35M VND
Salary Foreign: $60k-90k
4. Senior Technical Artist

Code
Responsibilities:
- Define art pipeline
- Lead VFX/shader team
- R&D new techniques

Requirements:
✅ Portfolio stunning
✅ Shader architecture
✅ Mentorship skills

Salary VN: 30-50M VND
Senior/Lead (5+ năm):
5. Lead Graphics Engineer

Code
AAA Studios (Ubisoft, EA, Unity Technologies)
- Architecture render pipeline
- Performance on multiple platforms
- Team leadership

Salary: $100k-150k+
🚀 QUICK WIN STRATEGIES
Nếu chỉ có 1 tháng:
Focus 3 shaders này:

Toon Shader (1 tuần)
Ramp lighting, rim, outline
Apply lên character
Dissolve Effect (3-4 ngày)
Noise texture, clip, edge glow
Demo enemy death
Water Shader (1.5 tuần)
Waves, normal map, fresnel
Polish đẹp nhất có thể
Deliverable:

1 video showcase (30-60 giây)
Add vào portfolio website
Mention trong resume: "Custom shader development (HLSL)"
Impact:

Stand out trong Unity Developer applications
+20% chance pass screening
Nếu chỉ có 1 tuần:
ShaderGraph Speed Run:

Day 1: Install, hello world
Day 2: Dissolve shader
Day 3: Rim lighting
Day 4-5: Polish 1 effect đẹp nhất
Day 6: Record video, screenshot
Day 7: Upload portfolio, update resume
Minimum viable graphics skill:

"Experience with Unity Shader Graph"
Link: itch.io demo
Still impressive cho intern level!
⚠️ COMMON PITFALLS (Tránh những lỗi này)
1. Tutorial Hell:
Code
❌ Xem 50 tutorials, không làm gì
✅ Xem 1 tutorial, implement ngay, modify, break it
2. Math Paralysis:
Code
❌ "Tôi phải học hết Linear Algebra trước"
✅ Học math on-demand (cần gì học nấy)
3. Perfectionism:
Code
❌ "Shader này chưa perfect, chưa portfolio được"
✅ "Shader này working, ship it, iterate sau"
4. Ignore Performance:
Code
❌ Shader đẹp nhưng 10 FPS
✅ Profile, optimize (Frame Debugger, Profiler)
Performance Rules:

Vertex shader > Fragment shader
Avoid loops trong fragment (nếu có thể)
Texture lookups expensive (cache nếu dùng nhiều lần)
Mobile: Keep instructions < 50-100
5. Not Documenting:
Code
❌ Viết shader, không note gì, quên sau 1 tháng
✅ Comment code, write breakdown, future you will thank