using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public GUISkin customSkin;// GUISkin
    public string text = null;// Text
    public float lifeTime = 1f;// Life time
    public float lifeTimer = 0f;// Life time
    public bool fadeEnd = false;// Fade out at last 1 second before destroyed
    public Color textColor = Color.white; // Text color
    public bool position3D = false; // enabled when you need the text along with world 3d position
    public Vector2 position; // 2D Position

    private float alpha = 1;
    private float timeTemp = 0;



    void Start()
    {
        timeTemp = Time.time;

        if (position3D)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            position = new Vector2(screenPos.x, Screen.height - screenPos.y);
        }
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            lifeTimer = 0f;
            transform.SetParent(UIManager.Instance.damageTxtPool.transform);

            gameObject.SetActive(false);
            return;
        }

        if (fadeEnd)
        {
            if (Time.time >= ((timeTemp + lifeTime) - 1))
            {
                alpha = 1.0f - (Time.time - ((timeTemp + lifeTime) - 1));
            }
        }
        else
        {
            alpha = 1.0f - ((1.0f / lifeTime) * (Time.time - timeTemp));
        }

        if (position3D)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            position = new Vector2(screenPos.x, Screen.height - screenPos.y);
        }
    }


    void OnGUI()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);

        if (customSkin)
        {
            GUI.skin = customSkin;
        }

        Vector2 textsize = GUI.skin.label.CalcSize(new GUIContent(text));
        Rect rect = new Rect(position.x - (textsize.x / 2), position.y, textsize.x, textsize.y);

        GUI.skin.label.normal.textColor = textColor;
        GUI.Label(rect, text);
    }
}
