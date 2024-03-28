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
        changeMaterials(enemy);
    }

   public override void ResetEffect(EnemyMovement enemy) {
      enemy.speed.ResetCurValue();
      //Destroy(spawnedDeco);
        //enemy.slownDecoration.SetActive(false);
    }

    public void changeMaterials(EnemyMovement enemy)
    {
        materialObject = enemy.gameObject.transform.GetChild(3).GetChild(0).gameObject;
        //Debug.Log(materialObject);
        Renderer renderer = materialObject.GetComponent<SkinnedMeshRenderer>();

        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {

            materials[i].SetFloat("Ice Scale", 0.65f);
            Debug.Log(materials[i] +" " +materials[i].GetFloat("Ice Scale"));
        }
        // Apply the modified materials back to the renderer
        renderer.materials = materials;
    }
}
