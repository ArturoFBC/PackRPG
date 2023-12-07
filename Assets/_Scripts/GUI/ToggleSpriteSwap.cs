using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleSpriteSwap : MonoBehaviour
{

    public Toggle targetToggle;
    public Sprite selectedSprite;
    public Image targetImage;
    private Sprite initialSprite;

    // Use this for initialization
    void Start()
    {
        targetToggle.toggleTransition = Toggle.ToggleTransition.None;
        targetToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);
        initialSprite = targetImage.sprite;
    }

    void OnTargetToggleValueChanged(bool newValue)
    {
        //Image targetImage = targetToggle.graphic as Image;
        if (targetImage != null)
        {
            if (newValue)
            {
                targetImage.overrideSprite = null;
            }
            else
            {
                targetImage.overrideSprite = selectedSprite;
            }
        }
    }
}