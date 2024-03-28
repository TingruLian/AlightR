using UnityEngine;

public class SlowEffect : BaseEffect {

   GameObject spawnedDeco;
   private GameObject materialObject; 
   public SlowEffect(float duration)
   : base(duration) {
   }

   public override void ApplyEffect(EnemyMovement enemy) {
      enemy.speed.SetCurValue(enemy.speed.GetBaseValue() * 0.5f);
        //spawnedDeco = Instantiate(enemy.slownDecoration, enemy.transform);
        //enemy.slownDecoration.SetActive(true);
        enemy.changeMaterials();
        //changeMaterials(enemy);
    }

   public override void ResetEffect(EnemyMovement enemy) {
      enemy.speed.ResetCurValue();
      //Destroy(spawnedDeco);
        //enemy.slownDecoration.SetActive(false);
        enemy.changeBackMaterials(duration);
    }

}
