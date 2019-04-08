using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public enum UIelement {
    Text = 0,
    Button = 1,
    Image = 2,
    Background = 3
}

public enum FontStyle {
    Normal = 0,
    Bold = 1,
    Italic = 2,
    BoldAndItalic = 3
}

public class UICreatorToolbox : EditorWindow {

    public UIelement elem;

    public FontStyle txtStyle;

    Color color, hColor, pColor, txtColor;
    Vector2 pos;
    int width, height;
    int txtSize;
    string txt;
    string canvasName, txtName, btnName, imgName, bgName;
    Font txtFont;
    Sprite sprite, btnSprite, bgSprite;

    private void OnEnable() {
        txtFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        txtSize = 18;
        width = 200;
        height = 75;
        canvasName = "UI Canvas";
        txtName = "Textbox";
        btnName = "Button";
        imgName = "Image";
        bgName = "Background";
        txt = "Enter text...";
        txtColor = Color.white;
        hColor = new Color32(41, 41, 41, 255);
        pColor = new Color32(41, 41, 41, 255);
    }

    [MenuItem("UI Creator/UI Creator Toolbox")]
    static void Init() {
        EditorWindow window = GetWindow(typeof(UICreatorToolbox), false, "UI Creator");
        window.position = new Rect(50, 50, 300, 500);
        window.Show();
    }

