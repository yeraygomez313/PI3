using System.Collections.Generic;
using UnityEngine;

public class PreparationCard : DraggableItem
{
    public bool hasbeenplayed = false;
	private Vector3 oldposition;
	public GameObject[] CardSlots;
	private int freeslot;
	private int oldfreeslot;
	[SerializeField] private CardVisuals cardVisuals;
	[SerializeField] private CardVisuals cardVisuals2;

	public override ItemInstance ItemInstance
	{
		get => CardInstance;
		protected set => CardInstance = value as CardInstance;
	}
	public CardInstance CardInstance { get; private set; }

	public override void SetItem(ItemInstance card)
	{
		if (card is not CardInstance cardInstance) return;
		CardInstance = cardInstance;
		cardVisuals.SetCard(cardInstance);
		cardVisuals2.SetCard(cardInstance);
	}


	private void OnMouseDown()
	{
		if (CardSlots != null)
		{
			if (hasbeenplayed)
			{
				this.gameObject.transform.position = oldposition;
				CardSlots[oldfreeslot].GetComponent<CardSlot>().isOccupied = false;
				hasbeenplayed = false;
			}
			else
			{
				freeslot = -1;
				for (int i = 0; i < CardSlots.Length; i++)
				{
					if (CardSlots[i].GetComponent<CardSlot>().isOccupied == false)
					{
						freeslot = i;
						break;
					}

				}
				if (freeslot == -1)
				{
					Debug.LogError("Error, no hay huecos libres");
				}
				else
				{
					CardSlots[freeslot].GetComponent<CardSlot>().isOccupied = true;
					hasbeenplayed = true;
					oldfreeslot = freeslot;
					oldposition = this.gameObject.transform.position;
					this.gameObject.transform.position = CardSlots[freeslot].transform.position;
				}
			}

		}
	}
}
