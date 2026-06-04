using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag; // O slot para onde ele vai
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Antes de começar, ele salva o slot atual (Loja) como destino padrão
        parentAfterDrag = transform.parent;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 2. No final, ele volta para o parentAfterDrag. 
        // Se ele foi solto no vazio, o parentAfterDrag ainda é o slot da loja!
        transform.SetParent(parentAfterDrag);

        // 3. Zera a posição para ele encaixar centralizado no slot
        transform.localPosition = Vector3.zero;

        canvasGroup.blocksRaycasts = true;
    }
}