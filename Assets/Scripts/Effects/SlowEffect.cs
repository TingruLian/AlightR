public class SlowEffect : BaseEffect {
   public SlowEffect(float duration)
   : base(duration) {
   }

   public override void ApplyEffect(EnemyMovement enemy) {
      enemy.speed.SetCurValue(enemy.speed.GetBaseValue() * 0.5f);
   }

   public override void ResetEffect(EnemyMovement enemy) {
      enemy.speed.ResetCurValue();
   }
}