    private void OnGUI() {

        GUILayout.Label("Unity UI Creator Toolbox", EditorStyles.boldLabel);
        GUILayout.Label("Select the UI element you wish to add:");

        elem = (UIelement)EditorGUILayout.EnumPopup(elem);
        GUILayout.Label("Style Properties", EditorStyles.miniBoldLabel);

        switch (elem) {
            case UIelement.Text:
                txtName = EditorGUILayout.TextField("Name: ", txtName);
                GUILayout.Label("Text:");
                txt = EditorGUILayout.TextArea(txt, GUILayout.Height(50));
                txtFont = EditorGUILayout.ObjectField("Font: ", txtFont, typeof(Font), true) as Font;
                txtSize = EditorGUILayout.IntField("Font Size: ", txtSize);
                txtStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style: ", txtStyle);
                break;
            case UIelement.Button:
                btnName = EditorGUILayout.TextField("Name: ", btnName);
                btnSprite = EditorGUILayout.ObjectField("Source Image: ", btnSprite, typeof(Sprite), true, GUILayout.Height(100)) as Sprite;
                EditorGUILayout.Space();
                txt = EditorGUILayout.TextField("Button Text: ", txt);
                txtFont = EditorGUILayout.ObjectField("Font: ", txtFont, typeof(Font), true) as Font;
                txtSize = EditorGUILayout.IntField("Font Size: ", txtSize);
                txtStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style: ", txtStyle);
                txtColor = EditorGUILayout.ColorField("Text Color: ", txtColor);
                txtColor.a = 255;
                hColor = EditorGUILayout.ColorField("Highlighted Color: ", hColor);
                hColor.a = 255;
                pColor = EditorGUILayout.ColorField("Pressed Color: ", pColor);
                pColor.a = 255;
                break;
            case UIelement.Image:
                imgName = EditorGUILayout.TextField("Name: ", imgName);
                sprite = EditorGUILayout.ObjectField("Source Image: ", sprite, typeof(Sprite), true, GUILayout.Height(100)) as Sprite;
                EditorGUILayout.Space();
                break;
            case UIelement.Background:
                bgName = EditorGUILayout.TextField("Name: ", bgName);
                GUILayout.Label("*Background will cover the entire canvas.", EditorStyles.miniLabel);
                EditorGUILayout.Space();
                bgSprite = EditorGUILayout.ObjectField("Source Image: ", bgSprite, typeof(Sprite), true, GUILayout.Height(100)) as Sprite;
                EditorGUILayout.Space();
                break;
        }

        color = EditorGUILayout.ColorField("Color: ", color);
        color.a = 255;

        GUILayout.Label("Transform Properties", EditorStyles.miniBoldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 50;
        width = EditorGUILayout.IntField("Width: ", width);
        EditorGUIUtility.labelWidth = 50;
        height = EditorGUILayout.IntField("Height: ", height);
        EditorGUILayout.EndHorizontal();
        pos = EditorGUILayout.Vector2Field("Position: ", pos);
        EditorGUILayout.Space();

        if (GUILayout.Button("Create UI element", GUILayout.Height(30))) {
            CreateComponent(elem);
        }
    }

    private void CreateComponent(UIelement elem) {

        Canvas canvas = (Canvas)FindObjectOfType(typeof(Canvas));

        // If the canvas does not exist, create one. No duplicates.
        if (canvas == null) {
            GameObject newCanvas = new GameObject("Canvas");
            Canvas c = newCanvas.AddComponent<Canvas>();
            canvas = c;
            c.name = canvasName;
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = newCanvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            newCanvas.AddComponent<GraphicRaycaster>();
        }

        switch (elem) {
            case UIelement.Text:
                GameObject newText = new GameObject("Text");
                newText.transform.SetParent(canvas.transform, false);
                newText.AddComponent<CanvasRenderer>();
                Text t = newText.AddComponent<Text>();
                t.name = txtName;
                t.raycastTarget = false;
                t.color = color;
                t.fontSize = txtSize;
                t.text = txt;
                t.alignment = TextAnchor.MiddleCenter;
                t.rectTransform.anchoredPosition = pos;
                t.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                t.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                switch (txtStyle) {
                    case FontStyle.Normal:
                        t.fontStyle = UnityEngine.FontStyle.Normal;
                        break;
                    case FontStyle.Bold:
                        t.fontStyle = UnityEngine.FontStyle.Bold;
                        break;
                    case FontStyle.Italic:
                        t.fontStyle = UnityEngine.FontStyle.Italic;
                        break;
                    case FontStyle.BoldAndItalic:
                        t.fontStyle = UnityEngine.FontStyle.BoldAndItalic;
                        break;
                }
                break;
            case UIelement.Button:
                GameObject newButton = new GameObject("Button");
                newButton.transform.SetParent(canvas.transform, false);
                newButton.AddComponent<RectTransform>();
                newButton.AddComponent<CanvasRenderer>();
                Image bImage = newButton.AddComponent<Image>();
                Button b = newButton.AddComponent<Button>();
                var cb = newButton.GetComponent<Button>().colors;
                cb.normalColor = color;
                cb.highlightedColor = hColor;
                cb.pressedColor = pColor;
                newButton.GetComponent<Button>().colors = cb;
                b.targetGraphic = b.image;
                b.name = btnName;
                bImage.sprite = btnSprite;
                bImage.type = Image.Type.Sliced;
                RectTransform bTransform = newButton.GetComponent<RectTransform>();
                bTransform.anchoredPosition = pos;
                bTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                bTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                GameObject newButtonText = new GameObject("Text");
                newButtonText.transform.SetParent(newButton.transform, false);
                newButtonText.AddComponent<CanvasRenderer>();
                Text tBtn = newButtonText.AddComponent<Text>();
                tBtn.color = txtColor;
                tBtn.fontSize = txtSize;
                tBtn.text = txt;
                tBtn.alignment = TextAnchor.MiddleCenter;
                tBtn.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                tBtn.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                break;
            case UIelement.Image:
                GameObject newImage = new GameObject("Image");
                newImage.transform.SetParent(canvas.transform, false);
                newImage.AddComponent<CanvasRenderer>();
                Image i = newImage.AddComponent<Image>();
                i.raycastTarget = false;
                i.name = imgName;
                i.color = color;
                i.sprite = sprite;
                i.rectTransform.anchoredPosition = pos;
                i.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                i.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                break;
            case UIelement.Background:
                GameObject newBackground = new GameObject("Panel");
                newBackground.transform.SetParent(canvas.transform, false);
                newBackground.transform.SetSiblingIndex(0);
                newBackground.AddComponent<CanvasRenderer>();
                Image bg = newBackground.AddComponent<Image>();
                bg.raycastTarget = false;
                bg.name = bgName;
                bg.color = color;
                bg.sprite = bgSprite;
                bg.type = Image.Type.Sliced;
                bg.rectTransform.anchoredPosition = pos;
                bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.GetComponent<RectTransform>().rect.width);
                bg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.GetComponent<RectTransform>().rect.height);
                break;
        }
    }
}
