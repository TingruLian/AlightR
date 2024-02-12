using TMPro;

public class UIField {

   private string label;
   private string value;
   private TMP_Text uiElement;

   public UIField(string label, TMP_Text uiElement, string defaultValue = "") {
      this.label = label;
      this.uiElement = uiElement;
      value = defaultValue;

      updateValue(defaultValue);
   }

   // The caller must convert whatever value they're updating to a string
   // might add convenience functions to set values of different types later
   public void updateValue(string value) {
      this.value = value;

      updateUI();
   }

   private void updateUI() {
      uiElement.text = label + ": " + value;
   }
}