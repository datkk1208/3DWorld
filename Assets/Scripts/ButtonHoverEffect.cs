using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float HoverScale = 1.1f;

    [Tooltip("Chỉ cần kéo Object Text (TMP) vào đây, code sẽ tự tìm hiệu ứng")]
    public GameObject TextObject;

    private Vector3 _originalScale;
    private MonoBehaviour _glowFilter;

    private void Start()
    {
        _originalScale = transform.localScale;

        // Tự động quét và tìm đúng script UIFX
        if (TextObject != null)
        {
            MonoBehaviour[] scripts = TextObject.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script.GetType().Name.Contains("FilterStack"))
                {
                    _glowFilter = script;
                    _glowFilter.enabled = false; // Tắt lúc mới vào
                    break;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = _originalScale * HoverScale;
        if (_glowFilter != null) _glowFilter.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = _originalScale;
        if (_glowFilter != null) _glowFilter.enabled = false;
    }
}