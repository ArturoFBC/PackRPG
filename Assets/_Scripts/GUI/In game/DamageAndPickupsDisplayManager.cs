using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageAndPickupsDisplayManager : MonoBehaviour
{
    public static DamageAndPickupsDisplayManager Ref;

    [SerializeField]
    private Camera _MainCamera;
    [SerializeField]
    private GameObject _DamageDisplayPrefab;
    [SerializeField]
    private Transform _DisplayContainer;
    [SerializeField]
    private Vector2 _HealthBarSize = new Vector2(100, 1);

    private Dictionary<GameObject, UIElementFollow> _MiniHealthBars = new Dictionary<GameObject, UIElementFollow>();

    void Awake ()
    {
        if (Ref == null)
            Ref = this;
        else
            Destroy(this);

        _MainCamera = Camera.main;

        if ( _DisplayContainer == null )
            _DisplayContainer = transform.Find("DisplaysFollowingWorldPanel");
	}
	
    public void DisplayDamage( float damage, Vector3 position, bool player = false, bool critical = false, bool healing = false )
    {
        Text text = Instantiate(_DamageDisplayPrefab, _MainCamera.WorldToScreenPoint(position), Quaternion.identity, _DisplayContainer).GetComponentInChildren<Text>();
        text.text = Mathf.RoundToInt(damage).ToString();
        if (healing)
        {
            text.color = Color.green;
        }
        else if (player)
        {
            text.color = Color.red;
        }
        else if (critical)
        {
            text.color = Color.yellow;
            text.fontStyle = FontStyle.BoldAndItalic;
            text.fontSize += 10;
        }
    }

    public void DisplayManaGain(float manaAmount, Vector3 position)
    {
        Text text = Instantiate(_DamageDisplayPrefab, _MainCamera.WorldToScreenPoint(position), Quaternion.identity, _DisplayContainer).GetComponentInChildren<Text>();
        text.text = Mathf.RoundToInt(manaAmount).ToString();
        text.color = Color.cyan;
    }

    public void DisplayPickup( int amount, string itemName, Vector3 position)
    {
        Text text = Instantiate(_DamageDisplayPrefab, _MainCamera.WorldToScreenPoint(position), Quaternion.identity, _DisplayContainer).GetComponentInChildren<Text>();
        text.text = ((amount != 1) ? amount.ToString() + " " + itemName : itemName);
        text.color = Color.yellow;
    }

    public void DisplayMessage( string message, Vector3 position, Color color, Color outlineColor, int textSize = 12, bool ornateFont = false)
    {
        GameObject messageGO = new GameObject();

        RectTransform messageTransform = messageGO.AddComponent<RectTransform>();
        messageTransform.position = _MainCamera.WorldToScreenPoint(position);
        messageTransform.rotation = Quaternion.identity;
        messageTransform.SetParent( _DisplayContainer );

        Text messageText = messageGO.AddComponent<Text>();
        messageText.text = message;
        messageText.color = color;
        messageText.fontSize = textSize;
        messageText.font = ornateFont ? IconsAndEffects._Ref.ornateFont : IconsAndEffects._Ref.ornateFont;
        messageText.horizontalOverflow = HorizontalWrapMode.Overflow;
        messageText.verticalOverflow = VerticalWrapMode.Overflow;

        Outline messageOutline = messageGO.AddComponent<Outline>();
        messageOutline.effectColor = outlineColor;

        DestroyInXseconds messageAutoDestroy = messageGO.AddComponent<DestroyInXseconds>();
        messageAutoDestroy.timeToBeDestroyed = 5f;
    }

    public void DisplayHealthBar( GameObject creature )
    {
        if (_MiniHealthBars.ContainsKey(creature) == false)
        {
            GameObject healthBarGO = new GameObject("MiniHealthBar");

            RectTransform healthBarTransform = healthBarGO.AddComponent<RectTransform>();
            healthBarTransform.SetParent(_DisplayContainer, true);
            healthBarTransform.sizeDelta = _HealthBarSize;
            healthBarTransform.localScale = Vector3.one;

            DividedHealthBar healthBar = healthBarGO.AddComponent<DividedHealthBar>();
            healthBar.delayedGaugeColorDecreasing = Color.white;
            healthBar.delayedGaugeColorIncreasing = Color.green;
            healthBar.divisionColor = Color.black;

            Image backgroundImage = healthBarGO.AddComponent<Image>();
            backgroundImage.color = Color.black;
            healthBarGO.AddComponent<Mask>();

            Image delayedGaugeImage = CreateImage(healthBarTransform, "DelayedGauge");
            healthBar._DelayedGauge = delayedGaugeImage.rectTransform;

            Image healthGaugeImage = CreateImage(healthBarTransform, "HealthGauge");
            healthGaugeImage.color = Color.red;
            healthBar._LifeGauge = healthGaugeImage.rectTransform;
            healthBar.SetTarget(creature, GaugeData.LIFE);

            UIElementFollow minihealthBar = healthBarGO.AddComponent<UIElementFollow>();
            minihealthBar.SetFollowing(creature.transform);

            _MiniHealthBars.Add(creature, minihealthBar);
        }
        else
        {
            _MiniHealthBars[creature].RefreshDuration();
        }
    }

    public void DestroyMiniHealthBar(GameObject creature)
    {
        if (_MiniHealthBars.ContainsKey(creature))
        {
            if ( _MiniHealthBars[creature] != null )
                Destroy(_MiniHealthBars[creature].gameObject);

            _MiniHealthBars.Remove(creature);
        }
    }

    public Image CreateImage( Transform parent, string name )
    {
        GameObject backgroundGO = new GameObject(name);
        RectTransform backgroundTransform = backgroundGO.AddComponent<RectTransform>();
        backgroundTransform.SetParent( parent, true );
        backgroundTransform.sizeDelta = _HealthBarSize;
        backgroundTransform.anchorMax = Vector2.one;
        backgroundTransform.anchorMin = Vector2.zero;
        backgroundTransform.offsetMax = Vector2.zero;
        backgroundTransform.offsetMin = Vector2.zero;
        backgroundTransform.pivot = new Vector2(0f, 0.5f);
        backgroundTransform.localScale = Vector3.one;

        return backgroundGO.AddComponent<Image>();
    }
}
