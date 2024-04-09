using UnityEngine;
using UnityEngine.EventSystems;

public class CartController : MonoBehaviour, IPointerClickHandler {

   public CartMovement cart;

   public void OnPointerClick(PointerEventData eventData) {
      if (cart != null) {
         cart.MoveCart();
      }
   }
}
