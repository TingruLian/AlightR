public class Attribute<T> {
   private T baseValue;
   private T curValue;

   public T GetBaseValue() {
      return curValue;
   }

   public void SetBaseValue(T val) {
      baseValue = val;
   }

   public T GetCurValue() {
      return curValue;
   }

   public void SetCurValue(T val) {
      curValue = val;
   }

   public void ResetCurValue() {
      curValue = baseValue;
   }
}
