using UnityEngine;

public class PutCard : MonoBehaviour
{
     //Transform oldposition;
    public Transform[] CardSlots;

    private void Start()
    {
        //oldposition.position = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
		
    }

    public void CheckPositions()
	{
		if (CardSlots != null)
		{
			for (int i = 0; i < CardSlots.Length; i++)
			{
				if (CardSlots[i] != null && CardSlots[i] != this.gameObject)
				{
					this.gameObject.transform.position = CardSlots[i].position;
				}
			}
		}
	}
}
