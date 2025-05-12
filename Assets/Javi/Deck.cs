using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour, IDropHandler
{
	
	public void OnDrop(PointerEventData eventData)
	{
		Drag d = eventData.pointerDrag.GetComponent<Drag>();
		if (d != null)
		{
			d.parentReturn = this.transform;
		}
	}

}
