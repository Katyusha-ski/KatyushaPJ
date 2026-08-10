#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        BuildQuestDetailLayout(questContent, frameSprite);
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
                unlockedBackground = cellUI.unlockedBackground
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

                SkillSystemUI skillUIRef = skillContent.GetComponent<SkillSystemUI>();
                if (skillUIRef != null)
                {
                    int index = r * 5 + c;
                    skillUIRef.cells[index] = new SkillSystemUI.SkillCell
                    {
                        background = cellUI.background,
                        icon = cellUI.icon,
                        lockedBackground = cellUI.lockedBackground,
                        unlockedBackground = cellUI.unlockedBackground
                    };
                }
            }
        }
    }

    [MenuItem("Tools/Setup/Build Quest Detail Layout")]
    public static void BuildQuestDetailLayoutInOpenScene()
    {
        GameObject uiThing = GameObject.Find("UIthing");
        Transform questContent = uiThing != null ? uiThing.transform.Find("Inventory UI/QuestContent") : null;
        if (questContent == null)
        {
            Debug.LogError("[InventoryTabSceneSetup] QuestContent was not found in the open scene.");
            return;
        }

        Sprite frameSprite = LoadSpriteByGuid(FrameSpriteGuid, FrameSpriteFileId);
        BuildQuestDetailLayout(questContent.gameObject, frameSprite);
        EditorSceneManager.MarkSceneDirty(questContent.gameObject.scene);
        Selection.activeGameObject = questContent.gameObject;
        Debug.Log("[InventoryTabSceneSetup] Quest detail layout built. Save the scene to keep the changes.");
    }

    private static void BuildQuestDetailLayout(GameObject questContent, Sprite frameSprite)
    {
        for (int i = questContent.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(questContent.transform.GetChild(i).gameObject);

        GameObject listCard = CreateCard(questContent.transform, "QuestItemsCard", frameSprite);
        SetRect(listCard, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(12f, 12f), new Vector2(232f, -12f));
        CreateText(listCard.transform, "Title", "Quest items", 16, TextAlignmentOptions.Left, new Vector2(12f, -12f), new Vector2(-12f, -42f));

        GameObject scroll = CreateUIObject(listCard.transform, "QuestScroll", typeof(Image), typeof(ScrollRect));
        SetRect(scroll, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(10f, 10f), new Vector2(-10f, -48f));
        scroll.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.08f);
        ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        GameObject viewport = CreateUIObject(scroll.transform, "Viewport", typeof(Image), typeof(Mask));
        SetStretch(viewport);
        viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        viewport.GetComponent<Mask>().showMaskGraphic = false;
        scrollRect.viewport = viewport.GetComponent<RectTransform>();

        GameObject content = CreateUIObject(viewport.transform, "Content", typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        SetRect(content, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(6f, 0f), new Vector2(-6f, 0f));
        RectTransform contentRT = content.GetComponent<RectTransform>();
        contentRT.pivot = new Vector2(0.5f, 1f);
        VerticalLayoutGroup contentLayout = content.GetComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 6f;
        contentLayout.padding = new RectOffset(2, 2, 2, 2);
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        ContentSizeFitter contentFitter = content.GetComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRT;

        GameObject slotTemplate = CreateQuestSlotTemplate(questContent.transform, frameSprite);
        slotTemplate.SetActive(false);

        GameObject detailCard = CreateCard(questContent.transform, "QuestDetailCard", frameSprite);
        SetRect(detailCard, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(242f, 12f), new Vector2(-12f, -12f));
        CreateText(detailCard.transform, "Title", "Quest details", 16, TextAlignmentOptions.Left, new Vector2(14f, -12f), new Vector2(-14f, -42f));

        GameObject detailController = CreateUIObject(detailCard.transform, "QuestDetailController", typeof(QuestDetailUI));
        // This object only hosts the QuestDetailUI controller; the UI layout lives on DetailBody.
        GameObject detailBody = CreateUIObject(detailController.transform, "DetailBody", typeof(RectTransform));
        SetRect(detailBody, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(14f, 14f), new Vector2(-14f, -50f));

        TMP_Text detailDescription = CreateText(detailBody.transform, "Description", "", 15, TextAlignmentOptions.TopLeft, Vector2.zero, Vector2.zero);

        QuestDetailUI detailUI = detailController.GetComponent<QuestDetailUI>();
        detailUI.descriptionText = detailDescription;
        detailUI.detailRoot = detailBody;

        QuestListUI listUI = listCard.AddComponent<QuestListUI>();
        listUI.contentParent = content.transform;
        listUI.slotPrefab = slotTemplate;
        listUI.detailUI = detailUI;
    }

    private static GameObject CreateQuestSlotTemplate(Transform parent, Sprite frameSprite)
    {
        GameObject row = CreateUIObject(parent, "QuestSlotTemplate", typeof(Image), typeof(Button), typeof(LayoutElement), typeof(QuestSlotUI));
        row.GetComponent<Image>().sprite = frameSprite;
        row.GetComponent<Image>().type = Image.Type.Sliced;
        row.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.45f);
        LayoutElement rowLayout = row.GetComponent<LayoutElement>();
        rowLayout.minHeight = 42f;
        rowLayout.preferredHeight = 42f;
        rowLayout.flexibleWidth = 1f;

        GameObject border = CreateUIObject(row.transform, "SelectionBorder", typeof(Image));
        SetStretch(border, 2f);
        border.GetComponent<Image>().sprite = frameSprite;
        border.GetComponent<Image>().type = Image.Type.Sliced;
        border.GetComponent<Image>().raycastTarget = false;

        GameObject icon = CreateUIObject(row.transform, "Icon", typeof(Image));
        SetRect(icon, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(6f, -17f), new Vector2(38f, 17f));
        icon.GetComponent<Image>().preserveAspect = true;

        TMP_Text nameText = CreateText(row.transform, "Name", "", 14, TextAlignmentOptions.Left, new Vector2(50f, 0f), new Vector2(-6f, 0f));
        nameText.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
        nameText.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        nameText.GetComponent<RectTransform>().offsetMin = new Vector2(50f, 0f);
        nameText.GetComponent<RectTransform>().offsetMax = new Vector2(-6f, 0f);

        QuestSlotUI slotUI = row.GetComponent<QuestSlotUI>();
        slotUI.icon = icon.GetComponent<Image>();
        slotUI.nameText = nameText;
        slotUI.button = row.GetComponent<Button>();
        slotUI.selectionBorder = border.GetComponent<Image>();
        slotUI.selectedBorderColor = new Color(1f, 0.8f, 0.2f, 1f);
        slotUI.normalBorderColor = new Color(1f, 1f, 1f, 0.25f);
        row.GetComponent<Button>().targetGraphic = row.GetComponent<Image>();
        return row;
    }

    private static GameObject CreateCard(Transform parent, string name, Sprite frameSprite)
    {
        GameObject card = CreateUIObject(parent, name, typeof(Image));
        Image image = card.GetComponent<Image>();
        image.sprite = frameSprite;
        image.type = Image.Type.Sliced;
        image.color = new Color(1f, 1f, 1f, 0.92f);
        return card;
    }

    private static GameObject CreateUIObject(Transform parent, string name, params System.Type[] componentTypes)
    {
        GameObject go = new GameObject(name, componentTypes);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetStretch(GameObject go, float inset = 0f)
    {
        SetRect(go, Vector2.zero, Vector2.one, new Vector2(inset, inset), new Vector2(-inset, -inset));
    }

    private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    private static TMP_Text CreateText(Transform parent, string name, string text, float fontSize, TextAlignmentOptions alignment, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject go = CreateUIObject(parent, name, typeof(TextMeshProUGUI));
        SetRect(go, Vector2.zero, Vector2.one, offsetMin, offsetMax);
        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.black;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.raycastTarget = false;
        if (TMP_Settings.defaultFontAsset != null)
            tmp.font = TMP_Settings.defaultFontAsset;
        return tmp;
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
