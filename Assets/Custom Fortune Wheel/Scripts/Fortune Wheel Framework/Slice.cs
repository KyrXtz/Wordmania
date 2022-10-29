using System;
using UnityEngine;
public class Slice : MonoBehaviour
{
    FortuneWheel fortuneWheel;
    Tuple<int, string> stats;
    private void Start() {
        fortuneWheel = gameObject.transform.parent.parent.GetComponent<FortuneWheel>();  
    }
   private void OnTriggerEnter2D(Collider2D other) {
       fortuneWheel?.RecieveLatestTick(stats.Item1);
       
       if(other.GetComponent<Animator>() != null)
            other.GetComponent<Animator>().SetTrigger("Kick");
   }
   public void SetStats(Tuple<int, string> stats)
   {
       this.stats = stats;
   }
}
