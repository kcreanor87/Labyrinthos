using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour, ISelectHandler {

	public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " selected");
    }
}
