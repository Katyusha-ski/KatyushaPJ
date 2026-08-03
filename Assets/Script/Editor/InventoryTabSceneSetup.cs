#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class InventoryTabSceneSetup
{
    private const string InventoryIconGuid = "236587ec711348a4792e0060efb1bf73";
    private const long InventoryIconFileId = 4717861300493662584;
    private const string FrameSpriteGuid = "c902e08bd83e04947b6e8d7f31f385be";
    private const long FrameSpriteFileId = -8929440768489705544;

    [MenuItem("Tools/Setup/Build Inventory Tabs (One-Time)")]
    public static void Build()
    {
        GameObject uiThing = GameObject.Find("UIthing");
        if (uiThing == null)
        {
            Debug.LogError("[InventoryTabSceneSetup] Không tìm thấy 'UIthing' trong scene đang mở. " +
                "Hãy mở đúng scene chứa UI chính (ví dụ GrassScene.unity) trước khi chạy.");
            return;
        }

        Transform inventoryUIT = uiThing.transform.Find("Inventory UI");
        if (inventoryUIT == null)
        {
            Debug.LogError("[InventoryTabSceneSetup] Không tìm thấy 'Inventory UI' dưới UIthing.");
            return;
        }
        GameObject inventoryUI = inventoryUIT.gameObject;

        if (inventoryUI.transform.Find("TabBar") != null)
        {
            Debug.LogError("[InventoryTabSceneSetup] Đã thấy 'TabBar' tồn tại — có vẻ đã chạy tool này rồi. " +
                "Nếu muốn chạy lại, tự xoá tay TabBar/InventoryContent/SkillContent/QuestContent trước.");
            return;
        }

        GameObject inventoryContent = CreateFullStretchChild(inventoryUI.transform, "InventoryContent");

        string[] existingChildNames = { "Image", "SlotPanel", "EquipSlots", "ItemDetail" };
        foreach (string childName in existingChildNames)
        {
            Transform child = inventoryUI.transform.Find(childName);
            if (child == null)
            {
                Debug.LogWarning($"[InventoryTabSceneSetup] Không tìm thấy con '{childName}' để di chuyển — bỏ qua.");
                continue;
            }
            child.SetParent(inventoryContent.transform, false);
        }

        Vector2 frameAnchoredPos = new Vector2(3.5377f, -22.6412f);
        Vector2 frameSize = new Vector2(548.5782f, 379.1223f);
        Sprite frameSprite = LoadSpriteByGuid(FrameSpriteGuid, FrameSpriteFileId);

        GameObject skillContent = CreateCenteredPanel(inventoryUI.transform, "SkillContent", frameAnchoredPos, frameSize, frameSprite);
        BuildSkillGrid(skillContent, frameSprite);
        skillContent.SetActive(false);

        GameObject questContent = CreateCenteredPanel(inventoryUI.transform, "QuestContent", frameAnchoredPos, frameSize, frameSprite);
        BuildQuestPlaceholder(questContent);
        questContent.SetActive(false);

        float tabBarWidth = 88f;
        float gap = 8f;
        float frameLeftEdgeX = frameAnchoredPos.x - frameSize.x / 2f;
        float tabBarCenterX = frameLeftEdgeX - gap - tabBarWidth / 2f;
        Vector2 tabBarAnchoredPos = new Vector2(tabBarCenterX, frameAnchoredPos.y);
        Vector2 tabBarSize = new Vector2(tabBarWidth, frameSize.y);

        GameObject tabBar = CreateCenteredPanel(inventoryUI.transform, "TabBar", tabBarAnchoredPos, tabBarSize, null);
        Image tabBarBg = tabBar.GetComponent<Image>();
        tabBarBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        VerticalLayoutGroup vlg = tabBar.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.spacing = 12f;
        vlg.padding = new RectOffset(12, 12, 16, 16);
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;

        Sprite inventoryIcon = LoadSpriteByGuid(InventoryIconGuid, InventoryIconFileId);
        Sprite skillIconPlaceholder = LoadSpriteByName("charged1");
        Sprite questIconPlaceholder = LoadSpriteByName("I_Scroll");

        GameObject btnInventory = CreateTabButton(tabBar.transform, "Btn_Inventory", inventoryIcon);
        GameObject btnSkill = CreateTabButton(tabBar.transform, "Btn_Skill", skillIconPlaceholder);
        GameObject btnQuest = CreateTabButton(tabBar.transform, "Btn_Quest", questIconPlaceholder);

        TabController tabController = inventoryUI.GetComponent<TabController>();
        if (tabController == null)
            tabController = inventoryUI.AddComponent<TabController>();

        tabController.tabs = new TabController.TabEntry[]
        {
            MakeTabEntry("Inventory", btnInventory, inventoryContent),
            MakeTabEntry("Skill", btnSkill, skillContent),
            MakeTabEntry("Quest", btnQuest, questContent),
        };
        tabController.activeBackgroundColor = Color.black;
        tabController.activeIconColor = Color.white;
        tabController.inactiveBackgroundColor = Color.white;
        tabController.inactiveIconColor = Color.black;
        tabController.defaultTabIndex = 0;

        EditorUtility.SetDirty(inventoryUI);
        EditorSceneManager.MarkSceneDirty(inventoryUI.scene);

        Debug.Log("[InventoryTabSceneSetup] Hoàn tất dựng Tab UI. Kiểm tra lại trong Scene view / Play mode " +
            "rồi Save Scene (Ctrl+S). Icon tab Skill/Quest đang là placeholder tạm, thay sau khi có icon thật.");
    }

    [MenuItem("Tools/Setup/Repair Skill Cells Wiring")]
    public static void RepairSkillCellsWiring()
    {
        GameObject uiThing = GameObject.Find("UIthing");
        if (uiThing == null) { Debug.LogError("[Repair] Không tìm thấy UIthing."); return; }

        Transform skillContentT = uiThing.transform.Find("Inventory UI/SkillContent");
        if (skillContentT == null) { Debug.LogError("[Repair] Không tìm thấy SkillContent."); return; }

        SkillSystemUI skillUI = skillContentT.GetComponent<SkillSystemUI>();
        if (skillUI == null) { Debug.LogError("[Repair] SkillContent không có SkillSystemUI."); return; }

        SkillCellUI[] allCells = skillContentT.GetComponentsInChildren<SkillCellUI>(true);
        if (allCells.Length == 0) { Debug.LogError("[Repair] Không tìm thấy ô SkillCellUI nào bên trong SkillContent."); return; }

        int fixedCount = 0;
        foreach (var cellUI in allCells)
        {
            int index = cellUI.row * 5 + cellUI.col;
            if (index < 0 || index >= skillUI.cells.Length) continue;
            skillUI.cells[index] = new SkillSystemUI.SkillCell
            {
                background = cellUI.background,
                icon = cellUI.icon,
                lockedBackground = cellUI.lockedBackground,
                unlockedBackground = cellUI.unlockedBackground,
                skillIcon = cellUI.skillIcon
            };
            fixedCount++;
        }

        EditorUtility.SetDirty(skillUI);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(skillContentT.gameObject.scene);
        Debug.Log($"[Repair] Đã wiring lại {fixedCount}/{allCells.Length} ô vào SkillSystemUI.cells. " +
            "Nhớ Save Scene (Ctrl+S).");
    }

    private static TabController.TabEntry MakeTabEntry(string name, GameObject button, GameObject content)
    {
        Image bg = button.GetComponent<Image>();
        Image icon = button.transform.Find("Icon").GetComponent<Image>();
        return new TabController.TabEntry
        {
            tabName = name,
            button = button.GetComponent<Button>(),
            buttonBackground = bg,
            buttonIcon = icon,
            contentPanel = content
        };
    }

    private static GameObject CreateFullStretchChild(Transform parent, string name)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    private static GameObject CreateCenteredPanel(Transform parent, string name, Vector2 anchoredPos, Vector2 size, Sprite sprite)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
        }
        else
        {
            img.color = new Color(1f, 1f, 1f, 0f);
        }
        return go;
    }

    private static GameObject CreateTabButton(Transform parent, string name, Sprite icon)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 64;
        le.preferredHeight = 64;

        Image bg = go.GetComponent<Image>();
        bg.color = Color.white;

        GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconGO.transform.SetParent(go.transform, false);
        RectTransform iconRT = iconGO.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0.15f, 0.15f);
        iconRT.anchorMax = new Vector2(0.85f, 0.85f);
        iconRT.offsetMin = Vector2.zero;
        iconRT.offsetMax = Vector2.zero;

        Image iconImg = iconGO.GetComponent<Image>();
        iconImg.color = Color.black;
        if (icon != null)
        {
            iconImg.sprite = icon;
        }
        else
        {
            Debug.LogWarning($"[InventoryTabSceneSetup] Không tìm thấy icon cho nút '{name}' — để trống, tự gán sau.");
        }

        return go;
    }

    private static void BuildSkillGrid(GameObject skillContent, Sprite frameSprite)
    {
        GameObject grid = new GameObject("Grid", typeof(RectTransform), typeof(GridLayoutGroup));
        grid.transform.SetParent(skillContent.transform, false);
        RectTransform gridRT = grid.GetComponent<RectTransform>();
        gridRT.anchorMin = Vector2.zero;
        gridRT.anchorMax = Vector2.one;
        gridRT.offsetMin = new Vector2(16, 16);
        gridRT.offsetMax = new Vector2(-16, -16);

        GridLayoutGroup glg = grid.GetComponent<GridLayoutGroup>();
        glg.cellSize = new Vector2(90, 64);
        glg.spacing = new Vector2(8, 8);
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 5;
        glg.childAlignment = TextAnchor.MiddleCenter;

        skillContent.AddComponent<SkillSystemUI>();

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                GameObject cell = new GameObject($"SkillCell_{r}_{c}", typeof(RectTransform), typeof(SkillCellUI));
                cell.transform.SetParent(grid.transform, false);

                GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
                bgGO.transform.SetParent(cell.transform, false);
                RectTransform bgRT = bgGO.GetComponent<RectTransform>();
                bgRT.anchorMin = Vector2.zero;
                bgRT.anchorMax = Vector2.one;
                bgRT.offsetMin = Vector2.zero;
                bgRT.offsetMax = Vector2.zero;
                Image bgImg = bgGO.GetComponent<Image>();
                if (frameSprite != null) { bgImg.sprite = frameSprite; bgImg.type = Image.Type.Sliced; }

                GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                iconGO.transform.SetParent(cell.transform, false);
                RectTransform iconRT = iconGO.GetComponent<RectTransform>();
                iconRT.anchorMin = new Vector2(0.2f, 0.2f);
                iconRT.anchorMax = new Vector2(0.8f, 0.8f);
                iconRT.offsetMin = Vector2.zero;
                iconRT.offsetMax = Vector2.zero;

                SkillCellUI cellUI = cell.GetComponent<SkillCellUI>();
                cellUI.row = r;
                cellUI.col = c;
                cellUI.background = bgImg;
                cellUI.icon = iconGO.GetComponent<Image>();
                cellUI.lockedBackground = frameSprite;
                cellUI.unlockedBackground = frameSprite;
                cellUI.skillIcon = null;

                SkillSystemUI skillUIRef = skillContent.GetComponent<SkillSystemUI>();
                if (skillUIRef != null)
                {
                    int index = r * 5 + c;
                    skillUIRef.cells[index] = new SkillSystemUI.SkillCell
                    {
                        background = cellUI.background,
                        icon = cellUI.icon,
                        lockedBackground = cellUI.lockedBackground,
                        unlockedBackground = cellUI.unlockedBackground,
                        skillIcon = cellUI.skillIcon
                    };
                }
            }
        }
    }

    private static void BuildQuestPlaceholder(GameObject questContent)
    {
        GameObject textGO = new GameObject("PlaceholderText", typeof(RectTransform), typeof(Text));
        textGO.transform.SetParent(questContent.transform, false);
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Text text = textGO.GetComponent<Text>();
        text.text = "Quest Item — đang phát triển";
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 20;
    }

    private static Sprite LoadSpriteByGuid(string guid, long fileId)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path)) return null;
        var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        foreach (var asset in assets)
        {
            if (asset is Sprite sprite)
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string g, out long localId) && localId == fileId)
                    return sprite;
            }
        }
        return null;
    }

    private static Sprite LoadSpriteByName(string approximateName)
    {
        string[] guids = AssetDatabase.FindAssets($"{approximateName} t:Sprite");
        if (guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
#endif
