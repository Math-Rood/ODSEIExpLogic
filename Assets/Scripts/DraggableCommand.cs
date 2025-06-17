using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 

public enum CommandType { MoveForward, TurnRight, TurnLeft }

public class DraggableCommand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CommandType commandType; 

    public RectTransform rectTransform;
    private CanvasGroup canvasGroup; 
    private Transform parentBeforeDrag; 
    private Image commandImage; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        commandImage = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent; 
        transform.SetParent(transform.root); 
        canvasGroup.blocksRaycasts = false; 
        commandImage.raycastTarget = false; 
    }

    
    public void OnDrag(PointerEventData eventData)
    {

        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

        public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; 
        commandImage.raycastTarget = true; 

        
        if (transform.parent == transform.root)
        {
            transform.SetParent(parentBeforeDrag);
            rectTransform.anchoredPosition = Vector2.zero; 
        }
    }
}