using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public Transform parentReturn = null;
	[SerializeField] private GameObject allie;
	public void OnBeginDrag(PointerEventData eventData)
	{
		parentReturn = this.transform.parent;
		//this.transform.SetParent(this.transform.parent.parent);
		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.position = mousePos;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this.transform.SetParent(parentReturn);

		GetComponent<CanvasGroup>().blocksRaycasts = true;

		//EventSystem.current.RaycastAll(eventData);

		/*
		 * 		Instantiate(allie, transform.position, Quaternion.identity);
		this.gameObject.SetActive(false);
		 */
	}
}
