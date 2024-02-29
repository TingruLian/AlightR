public class Attribute<T> {
   private T baseValue;
   private T curValue;

   public T GetBaseValue() {
      return baseValue;
   }

   public void SetBaseValue(T val) {
      baseValue = val;
      curValue = val; // may not be the right approach if we allow changing the base value after creation
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
